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
    /// 中间270°弧形进度条，用于显示CPU频率
    /// </summary>
    public class FrequencyGauge : Control
    {
        // 依赖属性定义
        public static readonly DependencyProperty FrequencyProperty = DependencyProperty.Register(
            "Frequency", typeof(double), typeof(FrequencyGauge), new PropertyMetadata(3000.0, OnPropertyChanged));

        public static readonly DependencyProperty MaxFrequencyProperty = DependencyProperty.Register(
            "MaxFrequency", typeof(double), typeof(FrequencyGauge), new PropertyMetadata(5000.0));

        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(
            "Radius", typeof(double), typeof(FrequencyGauge), new PropertyMetadata(100.0, OnPropertyChanged));

        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(
            "Thickness", typeof(double), typeof(FrequencyGauge), new PropertyMetadata(15.0, OnPropertyChanged));

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
        public FrequencyGauge()
        {
            this.SizeChanged += FrequencyGauge_SizeChanged;
        }

        private void FrequencyGauge_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.InvalidateVisual();
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((FrequencyGauge)d).InvalidateVisual();
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

            // 绘制背景弧
            DrawArc(drawingContext, centerX, centerY, actualRadius, startAngle, endAngle, Thickness * 2.25,
                    new SolidColorBrush(Color.FromRgb(53, 59, 71)));

            // 计算进度百分比
            double percentage = Frequency / MaxFrequency;
            if (percentage > 1.0) percentage = 1.0;

            // 绘制进度弧
            double progressEndAngle = startAngle + (angleRange * percentage);
            DrawArc(drawingContext, centerX, centerY, actualRadius, startAngle, progressEndAngle, Thickness,
                    new SolidColorBrush(Color.FromRgb(255, 0, 0)), new double[] { 0.5d, 0.05d });

            //// 绘制中心文本
            //DrawCenterText(drawingContext, centerX, centerY, Frequency);
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

        // 绘制中心文本
        private void DrawCenterText(DrawingContext dc, double centerX, double centerY, double frequency)
        {
            // 绘制CPU标签
            FormattedText cpuText = new FormattedText(
                "CPU",
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal),
                24,
                Brushes.White);

            Point cpuTextPoint = new Point(centerX - cpuText.Width / 2, centerY - 30);
            dc.DrawText(cpuText, cpuTextPoint);

            // 绘制频率值
            FormattedText freqText = new FormattedText(
                $"{frequency:N0}",
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.ExtraBold, FontStretches.Normal),
                48,
                new SolidColorBrush(Color.FromRgb(255, 255, 255)));

            Point freqTextPoint = new Point(centerX - freqText.Width / 2, centerY + 10);
            dc.DrawText(freqText, freqTextPoint);

            // 绘制MHz单位
            FormattedText unitText = new FormattedText(
                "MHz",
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"),
                16,
                Brushes.LightGray);

            Point unitTextPoint = new Point(centerX - unitText.Width / 2, centerY + 60);
            dc.DrawText(unitText, unitTextPoint);
        }
    }
}
