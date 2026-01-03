using Application.Orders.Commands.ExcludeOrder;
using Application.Orders.Commands.ExcludeOrderPosition;
using Application.Orders.Queries;
using Application.Orders.Queries.GetOrderById;
using MediatR;
using Web.Infrastructure;

namespace Web.Endpoints
{
    public class Orders : EndpointGroupBase
    {
        public override void Map(WebApplication app)
        {
            app.MapGroup(this)
                .RequireAuthorization()
                .MapGet(GetOrders)
                .MapGet(GetOrderById, "{orderId}")
                .MapPost(ExcludeOrder, "exclude")
                .MapPost(ExcludeOrderPosition, "exclude-position");
        }

        public async Task<IEnumerable<OrderDto>> GetOrders(ISender sender, DateTime dateFrom, DateTime dateTo)
        {
            return await sender.Send(new GetOrdersQuery
            {
                DateFrom = dateFrom,
                DateTo = dateTo
            });
        }

        public async Task<OrderDto> GetOrderById(ISender sender, string orderId)
        {
            return await sender.Send(new GetOrderByIdQuery { OrderId = orderId });
        }

        public async Task ExcludeOrder(ISender sender, ExcludeOrderCommand command)
        {
            await sender.Send(command);
        }

        public async Task ExcludeOrderPosition(ISender sender, ExcludeOrderPositionCommand command)
        {
            await sender.Send(command);
        }
    }
}
