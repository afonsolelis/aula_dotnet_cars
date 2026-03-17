using System.Security.Cryptography;
using System.Text;

namespace Volkswagen.Dashboard.Services.Security;

public sealed class Md5PasswordHasher : IPasswordHasher
{
    public string Hash(string input)
    {
        using var md5Hash = MD5.Create();
        var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input ?? string.Empty));
        var builder = new StringBuilder();

        foreach (var value in data)
        {
            builder.Append(value.ToString("x2"));
        }

        return builder.ToString();
    }
}
