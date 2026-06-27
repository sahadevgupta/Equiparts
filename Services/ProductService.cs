using System.Text.Json;
using Equiparts.Interfaces;
using Equiparts.Models;

namespace Equiparts.Services;


public class ProductService : IProductService
{
    public async Task<List<Category>> GetCategoriesAsync()
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync("categories.json");

        using var reader = new StreamReader(stream);

        var json = await reader.ReadToEndAsync();

        return JsonSerializer.Deserialize<List<Category>>(json)!;
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync("products.json");

        using var reader = new StreamReader(stream);

        var json = await reader.ReadToEndAsync();

        return JsonSerializer.Deserialize<List<Product>>(json)!;
    }

    public async Task<List<Order>> GetOrdersAsync()
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync("orders.json");

        using var reader = new StreamReader(stream);

        var json = await reader.ReadToEndAsync();

        return JsonSerializer.Deserialize<List<Order>>(json)!;
    }
}