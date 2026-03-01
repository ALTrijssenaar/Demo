using System.Collections.Concurrent;

namespace MatchHub.Api.Domain;

public sealed class ScoreStore
    : IScoreRepository
{
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, int>> _totals = new();
    private readonly ConcurrentDictionary<string, ConcurrentQueue<ScoreEntry>> _entries = new();

    public int AddScore(string matchId, string playerId, int delta, DateTimeOffset timestamp)
    {
        var perMatchTotals = _totals.GetOrAdd(matchId, static _ => new ConcurrentDictionary<string, int>(StringComparer.Ordinal));
        var updated = perMatchTotals.AddOrUpdate(playerId, delta, (_, current) => current + delta);

        var queue = _entries.GetOrAdd(matchId, static _ => new ConcurrentQueue<ScoreEntry>());
        queue.Enqueue(new ScoreEntry(playerId, updated, timestamp));

        return updated;
    }

    public Task<int> AddScoreAsync(
        string matchId,
        string playerId,
        int delta,
        DateTimeOffset timestamp,
        string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        _ = idempotencyKey;
        _ = cancellationToken;
        return Task.FromResult(AddScore(matchId, playerId, delta, timestamp));
    }

    public IReadOnlyList<ScoreEntry> GetScores(string matchId)
    {
        if (!_entries.TryGetValue(matchId, out var queue))
        {
            return [];
        }

        return queue.ToArray().OrderBy(item => item.Timestamp).ToList();
    }

    public Task<IReadOnlyList<ScoreEntry>> GetScoresAsync(string matchId, CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return Task.FromResult(GetScores(matchId));
    }
}
