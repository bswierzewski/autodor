using Autodor.Modules.Contractors.Application.Commands.CreateContractor;
using Autodor.Modules.Contractors.Application.Commands.UpdateContractor;
using Autodor.Modules.Contractors.Application.Commands.DeleteContractor;
using Autodor.Modules.Contractors.Application.Queries.GetAllContractors;
using Autodor.Modules.Contractors.Application.Queries.GetContractor;
using Autodor.Modules.Contractors.Application.Queries.GetContractorByNIP;
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

        group.MapGet("/{id:guid}", GetContractor)
            .WithName("GetContractor");

        group.MapGet("/", GetAllContractors)
            .WithName("GetAllContractors");

        group.MapPut("/{id:guid}", UpdateContractor)
            .WithName("UpdateContractor");

        group.MapDelete("/{id:guid}", DeleteContractor)
            .WithName("DeleteContractor");

        group.MapGet("/by-nip/{nip}", GetContractorByNIP)
            .WithName("GetContractorByNIP");

        return endpoints;
    }

    private static async Task<IResult> CreateContractor(
        [FromBody] CreateContractorCommand command,
        IMediator mediator)
    {
        var contractorId = await mediator.Send(command);

        return Results.Ok(contractorId);
    }

    private static async Task<IResult> GetContractor(
        Guid id,
        IMediator mediator)
    {
        try
        {
            var contractor = await mediator.Send(new GetContractorQuery(id));

            return Results.Ok(contractor);
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound();
        }
    }

    private static async Task<IResult> GetAllContractors(
        IMediator mediator)
    {
        var contractors = await mediator.Send(new GetAllContractorsQuery());

        return Results.Ok(contractors);
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

        try
        {
            await mediator.Send(command);

            return Results.NoContent();
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound();
        }
    }

    private static async Task<IResult> DeleteContractor(
        Guid id,
        IMediator mediator)
    {
        try
        {
            await mediator.Send(new DeleteContractorCommand(id));

            return Results.NoContent();
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound();
        }
    }

    private static async Task<IResult> GetContractorByNIP(
        string nip,
        IMediator mediator)
    {
        try
        {
            var contractor = await mediator.Send(new GetContractorByNIPQuery(nip));

            return Results.Ok(contractor);
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound();
        }
    }
}
