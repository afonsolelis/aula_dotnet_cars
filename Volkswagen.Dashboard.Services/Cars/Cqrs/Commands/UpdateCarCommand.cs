using MediatR;

namespace Volkswagen.Dashboard.Services.Cars.Cqrs.Commands;

public sealed record UpdateCarCommand(string Id, string Name, DateTime DateRelease) : IRequest<string>;
