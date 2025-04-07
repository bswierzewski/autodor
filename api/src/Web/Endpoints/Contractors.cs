using Application.Contractors.Commands.CreateContractor;
using Application.Contractors.Commands.DeleteContractor;
using Application.Contractors.Commands.UpdateContractor;
using Application.Contractors.Queries.GetContractor;
using Application.Contractors.Queries.GetContractors;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Web.Infrastructure;

namespace Web.Endpoints;

public class Contractors : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetContractor, "{id}")
            .MapGet(GetContractors)
            .MapPost(CreateContractor)
            .MapPut(UpdateContractor, "{id}")
            .MapDelete(DeleteContractor);
    }

    public async Task<Contractor> GetContractor(ISender sender, int id)
    {
        return await sender.Send(new GetContractorQuery { Id = id });
    }

    public async Task<IEnumerable<Contractor>> GetContractors(ISender sender)
    {
        return await sender.Send(new GetContractorsQuery());
    }

    public async Task<IResult> CreateContractor(ISender sender, CreateContractorCommand command)
    {
        await sender.Send(command);

        return Results.NoContent();
    }

    public async Task<IResult> UpdateContractor(ISender sender, int id, UpdateContractorCommand command)
    {
        if (id != command.Id) return Results.BadRequest();

        await sender.Send(command);

        return Results.NoContent();
    }

    public async Task<IResult> DeleteContractor(ISender sender, [FromBody] DeleteContractorCommand command)
    {
        await sender.Send(command);

        return Results.NoContent();
    }
}
