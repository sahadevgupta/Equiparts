using Equiparts.Models;
using Equiparts.Models.Catalog;

namespace Equiparts.Interfaces;

public interface IProductService
{
    Task<IEnumerable<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetProductsAsync(int? categoryId = null, CancellationToken cancellationToken = default);
    Task<PagedResult<Product>> GetProductsPagedAsync(int? categoryId = null, string? search = null, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<IEnumerable<Banner>> GetBannersAsync(CancellationToken cancellationToken = default);
}
