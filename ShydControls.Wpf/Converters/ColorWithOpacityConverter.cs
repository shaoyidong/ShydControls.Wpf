using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace ShydControls.Wpf.Converters
{
    public class ColorWithOpacityConverter : IValueConverter
    {
        /// <summary>
        /// 转换值
        /// </summary>
        /// <param name="value">原始颜色值</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="parameter">透明度参数（十六进制字符串，如"66"、"CC"等）</param>
        /// <param name="culture">当前文化</param>
        /// <returns>带有指定透明度的颜色</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 检查value是否为Color类型
            if (!(value is Color baseColor))
                return Colors.White; // 默认返回白色

            // 检查parameter是否为有效的十六进制字符串
            if (parameter is string opacityHex && !string.IsNullOrEmpty(opacityHex))
            {
                try
                {
                    // 将十六进制字符串转换为byte值
                    byte opacity = byte.Parse(opacityHex, NumberStyles.HexNumber);
                    // 创建新的颜色，保留原始颜色的RGB值，使用新的透明度
                    return Color.FromArgb(opacity, baseColor.R, baseColor.G, baseColor.B);
                }
                catch (FormatException)
                {
                    // 如果解析失败，返回带有50%透明度的原始颜色
                    return Color.FromArgb(128, baseColor.R, baseColor.G, baseColor.B);
                }
            }

            // 如果没有提供透明度参数，返回原始颜色
            return baseColor;
        }

        /// <summary>
        /// 转换回原始值（在这个场景中不使用）
        /// </summary>
        /// <param name="value">转换后的值</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="parameter">参数</param>
        /// <param name="culture">当前文化</param>
        /// <returns>原始颜色值</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
