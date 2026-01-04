using MediatR;

namespace Autodor.Modules.Contractors.Application.Commands.CreateContractor;

public record CreateContractorCommand(
    string Name,
    string NIP,
    string Street,
    string City,
    string ZipCode,
    string Email
) : IRequest<Guid>;