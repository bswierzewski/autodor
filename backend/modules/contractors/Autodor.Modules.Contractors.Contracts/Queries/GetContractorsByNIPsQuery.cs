namespace Autodor.Modules.Contractors.Contracts.Queries;

public record GetContractorsByNIPsQuery(IEnumerable<string> NIPs);
