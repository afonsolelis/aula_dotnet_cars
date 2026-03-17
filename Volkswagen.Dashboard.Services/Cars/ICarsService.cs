namespace Volkswagen.Dashboard.Services.Cars;

public interface ICarsService
{
    Task<CarDto?> GetCarByIdAsync(string id);
    Task<IReadOnlyCollection<CarDto>> GetCarsAsync();
    Task<string> CreateCarAsync(CreateCarInput input);
    Task<string> UpdateCarAsync(UpdateCarInput input);
    Task DeleteCarAsync(string id);
}
