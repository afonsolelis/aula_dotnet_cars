using MediatR;
using Volkswagen.Dashboard.Services.Cars;
using Volkswagen.Dashboard.Services.CQRS.Queries;

namespace Volkswagen.Dashboard.Services.CQRS.Handlers;

public class GetCarsQueryHandler : IRequestHandler<GetCarsQuery, IReadOnlyCollection<CarDto>>
{
    private readonly ICarsService _carsService;

    public GetCarsQueryHandler(ICarsService carsService)
    {
        _carsService = carsService;
    }

    public Task<IReadOnlyCollection<CarDto>> Handle(GetCarsQuery request, CancellationToken cancellationToken)
        => _carsService.GetCarsAsync();
}
