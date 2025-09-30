using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShydControls.Wpf.Aperture
{
    /// <summary>
    /// CircularProgressBar.xaml 的交互逻辑
    /// </summary>
    public partial class CircularProgressBar : UserControl
    {
        // 定义RingSize依赖属性
        public static readonly DependencyProperty RingSizeProperty =
            DependencyProperty.Register("RingSize", typeof(double), typeof(CircularProgressBar),
            new FrameworkPropertyMetadata(200.0, FrameworkPropertyMetadataOptions.AffectsRender));

        // 定义ProgressValue依赖属性
        public static readonly DependencyProperty ProgressValueProperty =
            DependencyProperty.Register("ProgressValue", typeof(double), typeof(CircularProgressBar),
            new FrameworkPropertyMetadata(65.0, FrameworkPropertyMetadataOptions.AffectsRender, OnProgressValueChanged));

        // RingSize属性的getter和setter
        public double RingSize
        {
            get { return (double)GetValue(RingSizeProperty); }
            set { SetValue(RingSizeProperty, value); }
        }

        // ProgressValue属性的getter和setter
        public double ProgressValue
        {
            get { return (double)GetValue(ProgressValueProperty); }
            set { SetValue(ProgressValueProperty, Math.Max(0, Math.Min(100, value))); }
        }

        // 进度值变化的回调函数
        private static void OnProgressValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CircularProgressBar;
            if (control != null)
            {
                // 确保进度值在0-100范围内
                double newValue = Math.Max(0, Math.Min(100, (double)e.NewValue));
                if (newValue != (double)e.NewValue)
                {
                    control.ProgressValue = newValue;
                }
                PathGeometry geometry = PathGeometry.CreateFromGeometry(PathGeometry.Parse(control.GetProgressPath()));
                control.ProgressPath.Data = geometry;
            }
        }
        // 计算进度路径
        private string GetProgressPath()
        {
            double progress = Math.Min(100, Math.Max(0, ProgressValue));
            double angle = (progress / 100) * 360;

            double xoffset = (this.Width - RingSize) / 2;
            double yoffset = (this.Height - RingSize) / 2;

            xoffset = 2.5;
            yoffset = 2.5;

            double radius = RingSize * 0.5;
            // 处理边界情况
            if (angle >= 360)
                return $"M {radius * 2 + xoffset},{radius + yoffset} A {radius},{radius} 0 1 1 {xoffset},{radius + yoffset} A {radius},{radius} 0 1 1 {radius * 2 + xoffset},{radius + yoffset}";
            //if (angle <= 0)
            //    return ""; // 空路径

            // 对于小于360度的角度，我们需要计算终点坐标
            double radians = angle * Math.PI / 180;
            double endX = radius + radius * Math.Sin(radians) + xoffset;
            double endY = radius - radius * Math.Cos(radians) + yoffset;

            // 确定是否超过半圆
            bool isLargeArc = angle > 180;

            // 构建路径数据
            // M: 移动到起点(1,0)，这是圆的顶部
            // A: 添加圆弧段，半径为1,1，终点为(endX, endY)
            return $"M {radius + xoffset},{yoffset} A {radius},{radius} 0 {(isLargeArc ? 1 : 0)} 1 {endX},{endY}";
        }

        public CircularProgressBar()
        {
            // 注册转换器
            this.Resources.Add("SizeConverter", new Converters.SizeConverter());
            InitializeComponent();
        }
    }
}
