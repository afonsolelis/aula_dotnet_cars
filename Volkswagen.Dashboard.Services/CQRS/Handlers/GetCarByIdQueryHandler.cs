using MediatR;
using Volkswagen.Dashboard.Repository;
using Volkswagen.Dashboard.Services.Cars;
using Volkswagen.Dashboard.Services.CQRS.Queries;

namespace Volkswagen.Dashboard.Services.CQRS.Handlers;

public class GetCarByIdQueryHandler : IRequestHandler<GetCarByIdQuery, CarModel?>
{
    private readonly ICarsService _carsService;

    public GetCarByIdQueryHandler(ICarsService carsService)
    {
        _carsService = carsService;
    }

    public Task<CarModel?> Handle(GetCarByIdQuery request, CancellationToken cancellationToken)
        => _carsService.GetCarById(request.Id);
}
