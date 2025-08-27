using MediatR;

namespace Autodor.Modules.Contractors.Application.Commands.UpdateContractor;

public record UpdateContractorCommand(
    Guid Id,
    string Name,
    string Street,
    string City,
    string ZipCode,
    string Email
) : IRequest;