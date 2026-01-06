using Autodor.Modules.Contractors.Application.Commands.CreateContractor;
using Autodor.Modules.Contractors.Application.Commands.UpdateContractor;
using Autodor.Modules.Contractors.Application.Commands.DeleteContractor;
using Autodor.Modules.Contractors.Application.Queries.GetContractors;
using Autodor.Modules.Contractors.Application.Queries.GetContractor;
using BuildingBlocks.Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Autodor.Modules.Contractors.Infrastructure.Endpoints;

public static class ContractorsEndpoints
{
    public static IEndpointRouteBuilder MapContractorsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/contractors")
            .RequireAuthorization()
            .WithTags("Contractors");

        group.MapPost("/", CreateContractor)
            .WithName("CreateContractor");

        group.MapGet("/", GetContractors)
            .WithName("GetContractors");

        group.MapGet("/{id:guid}", GetContractor)
            .WithName("GetContractor");

        group.MapPut("/{id:guid}", UpdateContractor)
            .WithName("UpdateContractor");

        group.MapDelete("/{id:guid}", DeleteContractor)
            .WithName("DeleteContractor");

        return endpoints;
    }

    private static async Task<IResult> CreateContractor(
        [FromBody] CreateContractorCommand command,
        IMediator mediator)
    {
        var result = await mediator.Send(command);

        return result.Match(
            value => Results.Ok(value),
            errors => errors.Problem());
    }

    private static async Task<IResult> GetContractor(
        Guid id,
        [FromQuery] string? nip,
        IMediator mediator)
    {
        // Jeśli podany jest NIP, szukaj po NIP zamiast ID
        var query = !string.IsNullOrEmpty(nip)
            ? new GetContractorQuery(NIP: nip)
            : new GetContractorQuery(Id: id);

        var result = await mediator.Send(query);
        return result.Match(
            value => Results.Ok(value),
            errors => errors.Problem());
    }

    private static async Task<IResult> GetContractors(
        IMediator mediator)
    {
        var result = await mediator.Send(new GetContractorsQuery());
        return result.Match(
            value => Results.Ok(value),
            errors => errors.Problem());
    }

    private static async Task<IResult> UpdateContractor(
        Guid id,
        [FromBody] UpdateContractorCommand command,
        IMediator mediator)
    {
        if (id != command.Id)
        {
            return Results.BadRequest("Route ID does not match command ID");
        }

        var result = await mediator.Send(command);
        return result.Match(
            value => Results.NoContent(),
            errors => errors.Problem());
    }

    private static async Task<IResult> DeleteContractor(
        Guid id,
        IMediator mediator)
    {
        var result = await mediator.Send(new DeleteContractorCommand(id));
        return result.Match(
            value => Results.NoContent(),
            errors => errors.Problem());
    }
}
