using MediatR;

namespace Autodor.Shared.Contracts.Contractors.Commands;

public record UpdateContractorCommand(
    Guid Id,
    string Name,
    string Street,
    string City,
    string ZipCode,
    string Email
) : IRequest;