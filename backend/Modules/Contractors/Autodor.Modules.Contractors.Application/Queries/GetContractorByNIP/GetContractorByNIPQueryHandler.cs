using Autodor.Modules.Contractors.Domain.Abstractions;
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Shared.Contracts.Contractors.Queries;
using MediatR;

namespace Autodor.Modules.Contractors.Application.Queries.GetContractorByNIP;

public class GetContractorByNIPQueryHandler : IRequestHandler<GetContractorByNIPQuery, GetContractorByNIPDto>
{
    private readonly IContractorRepository _repository;

    public GetContractorByNIPQueryHandler(IContractorRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetContractorByNIPDto> Handle(GetContractorByNIPQuery request, CancellationToken cancellationToken)
    {
        var contractor = await _repository.GetByNIPAsync(new TaxId(request.NIP));

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