using Autodor.Shared.Contracts.Contractors.Commands;
using Autodor.Shared.Contracts.Contractors.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Autodor.Web.Endpoints;

public static class ContractorsEndpoints
{
    public static IEndpointRouteBuilder MapContractorsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/contractors")
            .WithTags("Contractors");

        group.MapPost("/", CreateContractor)
            .WithName("CreateContractor")
            .WithOpenApi();

        group.MapGet("/{id:guid}", GetContractor)
            .WithName("GetContractor")
            .WithOpenApi();

        group.MapGet("/", GetAllContractors)
            .WithName("GetAllContractors")
            .WithOpenApi();

        return endpoints;
    }

    private static async Task<IResult> CreateContractor(
        [FromBody] CreateContractorCommand command,
        IMediator mediator)
    {
        var id = await mediator.Send(command);
        return Results.Ok(id);
    }

    private static async Task<IResult> GetContractor(
        Guid id,
        IMediator mediator)
    {
        var contractor = await mediator.Send(new GetContractorQuery(id));
        return Results.Ok(contractor);
    }

    private static async Task<IResult> GetAllContractors(
        IMediator mediator)
    {
        var contractors = await mediator.Send(new GetAllContractorsQuery());
        return Results.Ok(contractors);
    }
}