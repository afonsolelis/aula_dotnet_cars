using Volkswagen.Dashboard.Domain.Cars;

namespace Volkswagen.Dashboard.Services.Cars;

public sealed class CarDtoMapper : ICarDtoMapper
{
    public CarDto Map(Car car)
        => new(car.Id, car.Name, car.DateRelease);
}
