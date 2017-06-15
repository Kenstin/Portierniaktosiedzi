using System;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Portierniaktosiedzi.Models;

namespace Portierniaktosiedzi.Converters
{
    public class HideEmployeeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] == null || values[1] == null)
            {
                return false;
            }

            var employee = (Employee)values[0];
            var deletedEmployees = (Collection<Employee>)values[1];
            return deletedEmployees.Contains(employee);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
