using System.Net;
using System.Text.Json;
using Equiparts.Configuration.Mapper;
using Equiparts.Interfaces;
using Equiparts.Models;
using Equiparts.Models.Catalog;
using Microsoft.Extensions.Logging;
using Refit;

namespace Equiparts.Services;


public class ProductService(ICatalogApi catalogApi,
    IConnectivityService connectivityService,
    ILogger<AuthenticationService> logger) : IProductService
{
    public async Task<IEnumerable<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await connectivityService.CheckInternetAccessAsync();
            var response = await catalogApi.GetCategoriesAsync(cancellationToken);

            if (response is not { Success: true, Data: not null })
                return Enumerable.Empty<Category>();

            return BackendToAppModelMapper.GetCategories(response.Data);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (ApiException apiEx)
        {
            logger.LogWarning(apiEx, "Login rejected by server ({StatusCode}).", apiEx.StatusCode);
            var message = apiEx.StatusCode == HttpStatusCode.Unauthorized
                ? "Invalid email or password."
                : "Login failed. Please try again.";
            return Enumerable.Empty<Category>();
        }
    }

    public async Task<IEnumerable<Product>> GetProductsAsync(int? categoryId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            await connectivityService.CheckInternetAccessAsync();
            var response = await catalogApi.GetProductsAsync(categoryId: categoryId, cancellationToken: cancellationToken);

            if (response is not { Success: true, Data: not null })
                return Enumerable.Empty<Product>();

            return BackendToAppModelMapper.GetProducts(response.Data.Items);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (ApiException apiEx)
        {
            logger.LogWarning(apiEx, "Login rejected by server ({StatusCode}).", apiEx.StatusCode);
            var message = apiEx.StatusCode == HttpStatusCode.Unauthorized
                ? "Invalid email or password."
                : "Login failed. Please try again.";
            return Enumerable.Empty<Product>();
        }
    }

    public async Task<PagedResult<Product>> GetProductsPagedAsync(int? categoryId = null, string? search = null, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            await connectivityService.CheckInternetAccessAsync();
            var response = await catalogApi.GetProductsAsync(
                categoryId: categoryId,
                search: search,
                page: page,
                pageSize: pageSize,
                cancellationToken: cancellationToken);

            if (response is not { Success: true, Data: not null })
                return new PagedResult<Product>{ PageNumber = page, PageSize = pageSize };

            return new PagedResult<Product>
            {
                Items = BackendToAppModelMapper.GetProducts(response.Data.Items),
                PageNumber = response.Data.PageNumber,
                PageSize = response.Data.PageSize,
                TotalCount = response.Data.TotalCount,
                TotalPages = response.Data.TotalPages,
                HasNext = response.Data.HasNext,
                HasPrevious = response.Data.HasPrevious
            };
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (ApiException apiEx)
        {
            logger.LogWarning(apiEx, "Product fetch rejected by server ({StatusCode}).", apiEx.StatusCode);
            return new PagedResult<Product> { PageNumber = page, PageSize = pageSize };
        }
    }

    public async Task<IEnumerable<Banner>> GetBannersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await connectivityService.CheckInternetAccessAsync();
            var response = await catalogApi.GetBannersAsync(cancellationToken: cancellationToken);

            if (response is not { Success: true, Data: not null })
                return Enumerable.Empty<Banner>();

            return BackendToAppModelMapper.GetBanners(response.Data);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (ApiException apiEx)
        {
            logger.LogWarning(apiEx, "Login rejected by server ({StatusCode}).", apiEx.StatusCode);
            var message = apiEx.StatusCode == HttpStatusCode.Unauthorized
                ? "Invalid email or password."
                : "Login failed. Please try again.";
            return Enumerable.Empty<Banner>();
        }
    }
}