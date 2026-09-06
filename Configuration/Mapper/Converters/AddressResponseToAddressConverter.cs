using Equiparts.Models;
using Equiparts.Models.Profile;

namespace Equiparts.Configuration.Mapper.Converters;

public class AddressResponseToAddressConverter : ConverterBase<AddressResponse, Address>
{
    protected override Address ConvertImpl(AddressResponse source)
    {
        return new Address
        {
            Id = source.Id,
            Label = source.Label,
            AddressLine1 = source.AddressLine1,
            AddressLine2 = source.AddressLine2,
            City = source.City,
            State = source.State,
            PostalCode = source.PostalCode,
            Country = source.Country,
            PhoneNumber = source.PhoneNumber,
            IsDefault = source.IsDefault
        };
    }
}
