using MediatR;

namespace Volkswagen.Dashboard.Services.CQRS.Commands;

public record InsertCarCommand(string Name, DateTime DateRelease) : IRequest<string>;
