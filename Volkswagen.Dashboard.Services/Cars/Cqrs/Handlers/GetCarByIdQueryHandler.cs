using MediatR;
using Volkswagen.Dashboard.Repository;
using Volkswagen.Dashboard.Services.Cars.Cqrs.Queries;

namespace Volkswagen.Dashboard.Services.Cars.Cqrs.Handlers;

public sealed class GetCarByIdQueryHandler : IRequestHandler<GetCarByIdQuery, CarModel?>
{
    private readonly ICarsRepository _carsRepository;

    public GetCarByIdQueryHandler(ICarsRepository carsRepository)
    {
        _carsRepository = carsRepository;
    }

    public async Task<CarModel?> Handle(GetCarByIdQuery request, CancellationToken cancellationToken)
    {
        return await _carsRepository.GetCarById(request.Id);
    }
}
