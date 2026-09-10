using System.Globalization;

namespace Equiparts.Converter;

// Used to highlight the selected item in a list (e.g. payment methods) by comparing
// the item bound in the DataTemplate against the ViewModel's currently-selected one.
public class EqualityMultiConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is null || values.Length < 2)
            return false;

        bool isEqual = Equals(values[0], values[1]);

        return string.Equals(parameter as string, "Invert", StringComparison.OrdinalIgnoreCase)
            ? !isEqual
            : isEqual;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
