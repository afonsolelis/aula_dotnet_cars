using MediatR;
using Volkswagen.Dashboard.Services.Cars.Cqrs.Commands;
using Volkswagen.Dashboard.Repository;

namespace Volkswagen.Dashboard.Services.Cars.Cqrs.Handlers;

public sealed class DeleteCarCommandHandler : IRequestHandler<DeleteCarCommand>
{
    private readonly ICarsRepository _carsRepository;

    public DeleteCarCommandHandler(ICarsRepository carsRepository)
    {
        _carsRepository = carsRepository;
    }

    public async Task Handle(DeleteCarCommand request, CancellationToken cancellationToken)
    {
        await _carsRepository.DeleteCar(request.Id);
    }
}
