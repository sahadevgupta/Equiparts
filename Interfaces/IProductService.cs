using Equiparts.Models;

namespace Equiparts.Interfaces;

public interface IProductService
{
    Task<IEnumerable<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetProductsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Banner>> GetBannersAsync(CancellationToken cancellationToken = default);
}
