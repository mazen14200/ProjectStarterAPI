using System.Collections.Concurrent;

namespace WebApplicationAPI.Services;

public record RefreshTokenEntry(Guid UserId, DateTimeOffset ExpiresAt, bool Revoked);

public interface IRefreshTokenStore
{
    void Store(string token, Guid userId, TimeSpan lifetime);
    RefreshTokenEntry? Validate(string token);
    void Revoke(string token);
    void RevokeAllForUser(Guid userId);
}

/// <summary>
/// Demo-only in-memory refresh token store. In production, persist these
/// (hashed, not raw) in your database so tokens survive app restarts and
/// can be audited/revoked across instances.
/// </summary>
public class InMemoryRefreshTokenStore : IRefreshTokenStore
{
    private readonly ConcurrentDictionary<string, RefreshTokenEntry> _tokens = new();

    public void Store(string token, Guid userId, TimeSpan lifetime)
    {
        _tokens[token] = new RefreshTokenEntry(userId, DateTimeOffset.UtcNow.Add(lifetime), Revoked: false);
    }

    public RefreshTokenEntry? Validate(string token)
    {
        if (!_tokens.TryGetValue(token, out var entry)) return null;
        if (entry.Revoked || entry.ExpiresAt < DateTimeOffset.UtcNow) return null;
        return entry;
    }

    public void Revoke(string token)
    {
        if (_tokens.TryGetValue(token, out var entry))
        {
            _tokens[token] = entry with { Revoked = true };
        }
    }

    public void RevokeAllForUser(Guid userId)
    {
        foreach (var kvp in _tokens.Where(t => t.Value.UserId == userId && !t.Value.Revoked))
        {
            _tokens[kvp.Key] = kvp.Value with { Revoked = true };
        }
    }
}
