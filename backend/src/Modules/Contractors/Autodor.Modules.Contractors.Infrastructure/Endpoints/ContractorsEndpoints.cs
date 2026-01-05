using Autodor.Modules.Contractors.Application.Commands.CreateContractor;
using Autodor.Modules.Contractors.Application.Commands.UpdateContractor;
using Autodor.Modules.Contractors.Application.Commands.DeleteContractor;
using Autodor.Modules.Contractors.Application.Queries.GetContractors;
using Autodor.Modules.Contractors.Application.Queries.GetContractor;
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
        var contractorId = await mediator.Send(command);

        return Results.Ok(contractorId);
    }

    private static async Task<IResult> GetContractor(
        Guid id,
        [FromQuery] string? nip,
        IMediator mediator)
    {
        try
        {
            // Jeśli podany jest NIP, szukaj po NIP zamiast ID
            if (!string.IsNullOrEmpty(nip))
            {
                var contractor = await mediator.Send(new GetContractorQuery(NIP: nip));
                return Results.Ok(contractor);
            }

            // W przeciwnym razie szukaj po ID
            var contractorById = await mediator.Send(new GetContractorQuery(Id: id));
            return Results.Ok(contractorById);
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound();
        }
        catch (ArgumentException)
        {
            return Results.BadRequest("Either use route parameter {id} or provide 'nip' query parameter");
        }
    }

    private static async Task<IResult> GetContractors(
        IMediator mediator)
    {
        var contractors = await mediator.Send(new GetContractorsQuery());
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
}
