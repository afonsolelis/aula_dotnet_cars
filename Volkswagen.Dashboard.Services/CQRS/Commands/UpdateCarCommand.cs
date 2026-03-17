using MediatR;

namespace Volkswagen.Dashboard.Services.CQRS.Commands;

public record UpdateCarCommand(string Id, string Name, DateTime DateRelease) : IRequest<string>;
