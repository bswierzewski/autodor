using Autodor.Modules.Contractors.Contracts.Models;

namespace Autodor.Modules.Contractors.Contracts.Abstractions;

/// <summary>
/// Public API for Contractors module - for inter-module communication
/// </summary>
public interface IContractorsModuleApi
{
    /// <summary>
    /// Gets contractor by NIP
    /// </summary>
    Task<ContractorDto?> GetContractorByNipAsync(string nip, CancellationToken ct = default);
}
