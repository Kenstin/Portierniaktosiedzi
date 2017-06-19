using System;
using System.Windows.Data;
using Portierniaktosiedzi.Models;

namespace Portierniaktosiedzi.Converters
{
    public class GenderToPolishConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                return value.Equals(Gender.Man) ? "Mezczyzna" : "Kobieta";
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
