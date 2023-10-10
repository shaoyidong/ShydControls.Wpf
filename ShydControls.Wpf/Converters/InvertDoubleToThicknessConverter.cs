using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ShydControls.Wpf.Converters
{
    [ValueConversion(typeof(double), typeof(Thickness))]
    public class InvertDoubleToThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(parameter is Dock dock && value is double num)
            {
                num = 0 - num;
                switch (dock)
                {
                    case Dock.Left:
                        return new Thickness(num, 0, 0, 0);
                    case Dock.Top:
                        return new Thickness(0, num, 0, 0);
                    case Dock.Right:
                        return new Thickness(0, 0, num, 0);
                    case Dock.Bottom:
                        return new Thickness(0, 0, 0, num);
                    default:
                        break;
                }
            }
            return Binding.DoNothing;

        }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
