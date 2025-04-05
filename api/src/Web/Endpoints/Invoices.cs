using Application.Invoices.Commands.CreateInvoice;
using Application.Invoices.Commands.DTOs;
using Application.Invoices.Commands.PrintInvoice;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Web.Infrastructure;

namespace Web.Endpoints
{
    public class Invoices : EndpointGroupBase
    {
        public override void Map(WebApplication app)
        {
            app.MapGroup(this)
                .RequireAuthorization()
                .MapPost(PrintInvoice, "PrintInvoice")
                .MapPost(CreateInvoice);
        }

        public async Task<InvoiceResponseDto> CreateInvoice(ISender sender, CreateInvoiceCommand command)
        {
            return await sender.Send(command);
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
        public async Task<IResult> PrintInvoice(ISender sender, PrintInvoiceCommand command)
        {
            var fileResponse = await sender.Send(command);

            return Results.File(fileResponse.Content, fileResponse.ContentType, fileResponse.FileName);
        }
    }
}
