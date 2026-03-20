using Volkswagen.Dashboard.Domain.Cars;
using Volkswagen.Dashboard.Domain.Repositories;

namespace Volkswagen.Dashboard.Services.Cars;

public class CarsService : ICarsService
{
    private readonly ICarRepository _carRepository;
    private readonly ICarDtoMapper _carDtoMapper;

    public CarsService(ICarRepository carRepository, ICarDtoMapper carDtoMapper)
    {
        _carRepository = carRepository;
        _carDtoMapper = carDtoMapper;
    }

    public async Task<CarDto?> GetCarByIdAsync(string id)
    {
        var car = await _carRepository.GetByIdAsync(id);
        return car is null ? null : _carDtoMapper.Map(car);
    }

    public async Task<IReadOnlyCollection<CarDto>> GetCarsAsync()
    {
        var cars = await _carRepository.GetAllAsync();
        return cars.Select(_carDtoMapper.Map).ToArray();
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
