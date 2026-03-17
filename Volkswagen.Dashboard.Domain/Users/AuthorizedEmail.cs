namespace Volkswagen.Dashboard.Domain.Users;

public sealed class AuthorizedEmail
{
    public AuthorizedEmail(string id, EmailAddress email, DateTime createdAt)
    {
        Id = id;
        Email = email;
        CreatedAt = createdAt;
    }

    public string Id { get; }
    public EmailAddress Email { get; }
    public DateTime CreatedAt { get; }
}
