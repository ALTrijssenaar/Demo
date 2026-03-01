namespace MatchHub.Api.Domain;

public interface IScoreRepository
{
    Task<int> AddScoreAsync(
        string matchId,
        string playerId,
        int delta,
        DateTimeOffset timestamp,
        string? idempotencyKey,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ScoreEntry>> GetScoresAsync(string matchId, CancellationToken cancellationToken);
}
