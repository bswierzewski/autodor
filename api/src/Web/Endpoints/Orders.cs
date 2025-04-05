using Application.Orders.Commands.ExcludeOrder;
using Application.Orders.Queries;
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
                .MapPost(ExcludeOrder);    
        }

        public async Task<IEnumerable<OrderDto>> GetOrders(ISender sender, DateTime dateFrom, DateTime dateTo)
        {
            return await sender.Send(new GetOrdersQuery
            {
                DateFrom = dateFrom,
                DateTo = dateTo
            });
        }

        public async Task ExcludeOrder(ISender sender, ExcludeOrderCommand command)
        {
            await sender.Send(command);
        }
    }
}
