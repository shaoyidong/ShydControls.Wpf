using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ShydControls.Wpf.Converters
{
    [ValueConversion(typeof(bool), typeof(string))]
    public class BooleanToStringValveThemeConverter : IValueConverter
    {
        private string _trueStr = "green";
        private string _falseStr = "red";
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return b ? _trueStr : _falseStr;
            }
            return _falseStr;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string v)
            {
                return (v.Equals(_trueStr));
            }
            return false;
        }
    }
}
