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
    public class FanSpeedGauge : Control
    {
        // 依赖属性定义
        public static readonly DependencyProperty FanSpeedProperty = DependencyProperty.Register(
            "FanSpeed", typeof(double), typeof(FanSpeedGauge), new PropertyMetadata(2000.0, OnPropertyChanged));

        public static readonly DependencyProperty MaxFanSpeedProperty = DependencyProperty.Register(
            "MaxFanSpeed", typeof(double), typeof(FanSpeedGauge), new PropertyMetadata(5000.0));

        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(
            "Radius", typeof(double), typeof(FanSpeedGauge), new PropertyMetadata(60.0, OnPropertyChanged));

        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(
            "Thickness", typeof(double), typeof(FanSpeedGauge), new PropertyMetadata(8.0, OnPropertyChanged));

        public static readonly DependencyProperty StartAngleProperty = DependencyProperty.Register(
            "StartAngle", typeof(double), typeof(FanSpeedGauge), new PropertyMetadata(315.0, OnPropertyChanged));

        public static readonly DependencyProperty AngleRangeProperty = DependencyProperty.Register(
            "AngleRange", typeof(double), typeof(FanSpeedGauge), new PropertyMetadata(90.0, OnPropertyChanged));

        // 属性访问器
        public double FanSpeed
        {
            get { return (double)GetValue(FanSpeedProperty); }
            set { SetValue(FanSpeedProperty, value); }
        }

        public double MaxFanSpeed
        {
            get { return (double)GetValue(MaxFanSpeedProperty); }
            set { SetValue(MaxFanSpeedProperty, value); }
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
        public FanSpeedGauge()
        {
            this.SizeChanged += FanSpeedGauge_SizeChanged;
        }

        private void FanSpeedGauge_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.InvalidateVisual();
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((FanSpeedGauge)d).InvalidateVisual();
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

            // 绘制背景弧
            DrawArc(drawingContext, centerX, centerY, actualRadius, StartAngle, endAngle, Thickness,
                    new SolidColorBrush(Color.FromRgb(53, 59, 71)));

            // 计算进度百分比
            double percentage = FanSpeed / MaxFanSpeed;
            if (percentage > 1.0) percentage = 1.0;

            // 绘制进度弧
            double progressEndAngle = StartAngle + (AngleRange * percentage);
            DrawArc(drawingContext, centerX, centerY, actualRadius, StartAngle, progressEndAngle, Thickness,
                    new SolidColorBrush(Color.FromRgb(255, 0, 0)));

            // 绘制风扇转速文本
            //double textAngle = StartAngle + AngleRange / 2; // 文本居中显示
            //DrawFanSpeedText(drawingContext, centerX, centerY, actualRadius + Thickness * 2, textAngle, FanSpeed);
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
            dc.DrawGeometry(null, pen, pathGeometry);
        }

        // 绘制风扇转速文本
        private void DrawFanSpeedText(DrawingContext dc, double centerX, double centerY,
                                    double radius, double angle, double fanSpeed)
        {
            double rad = (angle - 90) * Math.PI / 180.0;
            Point textPoint = new Point(
                centerX + radius * Math.Cos(rad),
                centerY + radius * Math.Sin(rad));

            FormattedText formattedText = new FormattedText(
                $"{fanSpeed:N0} RPM",
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
    }
}
