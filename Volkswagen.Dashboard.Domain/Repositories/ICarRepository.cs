using Volkswagen.Dashboard.Domain.Cars;

namespace Volkswagen.Dashboard.Domain.Repositories;

public interface ICarRepository
{
    Task<IReadOnlyCollection<Car>> GetAllAsync();
    Task<Car?> GetByIdAsync(string id);
    Task<string> AddAsync(Car car);
    Task<bool> UpdateAsync(Car car);
    Task DeleteAsync(string id);
}
