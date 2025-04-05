using AutoMapper;
using Domain.Entities;
using PolcarDistributorsSalesClient;

namespace Infrastructure.Services.Polcar.Mappings;

public class Mapping : Profile
{
    public Mapping()
    {
        // Order
        CreateMap<DistributorSalesOrderResponse, Order>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.EntryDate))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.OrderID))
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.PolcarOrderNumber))
            .ForMember(dest => dest.Person, opt => opt.MapFrom(src => src.OrderingPerson))
            .ForMember(dest => dest.CustomerNumber, opt => opt.MapFrom(src => src.CustomerNumber))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderedItemsResponse));

        // Order Item
        CreateMap<DistributorSalesOrderItemResponse, OrderItem>()
            .ForMember(dest => dest.PartNumber, opt => opt.MapFrom(src => src.PartNumber))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.QuantityOrdered))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Price));
    }
}
