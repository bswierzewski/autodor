using MediatR;

namespace Autodor.Modules.Contractors.Application.Commands.DeleteContractor;

public record DeleteContractorCommand(Guid Id) : IRequest;