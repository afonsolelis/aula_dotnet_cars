using MediatR;
using Volkswagen.Dashboard.Repository;
using Volkswagen.Dashboard.Services.Cars.Cqrs.Queries;

namespace Volkswagen.Dashboard.Services.Cars.Cqrs.Handlers;

public sealed class GetCarsQueryHandler : IRequestHandler<GetCarsQuery, IEnumerable<CarModel>>
{
    private readonly ICarsRepository _carsRepository;

    public GetCarsQueryHandler(ICarsRepository carsRepository)
    {
        _carsRepository = carsRepository;
    }

    public async Task<IEnumerable<CarModel>> Handle(GetCarsQuery request, CancellationToken cancellationToken)
    {
        return await _carsRepository.GetCars();
    }
}
