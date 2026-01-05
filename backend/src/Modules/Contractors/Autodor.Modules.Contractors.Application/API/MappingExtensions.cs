using Autodor.Shared.Contracts.Contractors.Dtos;
using Autodor.Modules.Contractors.Application.Queries.GetContractor;
using Autodor.Modules.Contractors.Application.Queries.GetContractors;

namespace Autodor.Modules.Contractors.Application.API;

public static class MappingExtensions
{
    public static ContractorDto ToDto(this Domain.Aggregates.Contractor contractor)
    {
        return new ContractorDto(
            contractor.Id.Value,
            contractor.NIP.Value,
            contractor.Name,
            contractor.Address.Street,
            contractor.Address.City,
            contractor.Address.ZipCode,
            contractor.Email.Value
        );
    }

    public static GetContractorDto ToGetContractorDto(this Domain.Aggregates.Contractor contractor)
    {
        return new GetContractorDto(
            contractor.Id.Value,
            contractor.Name,
            contractor.NIP.Value,
            contractor.Address.Street,
            contractor.Address.City,
            contractor.Address.ZipCode,
            contractor.Email.Value
        );
    }

    public static GetContractorsDto ToGetContractorsDto(this Domain.Aggregates.Contractor contractor)
    {
        return new GetContractorsDto(
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