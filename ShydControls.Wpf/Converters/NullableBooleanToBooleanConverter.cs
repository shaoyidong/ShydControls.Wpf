using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ShydControls.Wpf.Converters
{
    [ValueConversion(typeof(bool?), typeof(bool))]
    public class NullableBooleanToBooleanConverter:IValueConverter    {
       
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;
            return (bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            return (bool)value;
        }
    }
}
