using System.Text.RegularExpressions;
using Equiparts.Models;
using Mopups.Pages;
using Mopups.Services;

namespace Equiparts.Controls;

public partial class AddressEditorPopup : PopupPage
{
    public static readonly BindableProperty HeaderTextProperty =
        BindableProperty.Create(nameof(HeaderText), typeof(string), typeof(AddressEditorPopup), string.Empty);

    public static readonly BindableProperty FullNameProperty =
        BindableProperty.Create(nameof(FullName), typeof(string), typeof(AddressEditorPopup), string.Empty);

    public static readonly BindableProperty MobileNumberProperty =
        BindableProperty.Create(nameof(MobileNumber), typeof(string), typeof(AddressEditorPopup), string.Empty);

    public static readonly BindableProperty AddressLine1Property =
        BindableProperty.Create(nameof(AddressLine1), typeof(string), typeof(AddressEditorPopup), string.Empty);

    public static readonly BindableProperty AddressLine2Property =
        BindableProperty.Create(nameof(AddressLine2), typeof(string), typeof(AddressEditorPopup), string.Empty);

    public static readonly BindableProperty CityProperty =
        BindableProperty.Create(nameof(City), typeof(string), typeof(AddressEditorPopup), string.Empty);

    public static readonly BindableProperty StateProperty =
        BindableProperty.Create(nameof(State), typeof(string), typeof(AddressEditorPopup), string.Empty);

    public static readonly BindableProperty PostalCodeProperty =
        BindableProperty.Create(nameof(PostalCode), typeof(string), typeof(AddressEditorPopup), string.Empty);

    public static readonly BindableProperty LandmarkProperty =
        BindableProperty.Create(nameof(Landmark), typeof(string), typeof(AddressEditorPopup), string.Empty);

    public static readonly BindableProperty IsDefaultProperty =
        BindableProperty.Create(nameof(IsDefault), typeof(bool), typeof(AddressEditorPopup), false);

    public static readonly BindableProperty ErrorMessageProperty =
        BindableProperty.Create(nameof(ErrorMessage), typeof(string), typeof(AddressEditorPopup), string.Empty);

    private readonly int _addressId;
    private readonly string _country;
    private readonly TaskCompletionSource<Address?> _resultCompletionSource = new();

    public Task<Address?> Result => _resultCompletionSource.Task;

    public string HeaderText
    {
        get => (string)GetValue(HeaderTextProperty);
        set => SetValue(HeaderTextProperty, value);
    }

    public string FullName
    {
        get => (string)GetValue(FullNameProperty);
        set => SetValue(FullNameProperty, value);
    }

    public string MobileNumber
    {
        get => (string)GetValue(MobileNumberProperty);
        set => SetValue(MobileNumberProperty, value);
    }

    public string AddressLine1
    {
        get => (string)GetValue(AddressLine1Property);
        set => SetValue(AddressLine1Property, value);
    }

    public string AddressLine2
    {
        get => (string)GetValue(AddressLine2Property);
        set => SetValue(AddressLine2Property, value);
    }

    public string City
    {
        get => (string)GetValue(CityProperty);
        set => SetValue(CityProperty, value);
    }

    public string State
    {
        get => (string)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    public string PostalCode
    {
        get => (string)GetValue(PostalCodeProperty);
        set => SetValue(PostalCodeProperty, value);
    }

    public string Landmark
    {
        get => (string)GetValue(LandmarkProperty);
        set => SetValue(LandmarkProperty, value);
    }

    public bool IsDefault
    {
        get => (bool)GetValue(IsDefaultProperty);
        set => SetValue(IsDefaultProperty, value);
    }

    public string ErrorMessage
    {
        get => (string)GetValue(ErrorMessageProperty);
        set => SetValue(ErrorMessageProperty, value);
    }

    public AddressEditorPopup(Address? editingAddress = null)
    {
        _addressId = editingAddress?.Id ?? 0;
        _country = string.IsNullOrWhiteSpace(editingAddress?.Country) ? "India" : editingAddress!.Country;
        HeaderText = editingAddress is null ? "Add New Address" : "Edit Address";

        if (editingAddress is not null)
        {
            FullName = editingAddress.FullName;
            MobileNumber = editingAddress.PhoneNumber ?? string.Empty;
            AddressLine1 = editingAddress.Line1;
            AddressLine2 = editingAddress.Line2 ?? string.Empty;
            City = editingAddress.City;
            State = editingAddress.State;
            PostalCode = editingAddress.PostalCode;
            Landmark = editingAddress.Landmark ?? string.Empty;
            IsDefault = editingAddress.IsDefault;
        }

        InitializeComponent();
        BindingContext = this;
    }

    async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(FullName) ||
            string.IsNullOrWhiteSpace(MobileNumber) ||
            string.IsNullOrWhiteSpace(AddressLine1) ||
            string.IsNullOrWhiteSpace(City) ||
            string.IsNullOrWhiteSpace(State) ||
            string.IsNullOrWhiteSpace(PostalCode))
        {
            ErrorMessage = "Please fill all required fields.";
            return;
        }

        if (!Regex.IsMatch(MobileNumber.Trim(), @"^[0-9]{10}$"))
        {
            ErrorMessage = "Enter a valid 10-digit mobile number.";
            return;
        }

        if (!Regex.IsMatch(PostalCode.Trim(), @"^[0-9]{6}$"))
        {
            ErrorMessage = "Enter a valid 6-digit pincode.";
            return;
        }

        ErrorMessage = string.Empty;

        var address = new Address
        {
            Id = _addressId,
            FullName = FullName.Trim(),
            PhoneNumber = MobileNumber.Trim(),
            Line1 = AddressLine1.Trim(),
            Line2 = string.IsNullOrWhiteSpace(AddressLine2) ? null : AddressLine2.Trim(),
            City = City.Trim(),
            State = State.Trim(),
            PostalCode = PostalCode.Trim(),
            Country = _country,
            Landmark = string.IsNullOrWhiteSpace(Landmark) ? null : Landmark.Trim(),
            IsDefault = IsDefault
        };

        await MopupService.Instance.PopAsync();
        _resultCompletionSource.TrySetResult(address);
    }

    async void OnCancelClicked(object sender, EventArgs e)
    {
        await MopupService.Instance.PopAsync();
        _resultCompletionSource.TrySetResult(null);
    }
}
