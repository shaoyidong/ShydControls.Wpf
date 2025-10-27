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
    /// 带指针的DRAM频率仪表盘
    /// </summary>
    public class DRAMFrequencyGauge : Control
    {
        // 依赖属性定义
        public static readonly DependencyProperty FrequencyProperty = DependencyProperty.Register(
            "Frequency", typeof(double), typeof(DRAMFrequencyGauge), new PropertyMetadata(3600.0, OnPropertyChanged));

        public static readonly DependencyProperty MaxFrequencyProperty = DependencyProperty.Register(
            "MaxFrequency", typeof(double), typeof(DRAMFrequencyGauge), new PropertyMetadata(6000.0));

        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(
            "Radius", typeof(double), typeof(DRAMFrequencyGauge), new PropertyMetadata(150.0, OnPropertyChanged));

        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(
           "Thickness", typeof(double), typeof(DRAMFrequencyGauge), new PropertyMetadata(15.0, OnPropertyChanged));

        // 属性访问器
        public double Frequency
        {
            get { return (double)GetValue(FrequencyProperty); }
            set { SetValue(FrequencyProperty, value); }
        }

        public double MaxFrequency
        {
            get { return (double)GetValue(MaxFrequencyProperty); }
            set { SetValue(MaxFrequencyProperty, value); }
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
        public DRAMFrequencyGauge()
        {
            this.SizeChanged += DRAMFrequencyGauge_SizeChanged;
        }

        private void DRAMFrequencyGauge_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.InvalidateVisual();
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DRAMFrequencyGauge)d).InvalidateVisual();
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
            double startAngle = 270; // 从135°开始
            double angleRange = 180; // 270°范围
            double endAngle = startAngle + angleRange;

            //// 绘制外弧
            //DrawArc(drawingContext, centerX, centerY, actualRadius, startAngle, endAngle, 2,
            //        new SolidColorBrush(Color.FromRgb(60, 60, 60)));


            // 绘制背景弧
            //double backRadius = actualRadius - ((2 / 2) + Thickness * 1.8 / 2) - Thickness * 0.5;
            DrawArc(drawingContext, centerX, centerY, actualRadius, startAngle, endAngle, Thickness,
                    new SolidColorBrush(Color.FromRgb(53, 59, 71)));

            // 绘制刻度
            DrawTicks(drawingContext, centerX, centerY, actualRadius, startAngle, angleRange, new SolidColorBrush(Color.FromRgb(139, 139, 139)), Thickness);

            // 计算进度百分比（PwmPercentage已在0-100范围内）
            double percentage = Frequency / MaxFrequency;
            if (percentage > 1.0) percentage = 1.0;

            // 绘制进度弧
            double progressEndAngle = startAngle + (angleRange * percentage);
            double prossRadius = actualRadius - (Thickness / 2);

            DrawArc(drawingContext, centerX, centerY, prossRadius, startAngle - 0.3, progressEndAngle + 0.3, Thickness,
                   new SolidColorBrush(Color.FromRgb(255, 00, 00)));

            DrawArc(drawingContext, centerX, centerY, prossRadius - Thickness * 2.2 / 2, startAngle - 0.3, progressEndAngle + 0.3, Thickness * 2.2,
                  new SolidColorBrush(Color.FromArgb(127, 60, 60, 60)));

            //绘制文本
            DrawLabels(drawingContext, centerX, centerY, prossRadius - Thickness * 2, startAngle, angleRange, new SolidColorBrush(Color.FromRgb(139, 139, 139)));

            //绘制指针
            DrawPointer(drawingContext, centerX, centerY, actualRadius * 0.55, startAngle, endAngle);
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

        // 绘制刻度和标签
        private void DrawTicks(DrawingContext dc, double centerX, double centerY,
                                       double radius, double startAngle, double angleRange, Brush brush, double thickness)
        {
            int tickStep = 125; // 每20%一个刻度
            for (int i = 0; i <= 1000; i += tickStep)
            {
                Pen pen;
                if (i <= Frequency / MaxFrequency * 1000)
                {
                    pen = new Pen(new SolidColorBrush(Colors.Red), 2);
                }
                else
                {
                    pen = new Pen(brush, 2);
                }
                double tickAngle = startAngle + (angleRange * i / 1000);

                // 绘制刻度线
                double rad = (tickAngle - 90) * Math.PI / 180.0;
                double innerRadius = radius - thickness * 0.5;
                double outerRadius = radius + thickness * 0.5;

                Point innerPoint = new Point(
                    centerX + innerRadius * Math.Cos(rad),
                    centerY + innerRadius * Math.Sin(rad));

                Point outerPoint = new Point(
                    centerX + outerRadius * Math.Cos(rad),
                    centerY + outerRadius * Math.Sin(rad));

                dc.DrawLine(pen, innerPoint, outerPoint);
            }
        }
        private void DrawLabels(DrawingContext dc, double centerX, double centerY,
                                       double radius, double startAngle, double angleRange, Brush brush)
        {
            int tickStep = 25; // 每20%一个刻度
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
                        $"{i}",
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Arial"),
                        13,
                        brush);

                    // 调整文本位置使其居中
                    textPoint.X -= formattedText.Width / 2;
                    textPoint.Y -= formattedText.Height / 2;

                    dc.DrawText(formattedText, textPoint);
                }
            }
        }

        // 绘制指针
        private void DrawPointer(DrawingContext dc, double centerX, double centerY, double radius, double startAngle, double endAngle)
        {
            //double startAngle = 135; // 开始角度（左侧）
            //double endAngle = 45;    // 结束角度（右侧）
            double angleRange = endAngle - startAngle; // 计算总角度范围

            // 计算指针角度
            double percentage = Frequency / MaxFrequency;
            if (percentage > 1.0) percentage = 1.0;
            if (percentage < 0.0) percentage = 0.0;

            double pointerAngle = startAngle + (angleRange * percentage);
            double pointerRad = (pointerAngle - 90) * Math.PI / 180.0;

            // 指针长度（半径的80%）
            double pointerLength = radius * 0.8;

            double pointerWidth = 10; // 指针宽度    

            // 计算扇形指针的三个顶点
            Point pointerTip = new Point(
                centerX + pointerLength * Math.Cos(pointerRad),
                centerY + pointerLength * Math.Sin(pointerRad));

            Point pointerLeft = new Point(
                centerX + pointerWidth * 0.5 * Math.Cos(pointerRad - Math.PI / 2),
                centerY + pointerWidth * 0.5 * Math.Sin(pointerRad - Math.PI / 2));

            Point pointerRight = new Point(
                centerX + pointerWidth * 0.5 * Math.Cos(pointerRad + Math.PI / 2),
                centerY + pointerWidth * 0.5 * Math.Sin(pointerRad + Math.PI / 2));

            // 创建指针形状
            StreamGeometry pointerGeometry = new StreamGeometry();
            using (StreamGeometryContext ctx = pointerGeometry.Open())
            {
                ctx.BeginFigure(pointerTip, true, true);
                ctx.LineTo(pointerLeft, true, false);
                ctx.LineTo(pointerRight, true, false);
            }

            // 绘制指针
            dc.DrawGeometry(new SolidColorBrush(Color.FromRgb(255, 0, 0)), null, pointerGeometry);

            // 绘制中心点
            dc.DrawEllipse(new SolidColorBrush(Color.FromRgb(255, 0, 0)),
                           new Pen(new SolidColorBrush(Color.FromRgb(180, 0, 0)), 0),
                           new Point(centerX, centerY), 5, 5);
        }

        // 绘制中心文本
        private void DrawCenterText(DrawingContext dc, double centerX, double centerY)
        {
            // 绘制DRAM标签
            FormattedText dramText = new FormattedText(
                "DRAM",
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"),
                18,
                new SolidColorBrush(Color.FromRgb(200, 200, 200)));

            Point dramTextPoint = new Point(centerX - dramText.Width / 2, centerY - 15);
            dc.DrawText(dramText, dramTextPoint);

            // 绘制频率值
            FormattedText freqText = new FormattedText(
                $"{Frequency:N0}",
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"),
                24,
                new SolidColorBrush(Color.FromRgb(255, 255, 255)));

            Point freqTextPoint = new Point(centerX - freqText.Width / 2, centerY + 5);
            dc.DrawText(freqText, freqTextPoint);

            // 绘制MHz单位
            FormattedText unitText = new FormattedText(
                "MHz",
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"),
                14,
                new SolidColorBrush(Color.FromRgb(150, 150, 150)));

            Point unitTextPoint = new Point(centerX - unitText.Width / 2, centerY + 35);
            dc.DrawText(unitText, unitTextPoint);
        }
    }
}
