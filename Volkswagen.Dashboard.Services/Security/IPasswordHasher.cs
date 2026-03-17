namespace Volkswagen.Dashboard.Services.Security;

public interface IPasswordHasher
{
    string Hash(string input);
}
