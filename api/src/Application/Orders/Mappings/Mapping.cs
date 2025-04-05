using Application.Orders.Queries;
using AutoMapper;
using Domain.Entities;

namespace Application.Orders.Mappings
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Order, OrderDto>();

            CreateMap<OrderItem, OrderItemDto>();
        }
    }
}
