using MediatR;

namespace Autodor.Shared.Contracts.Contractors.Commands;

public record DeleteContractorCommand(Guid Id) : IRequest;