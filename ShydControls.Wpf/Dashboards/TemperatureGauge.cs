using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace ShydControls.Wpf.Dashboards
{
    /// <summary>
    /// 小角度弧形进度条，用于显示温度信息
    /// </summary>
    public class TemperatureGauge : Control
    {
        // 依赖属性定义
        public static readonly DependencyProperty TemperatureProperty = DependencyProperty.Register(
            "Temperature", typeof(double), typeof(TemperatureGauge), new PropertyMetadata(50.0, OnPropertyChanged));

        public static readonly DependencyProperty MaxTemperatureProperty = DependencyProperty.Register(
            "MaxTemperature", typeof(double), typeof(TemperatureGauge), new PropertyMetadata(100.0));

        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(
            "Radius", typeof(double), typeof(TemperatureGauge), new PropertyMetadata(60.0, OnPropertyChanged));

        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(
            "Thickness", typeof(double), typeof(TemperatureGauge), new PropertyMetadata(8.0, OnPropertyChanged));

        public static readonly DependencyProperty StartAngleProperty = DependencyProperty.Register(
            "StartAngle", typeof(double), typeof(TemperatureGauge), new PropertyMetadata(135.0, OnPropertyChanged));

        public static readonly DependencyProperty AngleRangeProperty = DependencyProperty.Register(
            "AngleRange", typeof(double), typeof(TemperatureGauge), new PropertyMetadata(90.0, OnPropertyChanged));

        // 属性访问器
        public double Temperature
        {
            get { return (double)GetValue(TemperatureProperty); }
            set { SetValue(TemperatureProperty, value); }
        }

        public double MaxTemperature
        {
            get { return (double)GetValue(MaxTemperatureProperty); }
            set { SetValue(MaxTemperatureProperty, value); }
        }

        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
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

        // 构造函数
        public TemperatureGauge()
        {
            this.SizeChanged += TemperatureGauge_SizeChanged;
        }

        private void TemperatureGauge_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.InvalidateVisual();
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TemperatureGauge)d).InvalidateVisual();
        }

        // 重写OnRender方法进行绘制
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            double width = ActualWidth;
            double height = ActualHeight;
            double centerX = width / 2;
            double centerY = height / 2;

            // 计算实际半径（取设置值和可用空间的最小值）
            double actualRadius = Math.Min(Radius, Math.Min(width, height) * 0.45);

            // 绘制小角度弧形进度条
            double endAngle = StartAngle + AngleRange;

            // 绘制总背景弧
            DrawArcByClipping(drawingContext, centerX, centerY, actualRadius, StartAngle, endAngle, Thickness * 4.5,
                    new SolidColorBrush(Color.FromRgb(45, 45, 45)));
            // 绘制外弧
            DrawArc(drawingContext, centerX, centerY, actualRadius + (Thickness * 4.5 / 2 + 2 / 2), StartAngle, endAngle, 2,
                    new SolidColorBrush(Color.FromRgb(200, 0, 0)));

            // 绘制背景弧
            DrawArc(drawingContext, centerX, centerY, actualRadius + (Thickness * 4.5 / 2 - Thickness), StartAngle, endAngle, Thickness,
                    new SolidColorBrush(Color.FromRgb(53, 59, 71)), new double[] { 3, 0.2 });

            // 计算进度百分比
            double percentage = Temperature / MaxTemperature;
            if (percentage > 1.0) percentage = 1.0;

            // 绘制进度弧
            double progressEndAngle = StartAngle + (AngleRange * percentage);
            DrawArc(drawingContext, centerX, centerY, actualRadius + (Thickness * 4.5 / 2 - Thickness), StartAngle, progressEndAngle, Thickness,
                    new SolidColorBrush(Color.FromRgb(255, 0, 0)), new double[] { 3, 0.2 });

            //// 绘制温度文本
            //double textAngle = StartAngle + AngleRange / 2; // 文本居中显示
            //DrawTemperatureText(drawingContext, centerX, centerY, actualRadius + Thickness * 2, textAngle, Temperature);
        }

        // 绘制弧线
        private void DrawArc(DrawingContext dc, double centerX, double centerY, double radius,
                           double startAngle, double endAngle, double thickness, Brush brush, double[] dashPattern = null, double dashOffset = 0)
        {
            // 转换角度为弧度
            double startRad = (startAngle - 90) * Math.PI / 180.0;
            double endRad = (endAngle - 90) * Math.PI / 180.0;

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
            pen.StartLineCap = PenLineCap.Flat;
            pen.EndLineCap = PenLineCap.Flat;

            // 设置虚线样式，从参数获取虚线模式
            pen.DashStyle = dashPattern != null ? new DashStyle(dashPattern, dashOffset) : DashStyles.Solid;
            pen.DashCap = PenLineCap.Flat;

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

        // 绘制温度文本
        private void DrawTemperatureText(DrawingContext dc, double centerX, double centerY,
                                        double radius, double angle, double temperature)
        {
            double rad = (angle - 90) * Math.PI / 180.0;
            Point textPoint = new Point(
                centerX + radius * Math.Cos(rad),
                centerY + radius * Math.Sin(rad));

            FormattedText formattedText = new FormattedText(
                $"{temperature}°C",
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"),
                14,
                Brushes.White);

            // 调整文本位置使其居中
            textPoint.X -= formattedText.Width / 2;
            textPoint.Y -= formattedText.Height / 2;

            dc.DrawText(formattedText, textPoint);
        }

        // 根据温度获取颜色
        private Brush GetTemperatureColor(double percentage)
        {
            if (percentage < 0.5)
                return new SolidColorBrush(Color.FromRgb(80, 220, 255)); // 蓝色 - 低温
            else if (percentage < 0.75)
                return new SolidColorBrush(Color.FromRgb(255, 220, 80)); // 黄色 - 中温
            else
                return new SolidColorBrush(Color.FromRgb(255, 80, 80)); // 红色 - 高温
        }
    }
}
