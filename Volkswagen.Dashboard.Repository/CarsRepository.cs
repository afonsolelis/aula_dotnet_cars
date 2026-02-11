using MongoDB.Bson;
using MongoDB.Driver;

namespace Volkswagen.Dashboard.Repository
{
    public class CarsRepository : ICarsRepository
    {
        private readonly IMongoCollection<CarModel> _cars;

        public CarsRepository(IMongoDatabase database)
        {
            _cars = database.GetCollection<CarModel>("cars");
        }

        public async Task<IEnumerable<CarModel>> GetCars()
        {
            return await _cars.Find(FilterDefinition<CarModel>.Empty)
                .SortBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<CarModel?> GetCarById(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return null;
            }

            return await _cars.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<string> InsertCar(CarModel carModel)
        {
            var document = new CarModel
            {
                Name = carModel.Name,
                DateRelease = DateTime.SpecifyKind(carModel.DateRelease, DateTimeKind.Utc)
            };

            await _cars.InsertOneAsync(document);
            return document.Id;
        }

        public async Task DeleteCar(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return;
            }

            await _cars.DeleteOneAsync(x => x.Id == id);
        }

        public async Task<string> UpdateCar(CarModel carModel)
        {
            if (!ObjectId.TryParse(carModel.Id, out _))
            {
                return string.Empty;
            }

            var updated = new CarModel
            {
                Id = carModel.Id,
                Name = carModel.Name,
                DateRelease = DateTime.SpecifyKind(carModel.DateRelease, DateTimeKind.Utc)
            };

            var result = await _cars.ReplaceOneAsync(x => x.Id == carModel.Id, updated);
            return result.MatchedCount > 0 ? carModel.Id : string.Empty;
        }
    }
}
