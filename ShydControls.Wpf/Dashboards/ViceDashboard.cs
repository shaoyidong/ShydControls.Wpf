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

namespace ShydControls.Wpf.Dashboards
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ShydControls.Wpf.Dashboards"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ShydControls.Wpf.Dashboards;assembly=ShydControls.Wpf.Dashboards"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:ViceDashboard/>
    ///
    /// </summary>
    public class ViceDashboard : Control
    {
        static ViceDashboard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ViceDashboard), new FrameworkPropertyMetadata(typeof(ViceDashboard)));
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(ViceDashboard), new PropertyMetadata(75.0, new PropertyChangedCallback(OnPropertyChanged)));

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Maximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(ViceDashboard), new PropertyMetadata(100.0, OnPropertyChanged));


        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(ViceDashboard), new PropertyMetadata(0.0, OnPropertyChanged));

        public static readonly DependencyProperty StartAngleProperty = DependencyProperty.Register(
            "StartAngle", typeof(double), typeof(ViceDashboard), new PropertyMetadata(135.0, OnPropertyChanged));

        public static readonly DependencyProperty AngleRangeProperty = DependencyProperty.Register(
            "AngleRange", typeof(double), typeof(ViceDashboard), new PropertyMetadata(90.0, OnPropertyChanged));
        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(
         "Thickness", typeof(double), typeof(ViceDashboard), new PropertyMetadata(8.0, OnPropertyChanged));

        // 属性访问器
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public double StartAngle
        {
            get { return (double)GetValue(StartAngleProperty); }
            set { SetValue(StartAngleProperty, value); }
        }

        public double AngleRange
        {
            get { return (double)GetValue(AngleRangeProperty); }
            set { SetValue(AngleRangeProperty, value); }
        }
        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ViceDashboard)d).InvalidateVisual();
        }

        // 重写OnRender方法进行绘制
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (ActualWidth <= 0 || ActualHeight <= 0)
                return;

            double value = Value;

            if (value < Minimum)
            {
                value = Minimum;
            }
            else if (value > Maximum)
            {
                value = Maximum;
            }

            double width = ActualWidth;
            double height = ActualHeight;
            double centerX = width / 2;
            double centerY = height / 2;

            double diameter = Math.Min(width, height) - (Thickness);
            double radius = diameter / 2.0;

            // 绘制小角度弧形进度条
            double endAngle = StartAngle + AngleRange;

            // 绘制背景弧
            DrawArc(drawingContext, centerX, centerY, radius, StartAngle, endAngle, Thickness,
                    new SolidColorBrush(Color.FromArgb(38, 0xf0, 0xf1, 0xff)));

            // 计算进度百分比
            double percentage = (Value - Minimum) / (Maximum - Minimum);
            if (percentage > 1.0) percentage = 1.0;

            // 绘制进度弧
            double progressEndAngle = StartAngle + (AngleRange * percentage);
            DrawArc(drawingContext, centerX, centerY, radius, StartAngle, progressEndAngle, Thickness,
                    Foreground);
        }

        // 绘制弧线
        private void DrawArc(DrawingContext dc, double centerX, double centerY, double radius,
                           double startAngle, double endAngle, double thickness, Brush brush, double[] dashPattern = null, double dashOffset = 0)
        {
            // 转换角度为弧度
            double startRad = (startAngle) * Math.PI / 180.0;
            double endRad = (endAngle) * Math.PI / 180.0;

            // 计算起始点和结束点
            Point startPoint = new Point(
                centerX + radius * Math.Cos(startRad),
                centerY + radius * Math.Sin(startRad));

            Point endPoint = new Point(
                centerX + radius * Math.Cos(endRad),
                centerY + radius * Math.Sin(endRad));

            // 创建PathFigure
            PathFigure pathFigure = new PathFigure
            {
                StartPoint = startPoint,
                IsClosed = false,
                IsFilled = false
            };

            // 计算弧的大小
            Size arcSize = new Size(radius, radius);

            // 判断是否为大弧
            bool isLargeArc = Math.Abs(endAngle - startAngle) > 180;

            // 创建ArcSegment
            ArcSegment arcSegment = new ArcSegment
            {
                Point = endPoint,
                Size = arcSize,
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = isLargeArc,
                RotationAngle = 0
            };

            // 添加到PathFigure
            pathFigure.Segments.Add(arcSegment);

            // 创建PathGeometry
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(pathFigure);

            // 绘制Path
            Pen pen = new Pen(brush, thickness);
            pen.StartLineCap = PenLineCap.Round;
            pen.EndLineCap = PenLineCap.Round;

            // 设置虚线样式，从参数获取虚线模式
            pen.DashStyle = dashPattern != null ? new DashStyle(dashPattern, dashOffset) : DashStyles.Solid;
            pen.DashCap = PenLineCap.Round;

            dc.DrawGeometry(null, pen, pathGeometry);
        }

        /// 使用裁剪区域绘制弧的方法
        /// 先绘制完整圆，然后通过几何裁剪区域只显示指定角度范围的弧
        /// </summary>
        private void DrawArcByClipping(DrawingContext dc, double centerX, double centerY, double radius,
                                     double startAngle, double endAngle, double thickness, Brush brush)
        {
            // 创建裁剪区域（扇形）
            PathGeometry clipGeometry = new PathGeometry();
            PathFigure clipFigure = new PathFigure
            {
                StartPoint = new Point(centerX, centerY), // 中心点
                IsClosed = true,
                IsFilled = true
            };

            // 转换角度为弧度
            double startRad = (startAngle - 90) * Math.PI / 180.0;
            double endRad = (endAngle - 90) * Math.PI / 180.0;

            // 计算扇形的两个端点（使用稍大的半径确保完全覆盖弧）
            double clipRadius = radius + thickness / 2 + 5;
            Point outerStartPoint = new Point(
                centerX + clipRadius * Math.Cos(startRad),
                centerY + clipRadius * Math.Sin(startRad));
            Point outerEndPoint = new Point(
                centerX + clipRadius * Math.Cos(endRad),
                centerY + clipRadius * Math.Sin(endRad));

            // 添加扇形路径
            clipFigure.Segments.Add(new LineSegment(outerStartPoint, true));

            // 创建弧段连接两个端点
            ArcSegment arcSegment = new ArcSegment
            {
                Point = outerEndPoint,
                Size = new Size(clipRadius, clipRadius),
                SweepDirection = endAngle >= startAngle ?
                                 SweepDirection.Clockwise : SweepDirection.Counterclockwise,
                IsLargeArc = Math.Abs(endAngle - startAngle) > 180,
                RotationAngle = 0
            };
            clipFigure.Segments.Add(arcSegment);

            clipFigure.Segments.Add(new LineSegment(new Point(centerX, centerY), true)); // 回到中心点
            clipGeometry.Figures.Add(clipFigure);

            // 保存当前绘制上下文
            dc.PushClip(clipGeometry);

            try
            {
                // 绘制完整的圆环（外圆和内圆之间的区域）
                double outerRadius = radius + thickness / 2;
                double innerRadius = radius - thickness / 2;

                // 外圆
                // 创建一个几何图形组，包含外圆和内圆
                GeometryGroup geometryGroup = new GeometryGroup();
                geometryGroup.Children.Add(new EllipseGeometry(new Point(centerX, centerY), outerRadius, outerRadius));
                geometryGroup.Children.Add(new EllipseGeometry(new Point(centerX, centerY), innerRadius, innerRadius));

                // 使用组合模式创建圆环
                CombinedGeometry ringGeometry = new CombinedGeometry(GeometryCombineMode.Exclude, geometryGroup.Children[0], geometryGroup.Children[1]);

                // 使用填充方式绘制圆环
                dc.DrawGeometry(brush, null, ringGeometry);
            }
            finally
            {
                // 恢复绘制上下文，移除裁剪区域
                dc.Pop();
            }
        }
    }
}
