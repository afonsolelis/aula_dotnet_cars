using MediatR;
using Volkswagen.Dashboard.Repository;
using Volkswagen.Dashboard.Services.Cars;
using Volkswagen.Dashboard.Services.CQRS.Queries;

namespace Volkswagen.Dashboard.Services.CQRS.Handlers;

public class GetCarsQueryHandler : IRequestHandler<GetCarsQuery, IEnumerable<CarModel>>
{
    private readonly ICarsService _carsService;

    public GetCarsQueryHandler(ICarsService carsService)
    {
        _carsService = carsService;
    }

    public Task<IEnumerable<CarModel>> Handle(GetCarsQuery request, CancellationToken cancellationToken)
        => _carsService.GetCars();
}
