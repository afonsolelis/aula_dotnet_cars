using MediatR;
using Volkswagen.Dashboard.Repository;

namespace Volkswagen.Dashboard.Services.CQRS.Commands;

public record UpdateCarCommand(CarModel CarModel) : IRequest<string>;
