using MatchHub.Api.Domain;
using MatchHub.Api.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "MatchHub";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "MatchHubClients";
var jwtKey = builder.Configuration["Jwt:Key"] ?? "matchhub-super-secret-development-signing-key-2026";

builder.Services
	.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateIssuerSigningKey = true,
			ValidateLifetime = true,
			ValidIssuer = jwtIssuer,
			ValidAudience = jwtAudience,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
			ClockSkew = TimeSpan.FromSeconds(10)
		};
	});

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("CanSubmitScore", policy => policy.RequireRole("scorer"));
});

builder.Services.AddRateLimiter(options =>
{
	options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

	options.AddFixedWindowLimiter("ScoreSubmission", limiter =>
	{
		limiter.PermitLimit = builder.Configuration.GetValue<int>("RateLimit:ScoreSubmission:PermitLimit", 30);
		limiter.Window = TimeSpan.FromSeconds(builder.Configuration.GetValue<int>("RateLimit:ScoreSubmission:WindowSeconds", 10));
		limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
		limiter.QueueLimit = 0;
	});

	options.AddFixedWindowLimiter("ScoreRetrieval", limiter =>
	{
		limiter.PermitLimit = builder.Configuration.GetValue<int>("RateLimit:ScoreRetrieval:PermitLimit", 60);
		limiter.Window = TimeSpan.FromSeconds(builder.Configuration.GetValue<int>("RateLimit:ScoreRetrieval:WindowSeconds", 10));
		limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
		limiter.QueueLimit = 0;
	});
});

var useInMemoryScoreRepository = builder.Configuration.GetValue<bool>("UseInMemoryScoreRepository");
if (useInMemoryScoreRepository)
{
	builder.Services.AddSingleton<IScoreRepository, ScoreStore>();
}
else
{
	var connectionString = builder.Configuration.GetConnectionString("Postgres");
	if (string.IsNullOrWhiteSpace(connectionString))
	{
		throw new InvalidOperationException("ConnectionStrings:Postgres must be configured when UseInMemoryScoreRepository is false.");
	}

	builder.Services.AddSingleton<PostgresScoreRepository>(_ => new PostgresScoreRepository(connectionString));
	builder.Services.AddSingleton<IScoreRepository>(sp => sp.GetRequiredService<PostgresScoreRepository>());
}

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("MatchHub API starting, repository mode: {RepositoryMode}", useInMemoryScoreRepository ? "InMemory" : "PostgreSQL");

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

if (!useInMemoryScoreRepository)
{
	using var scope = app.Services.CreateScope();
	var repository = scope.ServiceProvider.GetRequiredService<PostgresScoreRepository>();
	await repository.InitializeAsync(CancellationToken.None);
}

var matches = app.MapGroup("/api/matches").RequireAuthorization();

matches.MapPost("/{matchId}/scores", async (
	string matchId,
	ScoreSubmission request,
	HttpRequest httpRequest,
	IScoreRepository scoreRepository,
	ILogger<Program> logger,
	CancellationToken cancellationToken) =>
{
	if (string.IsNullOrWhiteSpace(matchId))
	{
		logger.LogWarning("Score submission rejected: matchId missing");
		return Results.BadRequest(new { error = "matchId is required" });
	}

	if (matchId.Length > 100)
	{
		logger.LogWarning("Score submission rejected: matchId too long ({Length} chars)", matchId.Length);
		return Results.BadRequest(new { error = "matchId must be 100 characters or less" });
	}

	if (string.IsNullOrWhiteSpace(request.PlayerId))
	{
		logger.LogWarning("Score submission rejected: playerId missing for match {MatchId}", matchId);
		return Results.BadRequest(new { error = "playerId is required" });
	}

	if (request.PlayerId.Length > 100)
	{
		logger.LogWarning("Score submission rejected: playerId too long ({Length} chars) for match {MatchId}", request.PlayerId.Length, matchId);
		return Results.BadRequest(new { error = "playerId must be 100 characters or less" });
	}

	if (request.Score < 0 || request.Score > 100)
	{
		logger.LogWarning("Score submission rejected: invalid score {Score} for player {PlayerId} in match {MatchId}", request.Score, request.PlayerId, matchId);
		return Results.BadRequest(new { error = "score must be between 0 and 100" });
	}

	if (request.Timestamp == default)
	{
		logger.LogWarning("Score submission rejected: missing timestamp for player {PlayerId} in match {MatchId}", request.PlayerId, matchId);
		return Results.BadRequest(new { error = "timestamp is required" });
	}

	var idempotencyKey = httpRequest.Headers.TryGetValue("Idempotency-Key", out var key)
		? key.ToString()
		: null;

	logger.LogInformation("Processing score submission: match={MatchId}, player={PlayerId}, delta={Score}, idempotent={IsIdempotent}",
		matchId, request.PlayerId, request.Score, !string.IsNullOrWhiteSpace(idempotencyKey));

	try
	{
		var updatedScore = await scoreRepository.AddScoreAsync(
			matchId,
			request.PlayerId,
			request.Score,
			request.Timestamp,
			idempotencyKey,
			cancellationToken);

		logger.LogInformation("Score submitted successfully: match={MatchId}, player={PlayerId}, newTotal={UpdatedScore}",
			matchId, request.PlayerId, updatedScore);

		return Results.Ok(new SubmitScoreResponse
		{
			Success = true,
			UpdatedScore = updatedScore,
			MatchId = matchId
		});
	}
	catch (Exception ex)
	{
		logger.LogError(ex, "Score submission failed: match={MatchId}, player={PlayerId}", matchId, request.PlayerId);
		throw;
	}
})
.RequireAuthorization("CanSubmitScore")
.RequireRateLimiting("ScoreSubmission");

matches.MapGet("/{matchId}/scores", async (string matchId, IScoreRepository scoreRepository, ILogger<Program> logger, CancellationToken cancellationToken) =>
{
	if (string.IsNullOrWhiteSpace(matchId))
	{
		logger.LogWarning("Score retrieval rejected: matchId missing");
		return Results.BadRequest(new { error = "matchId is required" });
	}

	if (matchId.Length > 100)
	{
		logger.LogWarning("Score retrieval rejected: matchId too long ({Length} chars)", matchId.Length);
		return Results.BadRequest(new { error = "matchId must be 100 characters or less" });
	}

	logger.LogInformation("Retrieving scores for match {MatchId}", matchId);

	var response = new MatchScoresResponse
	{
		MatchId = matchId,
		Scores = await scoreRepository.GetScoresAsync(matchId, cancellationToken)
	};

	logger.LogDebug("Retrieved {Count} score entries for match {MatchId}", response.Scores.Count, matchId);

	return Results.Ok(response);
})
.RequireRateLimiting("ScoreRetrieval");

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();

public partial class Program;
