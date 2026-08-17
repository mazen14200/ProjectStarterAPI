namespace WebApplicationAPI.Models.AuthAndUser;

/// <summary>
/// Minimal user record. Swap the backing store (InMemoryUserStore) for a
/// real EF Core / Dapper repository against your database — the password
/// is never stored or logged in plain text, only PasswordHasher output.
/// </summary>
public class User
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = ["User"];
    public int AccessFailedCount { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
}
