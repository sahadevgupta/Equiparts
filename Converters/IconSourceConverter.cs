using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Equiparts.Controls.Enums;

namespace Equiparts.Converter
{
    public class IconSourceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 4)
                return null;

            if (values[0] is not CustomIconType iconType)
                return null;

            switch (iconType)
            {
                case CustomIconType.Font:
                    {
                        var glyph = values[1]?.ToString();

                        if (string.IsNullOrWhiteSpace(glyph))
                            return null;

                        var color = values[2] as Color ?? Colors.Black;

                        return new FontImageSource
                        {
                            FontFamily = "FontAwesome",
                            Glyph = glyph,
                            Color = color,
                            Size = System.Convert.ToDouble(values[3])
                        };
                    }
                case CustomIconType.Image:
                    return values[1];
                default:
                    return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}