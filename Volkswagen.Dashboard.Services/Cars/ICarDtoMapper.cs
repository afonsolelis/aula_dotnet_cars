using Volkswagen.Dashboard.Domain.Cars;

namespace Volkswagen.Dashboard.Services.Cars;

public interface ICarDtoMapper
{
    CarDto Map(Car car);
}
