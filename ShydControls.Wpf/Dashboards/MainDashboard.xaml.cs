using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Threading;

namespace ShydControls.Wpf.Dashboards
{
    /// <summary>
    /// MainDashboard.xaml 的交互逻辑
    /// </summary>
    public partial class MainDashboard : UserControl
    {
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(MainDashboard), new PropertyMetadata(75.0, new PropertyChangedCallback(OnValueChanged)));

        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Maximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(int), typeof(MainDashboard), new PropertyMetadata(100, OnMinimumOrMaximumChanged));


        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(int), typeof(MainDashboard), new PropertyMetadata(0, OnMinimumOrMaximumChanged));

        public MainDashboard()
        {
            InitializeComponent();
            this.SizeChanged += MainDashboard_SizeChanged;            
        }

        private void MainDashboard_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateClip(Value);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mainDashboard = d as MainDashboard;
            if (mainDashboard != null)
            {
                mainDashboard.UpdateClip((double)e.NewValue);
            }
        }

        private static void OnMinimumOrMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mainDashboard = d as MainDashboard;
            if (mainDashboard != null)
            {
                mainDashboard.UpdateClip(mainDashboard.Value);
            }
        }      

        // 生成一个环形扇区（annular sector）的 Geometry 并设置到 Image.Clip
        private void UpdateClip(double value)
        {
            if (arcImage.ActualWidth <= 0 || arcImage.ActualHeight <= 0)
                return;

            if (value < Minimum)
            {
                value = Minimum;
            }
            else if (value > Maximum)
            {
                value = Maximum;
            }

            double angleDegrees = 272 * (value - Minimum) / (Maximum - Minimum);
            rot.Angle = angleDegrees;
            if (angleDegrees >= 272 || angleDegrees < 0)
            {
                arcImage.Clip = null;
                return;
            }

            double w = arcImage.ActualWidth;
            double h = arcImage.ActualHeight;
            // 使用图片显示区域的最小边作为直径参考（保证是圆）
            double diameter = Math.Min(w, h);
            Point center = new Point(w / 2.0, h / 2.0);
            double radius = diameter / 2.0;

            // 从 -90 度（顶部）开始顺时针绘制弧（视觉上像进度从顶部开始）
            double startAngle = 134;
            double endAngle = startAngle + angleDegrees;

            // 角度到弧上的点
            Point GetPoint(double thetaDeg)
            {
                double theta = thetaDeg * Math.PI / 180.0;
                return new Point(center.X + radius * Math.Cos(theta), center.Y + radius * Math.Sin(theta));
            }

            var geom = new StreamGeometry();
            using (var ctx = geom.Open())
            {
                // 外弧起点
                Point p0 = GetPoint(startAngle);
                ctx.BeginFigure(p0, true, true); // isFilled, isClosed

                // 外弧
                bool isLargeArc = angleDegrees > 180.0;
                Point pOuterEnd = GetPoint(endAngle);
                ctx.ArcTo(pOuterEnd, new Size(radius, radius), 0, isLargeArc, SweepDirection.Clockwise, true, false);

                // 连接到中点

                ctx.LineTo(center, true, false);

                // Line 回到起点（StreamGeometry 会自动闭合）
            }
            geom.Freeze();

            arcImage.Clip = geom;
        }
    }
}
