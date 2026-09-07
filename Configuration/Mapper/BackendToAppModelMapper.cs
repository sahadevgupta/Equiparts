using Equiparts.Configuration.Mapper.Converters;
using Equiparts.Models;
using Equiparts.Models.Auth;
using Equiparts.Models.Catalog;
using Equiparts.Models.Orders;
using Equiparts.Models.Profile;

namespace Equiparts.Configuration.Mapper;

public static class BackendToAppModelMapper
{
    public static UserSession? GetUserSession(LoginResponse? loginResponse)
    {
        if (loginResponse is null)
            return null;

        var converter = new LoginResponseToUserSessionConverter();
        return converter.Convert(loginResponse);
    }

    public static UserProfile? GetUserProfile(ProfileResponse? profileResponse)
    {
        if (profileResponse is null)
            return null;

        var converter = new ProfileResponseToUserProfileConverter();
        return converter.Convert(profileResponse);
    }

    public static List<Banner> GetBanners(List<BannerResponse>? bannerResponses)
    {
        if (bannerResponses is null)
            return [];

        var converter = new BannerResponseToBannerConverter();
        return bannerResponses.Select(converter.Convert).ToList();
    }

    public static List<Category> GetCategories(List<CategoryResponse>? categoryResponses)
    {
        if (categoryResponses is null)
            return [];

        var converter = new CategoryResponseToCategoryConverter();
        return categoryResponses.Select(converter.Convert).ToList();
    }

    public static List<Category> GetProducts(List<ProductResponse>? productResponses)
    {
        if (productResponses is null)
            return [];

        var converter = new CategoryResponseToCategoryConverter();
        return productResponses.Select(converter.Convert).ToList();
    }

    public static Order? GetOrder(OrderResponse? orderResponse)
    {
        if (orderResponse is null)
            return null;

        var converter = new OrderResponseToOrderConverter();
        return converter.Convert(orderResponse);
    }

    public static List<Order> GetOrders(List<OrderResponse>? orderResponses)
    {
        if (orderResponses is null)
            return [];

        var converter = new OrderResponseToOrderConverter();
        return orderResponses.Select(converter.Convert).ToList();
    }

    public static Address? GetAddress(AddressResponse? addressResponse)
    {
        if (addressResponse is null)
            return null;

        var converter = new AddressResponseToAddressConverter();
        return converter.Convert(addressResponse);
    }

    public static List<Address> GetAddresses(List<AddressResponse>? addressResponses)
    {
        if (addressResponses is null)
            return [];

        var converter = new AddressResponseToAddressConverter();
        return addressResponses.Select(converter.Convert).ToList();
    }
}
