using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace MyApp.Converters
{
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool isAdmin && !isAdmin; // Inverse la valeur booléenne
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
