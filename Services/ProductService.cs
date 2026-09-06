using System.Net;
using System.Text.Json;
using Equiparts.Configuration.Mapper;
using Equiparts.Interfaces;
using Equiparts.Models;
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

    public async Task<IEnumerable<Product>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await connectivityService.CheckInternetAccessAsync();
            var response = await catalogApi.GetProductsAsync(cancellationToken: cancellationToken);
            return Enumerable.Empty<Product>();
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