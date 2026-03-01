using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using MatchHub.Api.Domain;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MatchHub.Api.Tests;

public sealed class ScoreApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ScoreApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostScore_WithoutAuthorization_ReturnsUnauthorized()
    {
        var payload = new ScoreSubmission("player-1", 1, DateTimeOffset.UtcNow);

        var response = await _client.PostAsJsonAsync("/api/matches/match-1/scores", payload);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetScore_WithoutAuthorization_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/matches/match-1/scores");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PostScore_WithInvalidScore_ReturnsBadRequest()
    {
        var response = await PostScoreAsync("match-1", new ScoreSubmission("player-1", -1, DateTimeOffset.UtcNow));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostScore_WithTooLongMatchId_ReturnsBadRequest()
    {
        var longMatchId = new string('m', 101);
        var response = await PostScoreAsync(longMatchId, new ScoreSubmission("player-1", 5, DateTimeOffset.UtcNow));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("100 characters or less", content);
    }

    [Fact]
    public async Task PostScore_WithTooLongPlayerId_ReturnsBadRequest()
    {
        var longPlayerId = new string('p', 101);
        var response = await PostScoreAsync("match-validation", new ScoreSubmission(longPlayerId, 5, DateTimeOffset.UtcNow));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("100 characters or less", content);
    }

    [Fact]
    public async Task GetScores_WithTooLongMatchId_ReturnsBadRequest()
    {
        var longMatchId = new string('m', 101);
        var response = await GetScoresAsync(longMatchId);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("100 characters or less", content);
    }

    [Fact]
    public async Task PostScore_ThenGetScores_PersistsAndReturnsEntries()
    {
        var post = await PostScoreAsync("match-2", new ScoreSubmission("player-a", 3, DateTimeOffset.UtcNow));

        post.EnsureSuccessStatusCode();

        var get = await GetScoresAsync("match-2");
        get.EnsureSuccessStatusCode();

        var payload = await get.Content.ReadFromJsonAsync<MatchScoresResponse>();
        Assert.NotNull(payload);
        Assert.Equal("match-2", payload.MatchId);
        Assert.Single(payload.Scores);
        Assert.Equal("player-a", payload.Scores[0].PlayerId);
        Assert.Equal(3, payload.Scores[0].Score);
    }

    [Fact]
    public async Task ConcurrentSubmissions_DoNotLoseScores()
    {
        const string matchId = "match-concurrency";
        var tasks = Enumerable.Range(1, 25).Select(i =>
            PostScoreAsync(matchId, new ScoreSubmission("player-a", 1, DateTimeOffset.UtcNow.AddMilliseconds(i))));

        var responses = await Task.WhenAll(tasks);
        Assert.All(responses, r => Assert.Equal(HttpStatusCode.OK, r.StatusCode));

        using var getResponse = await GetScoresAsync(matchId);
        getResponse.EnsureSuccessStatusCode();
        var response = await getResponse.Content.ReadFromJsonAsync<MatchScoresResponse>();

        Assert.NotNull(response);
        Assert.Equal(25, response.Scores.Count);
        Assert.Equal(25, response.Scores.Max(item => item.Score));
    }

    [Fact]
    public async Task ScoreSubmission_AverageLatencyUnderOneSecond()
    {
        const int iterations = 20;
        var elapsed = new List<long>(iterations);

        for (var i = 0; i < iterations; i++)
        {
            var sw = Stopwatch.StartNew();
            var response = await PostScoreAsync("match-perf", new ScoreSubmission("player-perf", 1, DateTimeOffset.UtcNow.AddMilliseconds(i)));
            sw.Stop();

            response.EnsureSuccessStatusCode();
            elapsed.Add(sw.ElapsedMilliseconds);
        }

        var p95 = Percentile(elapsed, 0.95);
        Assert.True(p95 < 1000, $"95th percentile latency was {p95}ms");
    }

    private async Task<HttpResponseMessage> PostScoreAsync(string matchId, ScoreSubmission submission)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"/api/matches/{matchId}/scores")
        {
            Content = JsonContent.Create(submission)
        };

        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", CustomWebApplicationFactory.CreateBearerToken("scorer"));
        return await _client.SendAsync(request);
    }

    private async Task<HttpResponseMessage> GetScoresAsync(string matchId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/matches/{matchId}/scores");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", CustomWebApplicationFactory.CreateBearerToken("scorer"));
        return await _client.SendAsync(request);
    }

    private static double Percentile(List<long> values, double percentile)
    {
        var ordered = values.OrderBy(v => v).ToArray();
        var index = (int)Math.Ceiling(percentile * ordered.Length) - 1;
        index = Math.Clamp(index, 0, ordered.Length - 1);
        return ordered[index];
    }
}
