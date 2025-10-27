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
    public class PwmGauge : Control
    {
        // 依赖属性定义
        public static readonly DependencyProperty PwmPercentageProperty = DependencyProperty.Register(
            "PwmPercentage", typeof(double), typeof(PwmGauge), new PropertyMetadata(40.0, OnPropertyChanged));

        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(
            "Radius", typeof(double), typeof(PwmGauge), new PropertyMetadata(120.0, OnPropertyChanged));

        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(
            "Thickness", typeof(double), typeof(PwmGauge), new PropertyMetadata(12.0, OnPropertyChanged));

        // 属性访问器
        public double PwmPercentage
        {
            get { return (double)GetValue(PwmPercentageProperty); }
            set { SetValue(PwmPercentageProperty, value); }
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

        // 构造函数
        public PwmGauge()
        {
            this.SizeChanged += PwmGauge_SizeChanged;
        }

        private void PwmGauge_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.InvalidateVisual();
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PwmGauge)d).InvalidateVisual();
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

            // 绘制270°弧形进度条
            double startAngle = 225; // 从135°开始
            double angleRange = 270; // 270°范围
            double endAngle = startAngle + angleRange;

            // 绘制外弧
            DrawArc(drawingContext, centerX, centerY, actualRadius, startAngle, endAngle, 2,
                    new SolidColorBrush(Color.FromRgb(60, 60, 60)));


            // 绘制背景弧
            double backRadius = actualRadius - ((2 / 2) + Thickness * 1.8 / 2) - Thickness * 0.5;
            DrawArc(drawingContext, centerX, centerY, backRadius, startAngle, endAngle, Thickness * 1.8,
                    new SolidColorBrush(Color.FromRgb(53, 59, 71)));

            // 计算进度百分比（PwmPercentage已在0-100范围内）
            double percentage = PwmPercentage / 100.0;
            if (percentage > 1.0) percentage = 1.0;

            // 使用裁剪方法绘制进度弧
            double progressEndAngle = startAngle + (angleRange * percentage);
            double prossRadius = backRadius - (((Thickness * 1.8) / 2) - (Thickness / 2));

            RadialGradientBrush gradientBrush = new RadialGradientBrush
            {
                Center = new Point(0.5, 0.5),
                GradientOrigin = new Point(0.5, 0.5),
                RadiusX = 0.5,
                RadiusY = 0.5
            };
            gradientBrush.GradientStops.Add(new GradientStop(Colors.Red, (prossRadius - Thickness) / prossRadius));
            gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(15, 00, 00), 1.0));

            DrawArcByClipping(drawingContext, centerX, centerY, prossRadius, startAngle, progressEndAngle, Thickness,
                  gradientBrush);

            // 绘制内弧
            DrawArc(drawingContext, centerX, centerY, backRadius - (((Thickness * 1.8) / 2) - (2 / 2)) - 4, startAngle, endAngle, 2,
                    new SolidColorBrush(Color.FromRgb(53, 59, 71)));

            // 绘制刻度和文本标识
            DrawTicks(drawingContext, centerX, centerY, backRadius, startAngle, angleRange, new SolidColorBrush(Color.FromRgb(139, 139, 139)));
            DrawLabels(drawingContext, centerX, centerY, backRadius, startAngle, angleRange, new SolidColorBrush(Color.FromRgb(139, 139, 139)));
        }

        // 绘制弧线
        private void DrawArc(DrawingContext dc, double centerX, double centerY, double radius,
                           double startAngle, double endAngle, double thickness, Brush brush)
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
                IsFilled = false,
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
                RotationAngle = 0,
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
            dc.DrawGeometry(null, pen, pathGeometry);
        }

        /// <summary>
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

        // 绘制刻度和标签
        private void DrawTicks(DrawingContext dc, double centerX, double centerY,
                                       double radius, double startAngle, double angleRange, Brush brush)
        {
            int tickStep = 10; // 每20%一个刻度
            for (int i = 0; i <= 100; i += tickStep)
            {
                double tickAngle = startAngle + (angleRange * i / 100);

                // 绘制刻度线
                double rad = (tickAngle - 90) * Math.PI / 180.0;
                double innerRadius = radius - Thickness * 0.5;
                double outerRadius = radius;

                Point innerPoint = new Point(
                    centerX + innerRadius * Math.Cos(rad),
                    centerY + innerRadius * Math.Sin(rad));

                Point outerPoint = new Point(
                    centerX + outerRadius * Math.Cos(rad),
                    centerY + outerRadius * Math.Sin(rad));

                dc.DrawLine(new Pen(brush, Thickness * 0.1), innerPoint, outerPoint);
            }
        }
        private void DrawLabels(DrawingContext dc, double centerX, double centerY,
                                       double radius, double startAngle, double angleRange, Brush brush)
        {
            int tickStep = 20; // 每20%一个刻度
            for (int i = 0; i <= 100; i += tickStep)
            {
                double tickAngle = startAngle + (angleRange * i / 100);
                double rad = (tickAngle - 90) * Math.PI / 180.0;
                // 绘制百分比文本（跳过0%和100%，避免与其他控件重叠）
                if (i >= 0 && i <= 100)
                {
                    double textRadius = radius + Thickness * 1;
                    Point textPoint = new Point(
                        centerX + textRadius * Math.Cos(rad),
                        centerY + textRadius * Math.Sin(rad));


                    FormattedText formattedText = new FormattedText(
                        $"{i}%",
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Arial"),
                        12,
                        brush);

                    if (i == 0)
                    {
                        formattedText = new FormattedText(
                        $"{i}",
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Arial"),
                        12,
                        brush);
                    }

                    // 调整文本位置使其居中
                    textPoint.X -= formattedText.Width / 2;
                    textPoint.Y -= formattedText.Height / 2;

                    dc.DrawText(formattedText, textPoint);
                }
            }
        }
    }
}
