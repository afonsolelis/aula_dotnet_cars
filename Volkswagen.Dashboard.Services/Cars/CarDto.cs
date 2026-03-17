using Volkswagen.Dashboard.Domain.Cars;

namespace Volkswagen.Dashboard.Services.Cars;

public sealed record CarDto(string Id, string Name, DateTime DateRelease)
{
    public static CarDto FromDomain(Car car)
        => new(car.Id, car.Name, car.DateRelease);
}
