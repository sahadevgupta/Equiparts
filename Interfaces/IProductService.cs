using Equiparts.Models;

namespace Equiparts.Interfaces;

public interface IProductService
{
    Task<List<Category>> GetCategoriesAsync();
    Task<List<Order>> GetOrdersAsync();
    Task<List<Product>> GetProductsAsync();
}
