using System.Net.Mail;
using Volkswagen.Dashboard.Domain.Common;

namespace Volkswagen.Dashboard.Domain.Users;

public sealed class EmailAddress : IEquatable<EmailAddress>
{
    public EmailAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("O email é obrigatório.");
        }

        try
        {
            var parsed = new MailAddress(value.Trim());
            Value = parsed.Address.ToLowerInvariant();
        }
        catch (FormatException)
        {
            throw new DomainException("O email informado é inválido.");
        }
    }

    public string Value { get; }

    public bool Equals(EmailAddress? other)
        => other is not null && Value == other.Value;

    public override bool Equals(object? obj)
        => obj is EmailAddress other && Equals(other);

    public override int GetHashCode()
        => Value.GetHashCode(StringComparison.Ordinal);

    public override string ToString()
        => Value;
}
