using MongoDB.Bson;
using MongoDB.Driver;
using Volkswagen.Dashboard.Domain.Cars;
using Volkswagen.Dashboard.Domain.Repositories;
using Volkswagen.Dashboard.Repository.Documents;

namespace Volkswagen.Dashboard.Repository;

public class CarsRepository : ICarRepository
{
    private readonly IMongoCollection<CarDocument> _cars;

    public CarsRepository(IMongoDatabase database)
    {
        _cars = database.GetCollection<CarDocument>("cars");
    }

    public async Task<IReadOnlyCollection<Car>> GetAllAsync()
    {
        var documents = await _cars.Find(FilterDefinition<CarDocument>.Empty)
            .SortBy(x => x.Name)
            .ToListAsync();

        return documents.Select(ToDomain).ToArray();
    }

    public async Task<Car?> GetByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return null;
        }

        var document = await _cars.Find(x => x.Id == id).FirstOrDefaultAsync();
        return document is null ? null : ToDomain(document);
    }

    public async Task<string> AddAsync(Car car)
    {
        var document = new CarDocument
        {
            Name = car.Name,
            DateRelease = car.DateRelease
        };

        await _cars.InsertOneAsync(document);
        return document.Id;
    }

    public async Task<bool> UpdateAsync(Car car)
    {
        if (!ObjectId.TryParse(car.Id, out _))
        {
            return false;
        }

        var result = await _cars.ReplaceOneAsync(x => x.Id == car.Id, ToDocument(car));
        return result.MatchedCount > 0;
    }

    public async Task DeleteAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return;
        }

        await _cars.DeleteOneAsync(x => x.Id == id);
    }

    private static Car ToDomain(CarDocument document)
        => Car.Restore(document.Id, document.Name, document.DateRelease);

    private static CarDocument ToDocument(Car car)
        => new()
        {
            Id = car.Id,
            Name = car.Name,
            DateRelease = car.DateRelease
        };
}
