using Equiparts.Models.Auth;
using Equiparts.Models.Catalog;
using Refit;

namespace Equiparts.Interfaces;

// Public catalog browsing endpoints: registered WITHOUT AuthHandler since banners,
// categories and products are visible to anonymous users.
public interface ICatalogApi
{
    [Get("/api/banners")]
    Task<ApiResult<List<BannerResponse>>> GetBannersAsync(CancellationToken cancellationToken = default);

    [Get("/api/categories")]
    Task<ApiResult<List<CategoryResponse>>> GetCategoriesAsync(CancellationToken cancellationToken = default);

    [Get("/api/products")]
    Task<ApiResult<PagedResult<ProductResponse>>> GetProductsAsync(
        [Query] int? categoryId = null,
        [Query] string? search = null,
        [Query] string? sort = null,
        [Query] int page = 1,
        [Query] int pageSize = 20,
        CancellationToken cancellationToken = default);
}
