using Volkswagen.Dashboard.Domain.Common;

namespace Volkswagen.Dashboard.Domain.Users;

public sealed class User
{
    private User(
        string username,
        EmailAddress email,
        string passwordHash,
        DateTime createdAt,
        string? whitelistEntryId)
    {
        ChangeUsername(username);
        ChangePasswordHash(passwordHash);
        Email = email;
        CreatedAt = createdAt;
        WhitelistEntryId = whitelistEntryId;
    }

    public string Username { get; private set; } = string.Empty;
    public EmailAddress Email { get; }
    public string PasswordHash { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; }
    public string? WhitelistEntryId { get; }

    public static User Register(
        string username,
        EmailAddress email,
        string passwordHash,
        DateTime? createdAt = null,
        string? whitelistEntryId = null)
        => new(username, email, passwordHash, createdAt ?? DateTime.UtcNow, whitelistEntryId);

    public void ChangeUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new DomainException("O nome de usuário é obrigatório.");
        }

        Username = username.Trim();
    }

    public void ChangePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new DomainException("A senha é obrigatória.");
        }

        PasswordHash = passwordHash.Trim();
    }
}
