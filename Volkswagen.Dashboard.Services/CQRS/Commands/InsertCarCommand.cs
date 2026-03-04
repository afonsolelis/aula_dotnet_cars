using MediatR;
using Volkswagen.Dashboard.Repository;

namespace Volkswagen.Dashboard.Services.CQRS.Commands;

public record InsertCarCommand(CarModel CarModel) : IRequest<string>;
