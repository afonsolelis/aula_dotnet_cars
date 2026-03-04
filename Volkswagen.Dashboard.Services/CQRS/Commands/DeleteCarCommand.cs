using MediatR;

namespace Volkswagen.Dashboard.Services.CQRS.Commands;

public record DeleteCarCommand(string Id) : IRequest;
