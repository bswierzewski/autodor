using Domain.Entities;

namespace Application.Interfaces
{
    public interface IHtmlGeneratorService
    {
        string Generate(Contractor contractor, Order order);
    }
}
