using MediatR;

namespace Autodor.Modules.Contractors.Application.Queries.GetContractors;

public record GetContractorsQuery() : IRequest<IEnumerable<GetContractorsDto>>;

public record GetContractorsDto(
    Guid Id,
    string Name,
    string NIP,
    string Street,
    string City,
    string ZipCode,
    string Email
);