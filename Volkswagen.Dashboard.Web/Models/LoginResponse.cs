namespace Volkswagen.Dashboard.Web.Models;

public class LoginResponse
{
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string AccessToken { get; set; } = string.Empty;
}
