using Volkswagen.Dashboard.Domain.Common;

namespace Volkswagen.Dashboard.Domain.Cars;

public sealed class Car
{
    private Car(string id, string name, DateTime dateRelease)
    {
        Id = id;
        Rename(name);
        ChangeReleaseDate(dateRelease);
    }

    public string Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateTime DateRelease { get; private set; }

    public static Car Create(string name, DateTime dateRelease)
        => new(string.Empty, name, dateRelease);

    public static Car Restore(string id, string name, DateTime dateRelease)
        => new(id?.Trim() ?? string.Empty, name, dateRelease);

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("O nome do carro é obrigatório.");
        }

        Name = name.Trim();
    }

    public void ChangeReleaseDate(DateTime dateRelease)
    {
        if (dateRelease == default)
        {
            throw new DomainException("A data de lançamento é obrigatória.");
        }

        DateRelease = dateRelease.Kind == DateTimeKind.Utc
            ? dateRelease
            : DateTime.SpecifyKind(dateRelease, DateTimeKind.Utc);
    }

    public void DefineId(string id)
    {
        Id = id?.Trim() ?? string.Empty;
    }
}
