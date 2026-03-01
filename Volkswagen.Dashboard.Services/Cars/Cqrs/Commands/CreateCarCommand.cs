using MediatR;

namespace Volkswagen.Dashboard.Services.Cars.Cqrs.Commands;

public sealed record CreateCarCommand(string Name, DateTime DateRelease) : IRequest<string>;
