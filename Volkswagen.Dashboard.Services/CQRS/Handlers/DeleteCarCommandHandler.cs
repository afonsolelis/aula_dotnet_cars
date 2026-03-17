using MediatR;
using Volkswagen.Dashboard.Services.Cars;
using Volkswagen.Dashboard.Services.CQRS.Commands;

namespace Volkswagen.Dashboard.Services.CQRS.Handlers;

public class DeleteCarCommandHandler : IRequestHandler<DeleteCarCommand>
{
    private readonly ICarsService _carsService;

    public DeleteCarCommandHandler(ICarsService carsService)
    {
        _carsService = carsService;
    }

    public Task Handle(DeleteCarCommand request, CancellationToken cancellationToken)
        => _carsService.DeleteCarAsync(request.Id);
}
