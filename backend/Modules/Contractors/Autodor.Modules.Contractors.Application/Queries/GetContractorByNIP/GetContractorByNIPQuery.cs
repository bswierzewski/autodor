using MediatR;

namespace Autodor.Modules.Contractors.Application.Queries.GetContractorByNIP;

public record GetContractorByNIPDto(
    Guid Id,
    string Name,
    string NIP,
    string Street,
    string City,
    string ZipCode,
    string Email
);

public record GetContractorByNIPQuery(string NIP) : IRequest<GetContractorByNIPDto>;