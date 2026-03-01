using MatchHub.Api.Domain;
using Npgsql;

namespace MatchHub.Api.Infrastructure;

public sealed class PostgresScoreRepository : IScoreRepository
{
    private readonly string _connectionString;

    public PostgresScoreRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        var sql = """
            CREATE TABLE IF NOT EXISTS player_scores (
              match_id TEXT NOT NULL,
              player_id TEXT NOT NULL,
              total_score INTEGER NOT NULL,
              PRIMARY KEY (match_id, player_id)
            );

            CREATE TABLE IF NOT EXISTS score_events (
              id BIGSERIAL PRIMARY KEY,
              match_id TEXT NOT NULL,
              player_id TEXT NOT NULL,
              total_score INTEGER NOT NULL,
              submitted_at TIMESTAMPTZ NOT NULL,
              request_key TEXT NULL
            );

            CREATE UNIQUE INDEX IF NOT EXISTS ux_score_events_request_key
              ON score_events(request_key)
              WHERE request_key IS NOT NULL;

            CREATE INDEX IF NOT EXISTS ix_score_events_match_id_submitted_at
              ON score_events(match_id, submitted_at, id);
            """;

        await using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<int> AddScoreAsync(
        string matchId,
        string playerId,
        int delta,
        DateTimeOffset timestamp,
        string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(idempotencyKey))
        {
            var existing = await TryGetByRequestKeyAsync(connection, transaction, idempotencyKey, cancellationToken);
            if (existing.HasValue)
            {
                await transaction.CommitAsync(cancellationToken);
                return existing.Value;
            }
        }

        const string upsertSql = """
            INSERT INTO player_scores (match_id, player_id, total_score)
            VALUES (@matchId, @playerId, @delta)
            ON CONFLICT (match_id, player_id)
            DO UPDATE SET total_score = player_scores.total_score + EXCLUDED.total_score
            RETURNING total_score;
            """;

        await using var upsert = new NpgsqlCommand(upsertSql, connection, transaction);
        upsert.Parameters.AddWithValue("matchId", matchId);
        upsert.Parameters.AddWithValue("playerId", playerId);
        upsert.Parameters.AddWithValue("delta", delta);

        var updatedScore = Convert.ToInt32(await upsert.ExecuteScalarAsync(cancellationToken));

        const string eventSql = """
            INSERT INTO score_events (match_id, player_id, total_score, submitted_at, request_key)
            VALUES (@matchId, @playerId, @updatedScore, @submittedAt, @requestKey);
            """;

        await using var insertEvent = new NpgsqlCommand(eventSql, connection, transaction);
        insertEvent.Parameters.AddWithValue("matchId", matchId);
        insertEvent.Parameters.AddWithValue("playerId", playerId);
        insertEvent.Parameters.AddWithValue("updatedScore", updatedScore);
        insertEvent.Parameters.AddWithValue("submittedAt", timestamp);
        insertEvent.Parameters.AddWithValue("requestKey", (object?)idempotencyKey ?? DBNull.Value);

        try
        {
            await insertEvent.ExecuteNonQueryAsync(cancellationToken);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation && !string.IsNullOrWhiteSpace(idempotencyKey))
        {
            var existing = await TryGetByRequestKeyAsync(connection, transaction, idempotencyKey!, cancellationToken);
            if (existing.HasValue)
            {
                await transaction.CommitAsync(cancellationToken);
                return existing.Value;
            }

            throw;
        }

        await transaction.CommitAsync(cancellationToken);
        return updatedScore;
    }

    public async Task<IReadOnlyList<ScoreEntry>> GetScoresAsync(string matchId, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string query = """
            SELECT player_id, total_score, submitted_at
            FROM score_events
            WHERE match_id = @matchId
            ORDER BY submitted_at ASC, id ASC;
            """;

        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("matchId", matchId);

        var result = new List<ScoreEntry>();

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var playerId = reader.GetString(0);
            var totalScore = reader.GetInt32(1);
            var submittedAt = reader.GetFieldValue<DateTimeOffset>(2);
            result.Add(new ScoreEntry(playerId, totalScore, submittedAt));
        }

        return result;
    }

    private static async Task<int?> TryGetByRequestKeyAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        string requestKey,
        CancellationToken cancellationToken)
    {
        const string query = """
            SELECT total_score
            FROM score_events
            WHERE request_key = @requestKey
            LIMIT 1;
            """;

        await using var command = new NpgsqlCommand(query, connection, transaction);
        command.Parameters.AddWithValue("requestKey", requestKey);
        var value = await command.ExecuteScalarAsync(cancellationToken);

        if (value is null || value is DBNull)
        {
            return null;
        }

        return Convert.ToInt32(value);
    }
}
