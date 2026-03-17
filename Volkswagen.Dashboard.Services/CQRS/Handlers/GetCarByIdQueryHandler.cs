using MediatR;
using Volkswagen.Dashboard.Services.Cars;
using Volkswagen.Dashboard.Services.CQRS.Queries;

namespace Volkswagen.Dashboard.Services.CQRS.Handlers;

public class GetCarByIdQueryHandler : IRequestHandler<GetCarByIdQuery, CarDto?>
{
    private readonly ICarsService _carsService;

    public GetCarByIdQueryHandler(ICarsService carsService)
    {
        _carsService = carsService;
    }

    public Task<CarDto?> Handle(GetCarByIdQuery request, CancellationToken cancellationToken)
        => _carsService.GetCarByIdAsync(request.Id);
}
