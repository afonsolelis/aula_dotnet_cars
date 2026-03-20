namespace Volkswagen.Dashboard.Services.Security;

public interface IPasswordHasher
{
    string Hash(string input);
    bool Verify(string input, string hash);
}
