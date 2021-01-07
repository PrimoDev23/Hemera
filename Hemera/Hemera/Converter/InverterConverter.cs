using System;
using System.Globalization;
using Xamarin.Forms;

namespace Hemera.Converter
{
    public class InverterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return !b;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //We don't actually need this
            throw new NotImplementedException();
        }
    }
}
