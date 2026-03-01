using MongoDB.Bson;
using MongoDB.Driver;

namespace Volkswagen.Dashboard.Repository
{
    public class CarsRepository : ICarsRepository
    {
        private readonly IMongoCollection<CarModel> _cars;
        private readonly object _circuitLock = new();
        private DateTimeOffset? _circuitOpenedAt;
        private static readonly TimeSpan CircuitBreakDuration = TimeSpan.FromSeconds(10);

        public CarsRepository(IMongoDatabase database)
        {
            _cars = database.GetCollection<CarModel>("cars");
        }

        public async Task<IEnumerable<CarModel>> GetCars()
        {
            return await ExecuteWithResilienceAsync(
                operationName: "listar carros",
                operation: async () => await _cars.Find(FilterDefinition<CarModel>.Empty)
                    .SortBy(x => x.Name)
                    .ToListAsync());
        }

        public async Task<CarModel?> GetCarById(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return null;
            }

            return await ExecuteWithResilienceAsync(
                operationName: "buscar carro por id",
                operation: async () => await _cars.Find(x => x.Id == id).FirstOrDefaultAsync());
        }

        public async Task<string> InsertCar(CarModel carModel)
        {
            var document = new CarModel
            {
                Name = carModel.Name,
                DateRelease = DateTime.SpecifyKind(carModel.DateRelease, DateTimeKind.Utc)
            };

            return await ExecuteWithResilienceAsync(
                operationName: "inserir carro",
                operation: async () =>
                {
                    await _cars.InsertOneAsync(document);
                    return document.Id;
                });
        }

        public async Task DeleteCar(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return;
            }

            await ExecuteWithResilienceAsync(
                operationName: "deletar carro",
                operation: async () =>
                {
                    await _cars.DeleteOneAsync(x => x.Id == id);
                    return true;
                });
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

            return await ExecuteWithResilienceAsync(
                operationName: "atualizar carro",
                operation: async () =>
                {
                    var result = await _cars.ReplaceOneAsync(x => x.Id == carModel.Id, updated);
                    return result.MatchedCount > 0 ? carModel.Id : string.Empty;
                });
        }

        private async Task<T> ExecuteWithResilienceAsync<T>(string operationName, Func<Task<T>> operation)
        {
            EnsureCircuitClosed();

            try
            {
                var result = await operation();
                CloseCircuit();
                return result;
            }
            catch (Exception ex) when (IsTransient(ex))
            {
                OpenCircuit();
                throw new RepositoryUnavailableException($"MongoDB indisponivel ao tentar {operationName}.", ex);
            }
        }

        private void EnsureCircuitClosed()
        {
            lock (_circuitLock)
            {
                if (_circuitOpenedAt is null)
                {
                    return;
                }

                var elapsed = DateTimeOffset.UtcNow - _circuitOpenedAt.Value;

                if (elapsed >= CircuitBreakDuration)
                {
                    _circuitOpenedAt = null;
                    return;
                }

                throw new RepositoryUnavailableException("Circuit breaker aberto para o MongoDB. Tente novamente em instantes.");
            }
        }

        private void OpenCircuit()
        {
            lock (_circuitLock)
            {
                _circuitOpenedAt = DateTimeOffset.UtcNow;
            }
        }

        private void CloseCircuit()
        {
            lock (_circuitLock)
            {
                _circuitOpenedAt = null;
            }
        }

        private static bool IsTransient(Exception ex)
        {
            return ex is MongoConnectionException
                or MongoExecutionTimeoutException
                or TimeoutException;
        }
    }
}
