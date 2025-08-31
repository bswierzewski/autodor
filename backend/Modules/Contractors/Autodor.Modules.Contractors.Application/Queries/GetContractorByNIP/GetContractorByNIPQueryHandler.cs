using Autodor.Modules.Contractors.Domain.Aggregates;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using MediatR;
using SharedKernel.Domain.Abstractions;

namespace Autodor.Modules.Contractors.Application.Queries.GetContractorByNIP;

public class GetContractorByNIPQueryHandler : IRequestHandler<GetContractorByNIPQuery, GetContractorByNIPDto>
{
    private readonly IRepository<Contractor> _repository;

    public GetContractorByNIPQueryHandler(IRepository<Contractor> repository)
    {
        _repository = repository;
    }

    public async Task<GetContractorByNIPDto> Handle(GetContractorByNIPQuery request, CancellationToken cancellationToken)
    {
        var contractor = await _repository.FirstOrDefaultAsync(x => x.NIP == new TaxId(request.NIP));

        if (contractor is null)
            throw new InvalidOperationException($"Contractor with NIP {request.NIP} not found");

        return new GetContractorByNIPDto(
            contractor.Id.Value,
            contractor.Name,
            contractor.NIP.Value,
            contractor.Address.Street,
            contractor.Address.City,
            contractor.Address.ZipCode,
            contractor.Email.Value
        );
    }
}