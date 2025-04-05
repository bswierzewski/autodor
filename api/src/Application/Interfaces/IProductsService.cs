using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IProductsService
{
    Task<IDictionary<string, Product>> GetProductsAsync();
}
