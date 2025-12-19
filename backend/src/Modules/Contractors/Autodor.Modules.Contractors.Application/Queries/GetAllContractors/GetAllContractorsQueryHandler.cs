using Autodor.Modules.Contractors.Application.Abstractions;
using Autodor.Modules.Contractors.Application.API;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Application.Queries.GetAllContractors;

/// <summary>
/// Handles retrieval of all contractors by processing GetAllContractorsQuery requests.
/// </summary>
public class GetAllContractorsQueryHandler : IRequestHandler<GetAllContractorsQuery, IEnumerable<GetAllContractorsDto>>
{
    private readonly IContractorsDbContext _dbContext;

    public GetAllContractorsQueryHandler(IContractorsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Retrieves all contractors and maps them to DTOs for client consumption.
    /// </summary>
    /// <param name="request">Query request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of contractor DTOs.</returns>
    public async Task<IEnumerable<GetAllContractorsDto>> Handle(GetAllContractorsQuery request, CancellationToken cancellationToken)
    {
        var contractors = await _dbContext.Contractors
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return contractors.Select(c => c.ToGetAllContractorsDto());
    }
}