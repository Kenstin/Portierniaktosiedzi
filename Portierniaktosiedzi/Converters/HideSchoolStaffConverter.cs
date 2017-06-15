using System;
using System.Windows.Data;
using Portierniaktosiedzi.Models;

namespace Portierniaktosiedzi.Converters
{
    public class HideSchoolStaffConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                return value.GetType() == typeof(SchoolStaff);
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
