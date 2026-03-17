namespace Volkswagen.Dashboard.WebApi.Contracts;

public sealed class SaveCarRequest
{
    public string Name { get; set; } = string.Empty;
    public DateTime DateRelease { get; set; }
}
