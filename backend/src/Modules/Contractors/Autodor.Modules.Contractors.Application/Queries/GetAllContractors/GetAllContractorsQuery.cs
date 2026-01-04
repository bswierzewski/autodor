using MediatR;

namespace Autodor.Modules.Contractors.Application.Queries.GetAllContractors;

public record GetAllContractorsQuery() : IRequest<IEnumerable<GetAllContractorsDto>>;

public record GetAllContractorsDto(
    Guid Id,
    string Name,
    string NIP,
    string Street,
    string City,
    string ZipCode,
    string Email
);