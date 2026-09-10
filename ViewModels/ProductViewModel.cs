using System.Collections.ObjectModel;
using System.Diagnostics;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Equiparts.Enums;
using Equiparts.Interfaces;
using Equiparts.Models;
using Equiparts.Services;
using Equiparts.Views;

namespace Equiparts.ViewModels;

public partial class ProductViewModel : BaseViewModel, IQueryAttributable
{
    const int PageSize = 10;

    readonly IProductService _productService;
    readonly ICartService _cartService;
    readonly INavigationService _navigationService;
    readonly IDialogService _dialogService;

    int? _categoryId;
    int _currentPage;
    CancellationTokenSource? _loadCts;

    public ObservableCollection<Product> Products { get; } = [];

    [ObservableProperty]
    private string? searchText;

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private bool isLoadingMore;

    [ObservableProperty]
    private bool hasMoreData = true;

    [ObservableProperty]
    private bool hasError;

    [ObservableProperty]
    private string? debugInfo;

    public ProductViewModel(IProductService productService, ICartService cartService, INavigationService navigationService, IDialogService dialogService)
    {
        Title = "Products";

        _productService = productService;
        _cartService = cartService;
        _navigationService = navigationService;
        _dialogService = dialogService;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("CategoryId", out var categoryIdValue) && categoryIdValue is int categoryId)
            _categoryId = categoryId;

        if (query.TryGetValue("Title", out var titleValue) && titleValue is string title)
            Title = title;
    }

    [RelayCommand]
    async Task BulkOrder()
    {
        await _dialogService.ShowAlertAsync(
            "Our sales team will reach out to help with your bulk order or request for quotation.",
            AlertType.Success);
    }

    [RelayCommand]
    async Task GoBack()
    {
        await _navigationService.GoBackAsync();
    }

    [RelayCommand]
    async Task Load()
    {
        if (IsBusy)
            return;

        IsBusy = true;

        try
        {
            await LoadPageAsync(resetPagination: true);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task Refresh()
    {
        IsRefreshing = true;

        try
        {
            await LoadPageAsync(resetPagination: true);
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    async Task LoadMore()
    {
        if (IsBusy || IsRefreshing || IsLoadingMore || !HasMoreData)
            return;

        IsLoadingMore = true;

        try
        {
            await LoadPageAsync(resetPagination: false);
        }
        finally
        {
            IsLoadingMore = false;
        }
    }

    [RelayCommand]
    async Task Search(string? text)
    {
        DebugInfo = $"Search invoked: '{text}' @ {DateTime.Now:HH:mm:ss.fff}";
        SearchText = text;

        await LoadPageAsync(resetPagination: true);
    }

    [RelayCommand]
    async Task ClearSearch()
    {
        if (string.IsNullOrEmpty(SearchText))
            return;

        SearchText = string.Empty;

        await LoadPageAsync(resetPagination: true);
    }

    async Task LoadPageAsync(bool resetPagination)
    {
        if (!resetPagination && !HasMoreData)
            return;

        // Cancel any in-flight request (a stale search/page fetch) so its results
        // can never land after and overwrite what this newer request produces.
        _loadCts?.Cancel();
        _loadCts?.Dispose();
        var cts = new CancellationTokenSource();
        _loadCts = cts;

        var nextPage = resetPagination ? 1 : _currentPage + 1;

        DebugInfo = $"Fetching page={nextPage} search='{SearchText}' cat={_categoryId} @ {DateTime.Now:HH:mm:ss.fff}";

        try
        {
            var result = await _productService.GetProductsPagedAsync(
                categoryId: _categoryId,
                search: string.IsNullOrWhiteSpace(SearchText) ? null : SearchText,
                page: nextPage,
                pageSize: PageSize,
                cancellationToken: cts.Token);

            if (cts.Token.IsCancellationRequested)
            {
                DebugInfo = $"Discarded stale result for page={nextPage}";
                return;
            }

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                if (resetPagination)
                    Products.Clear();

                foreach (var product in result.Items)
                    Products.Add(product);

                _currentPage = result.PageNumber > 0 ? result.PageNumber : nextPage;
                HasMoreData = result.HasNext;
                HasError = false;
            });

            DebugInfo = $"Got {result.Items.Count} items, total={result.TotalCount}, hasNext={result.HasNext}, ProductsNow={Products.Count}";
        }
        catch (OperationCanceledException)
        {
            DebugInfo = "OperationCanceledException";
        }
        catch (Exception ex)
        {
            DebugInfo = $"EXCEPTION: {ex.GetType().Name}: {ex.Message}";
            if (resetPagination)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Products.Clear();
                    HasError = true;
                    HasMoreData = false;
                });
            }
        }
    }

    [RelayCommand]
    async Task OpenProduct(Product product)
    {
        if (product is null)
            return;

        await _navigationService.NaviagteAsync<ProductDetailPage>(parameters: new Dictionary<string, object>
        {
            ["Product"] = product
        });
    }

    [RelayCommand]
    async Task AddToCart(Product product)
    {
        if (product is null || !product.CanAddToCart)
            return;

        await _cartService.AddToCartAsync(product.Id, 1);

        _dialogService.ShowToast($"{product.Name} added to cart.");
    }
}
