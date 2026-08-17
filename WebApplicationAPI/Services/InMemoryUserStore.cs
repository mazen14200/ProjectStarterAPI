using System.Collections.Concurrent;
using Microsoft.AspNetCore.Identity;
using WebApplicationAPI.Models.AuthAndUser;

namespace WebApplicationAPI.Services;

public interface IUserStore
{
    User? FindByEmail(string email);
    User? FindById(Guid id);
    User Create(string email, string password, string fullName);
    bool VerifyPassword(User user, string password);
    void RegisterFailedAttempt(User user);
    void ResetFailedAttempts(User user);
    bool IsLockedOut(User user);
}

/// <summary>
/// Demo-only in-memory user store. Replace with a real repository backed by
/// your database — keep the same password-hashing and lockout behavior.
/// Uses ASP.NET Core Identity's PasswordHasher (PBKDF2), independent of
/// the full Identity UI/cookie stack, so it fits a stateless JWT API.
/// </summary>
public class InMemoryUserStore : IUserStore
{
    private readonly ConcurrentDictionary<string, User> _usersByEmail = new(StringComparer.OrdinalIgnoreCase);
    private readonly PasswordHasher<User> _hasher = new();

    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

    public User? FindByEmail(string email) =>
        _usersByEmail.TryGetValue(email, out var user) ? user : null;

    public User? FindById(Guid id) =>
        _usersByEmail.Values.FirstOrDefault(u => u.Id == id);

    public User Create(string email, string password, string fullName)
    {
        var user = new User { Email = email, FullName = fullName };
        user.PasswordHash = _hasher.HashPassword(user, password);

        if (!_usersByEmail.TryAdd(email, user))
        {
            throw new InvalidOperationException("A user with this email already exists.");
        }

        return user;
    }

    public bool VerifyPassword(User user, string password)
    {
        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);

        // Transparently rehash if the hasher's algorithm/iteration count was upgraded
        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _hasher.HashPassword(user, password);
        }

        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }

    public bool IsLockedOut(User user) =>
        user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow;

    public void RegisterFailedAttempt(User user)
    {
        user.AccessFailedCount++;
        if (user.AccessFailedCount >= MaxFailedAttempts)
        {
            user.LockoutEnd = DateTimeOffset.UtcNow.Add(LockoutDuration);
        }
    }

    public void ResetFailedAttempts(User user)
    {
        user.AccessFailedCount = 0;
        user.LockoutEnd = null;
    }
}
