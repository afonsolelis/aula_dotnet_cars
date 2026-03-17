namespace Volkswagen.Dashboard.Services.Auth;

public class LoginResponse
{
    public DateTime CreatedAt { get; internal set; }
    public DateTime ExpiresAt { get; internal set; }
    public string AccessToken { get; internal set; } = string.Empty;
}
