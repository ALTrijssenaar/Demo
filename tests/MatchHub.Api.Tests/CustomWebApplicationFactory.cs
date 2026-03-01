using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MatchHub.Api.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace MatchHub.Api.Tests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string Issuer = "MatchHub";
    private const string Audience = "MatchHubClients";
    private const string SigningKey = "matchhub-super-secret-development-signing-key-2026";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["UseInMemoryScoreRepository"] = "true",
                ["Jwt:Issuer"] = Issuer,
                ["Jwt:Audience"] = Audience,
                ["Jwt:Key"] = SigningKey,
                ["RateLimit:ScoreSubmission:PermitLimit"] = "10000",
                ["RateLimit:ScoreSubmission:WindowSeconds"] = "1",
                ["RateLimit:ScoreRetrieval:PermitLimit"] = "10000",
                ["RateLimit:ScoreRetrieval:WindowSeconds"] = "1"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IScoreRepository>();
            services.RemoveAll<ScoreStore>();
            services.AddSingleton<ScoreStore>();
            services.AddSingleton<IScoreRepository>(sp => sp.GetRequiredService<ScoreStore>());
        });
    }

    public static string CreateBearerToken(string role = "scorer")
    {
        var handler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = handler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = Issuer,
            Audience = Audience,
            Expires = DateTime.UtcNow.AddMinutes(30),
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, "test-user"),
                new Claim(ClaimTypes.Role, role)
            ]),
            SigningCredentials = credentials
        });

        return handler.WriteToken(token);
    }
}
