using Autodor.Modules.Contractors.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Application.Queries.GetAllContractors;

/// <summary>
/// Handles retrieval of all contractors by processing GetAllContractorsQuery requests.
/// </summary>
public class GetAllContractorsQueryHandler : IRequestHandler<GetAllContractorsQuery, IEnumerable<GetAllContractorsDto>>
{
    private readonly IContractorsReadDbContext _readDbContext;

    public GetAllContractorsQueryHandler(IContractorsReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    /// <summary>
    /// Retrieves all contractors and maps them to DTOs for client consumption.
    /// </summary>
    /// <param name="request">Query request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of contractor DTOs.</returns>
    public async Task<IEnumerable<GetAllContractorsDto>> Handle(GetAllContractorsQuery request, CancellationToken cancellationToken)
    {
        var contractors = await _readDbContext.Contractors.ToListAsync(cancellationToken);

        return contractors.Select(contractor => new GetAllContractorsDto(
            contractor.Id.Value,
            contractor.Name,
            contractor.NIP.Value,
            contractor.Address.Street,
            contractor.Address.City,
            contractor.Address.ZipCode,
            contractor.Email.Value
        ));
    }
}