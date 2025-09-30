using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// GlowRingControl.xaml 的交互逻辑
    /// </summary>
    public partial class GlowRingControl : UserControl
    {
        // 定义RingSize依赖属性
        public static readonly DependencyProperty RingSizeProperty =
            DependencyProperty.Register("RingSize", typeof(double), typeof(GlowRingControl),
            new FrameworkPropertyMetadata(200.0, FrameworkPropertyMetadataOptions.AffectsRender));

        // RingSize属性的getter和setter
        public double RingSize
        {
            get { return (double)GetValue(RingSizeProperty); }
            set { SetValue(RingSizeProperty, value); }
        }

        public GlowRingControl()
        {
            // 注册转换器
            this.Resources.Add("SizeConverter", new Converters.SizeConverter());
            InitializeComponent();
        }
    }   
}
