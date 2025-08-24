using MediatR;

namespace Autodor.Shared.Contracts.Contractors.Commands;

public record CreateContractorCommand(
    string Name,
    string NIP,
    string Street,
    string City,
    string ZipCode,
    string Email
) : IRequest<Guid>;