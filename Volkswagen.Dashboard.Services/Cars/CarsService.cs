using Volkswagen.Dashboard.Domain.Cars;
using Volkswagen.Dashboard.Domain.Repositories;

namespace Volkswagen.Dashboard.Services.Cars;

public class CarsService : ICarsService
{
    private readonly ICarRepository _carRepository;

    public CarsService(ICarRepository carRepository)
    {
        _carRepository = carRepository;
    }

    public async Task<CarDto?> GetCarByIdAsync(string id)
    {
        var car = await _carRepository.GetByIdAsync(id);
        return car is null ? null : CarDto.FromDomain(car);
    }

    public async Task<IReadOnlyCollection<CarDto>> GetCarsAsync()
    {
        var cars = await _carRepository.GetAllAsync();
        return cars.Select(CarDto.FromDomain).ToArray();
    }

    public Task<string> CreateCarAsync(CreateCarInput input)
        => _carRepository.AddAsync(Car.Create(input.Name, input.DateRelease));

    public async Task<string> UpdateCarAsync(UpdateCarInput input)
    {
        var car = Car.Restore(input.Id, input.Name, input.DateRelease);
        var updated = await _carRepository.UpdateAsync(car);
        return updated ? car.Id : string.Empty;
    }

    public Task DeleteCarAsync(string id)
        => _carRepository.DeleteAsync(id);
}
