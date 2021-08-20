using System;
using System.Globalization;
using System.Windows.Data;

namespace ManagementUI.Models.Converters
{
    [ValueConversion(typeof(uint), typeof(string))]
    public class UIntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue && uint.TryParse(strValue, out uint realValue))
                return realValue;

            else
                return 0u;
        }
    }
}
