namespace MatchHub.Api.Domain;

public sealed record ScoreSubmission(string PlayerId, int Score, DateTimeOffset Timestamp);

public sealed record ScoreEntry(string PlayerId, int Score, DateTimeOffset Timestamp);

public sealed class MatchScoresResponse
{
    public required string MatchId { get; init; }
    public required IReadOnlyList<ScoreEntry> Scores { get; init; }
}

public sealed class SubmitScoreResponse
{
    public bool Success { get; init; }
    public required int UpdatedScore { get; init; }
    public required string MatchId { get; init; }
}
