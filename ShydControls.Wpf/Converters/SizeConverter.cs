using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ShydControls.Wpf.Converters
{
    public class SizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double size && parameter is string param)
            {
                // 解析参数，支持+和-操作
                if (param.StartsWith("+"))
                {
                    if (double.TryParse(param.Substring(1), out double addValue))
                    {
                        return size + addValue;
                    }
                }
                else if (param.StartsWith("-"))
                {
                    if (double.TryParse(param.Substring(1), out double subValue))
                    {
                        return size - subValue;
                    }
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
