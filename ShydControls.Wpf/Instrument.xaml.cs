using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShydControls.Wpf
{
    /// <summary>
    /// Instrument.xaml 的交互逻辑
    /// </summary>
    public partial class Instrument : UserControl
    {
        public Instrument()
        {
            InitializeComponent();
            this.Loaded += Instrument_Loaded;
           
        }


        public float Value
        {
            get { return (float)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(float), typeof(Instrument), new PropertyMetadata(0f,new PropertyChangedCallback(OnValueChanged)));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Instrument instrument = d as Instrument;
            if (instrument != null)
            {
                //instrument.rtPointer.Angle = 270f / (100) * Convert.ToSingle(e.NewValue) - 45;
                DoubleAnimation doubleAnimation = new DoubleAnimation(270f / (instrument.Maximum - instrument.Minimum) * (Convert.ToSingle(e.NewValue) - instrument.Minimum) - 45,new Duration(TimeSpan.FromMilliseconds(200)));

                instrument.rtPointer.BeginAnimation(RotateTransform.AngleProperty, doubleAnimation);
            }
        }


        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Maximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(int), typeof(Instrument), new PropertyMetadata(100,new PropertyChangedCallback(OnMaximumChanged)));

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Instrument instrument = d as Instrument;
            instrument?.Draw();          
        }


        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(int), typeof(Instrument), new PropertyMetadata(0,new PropertyChangedCallback(OnMinimumChanged)));

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Instrument instrument = d as Instrument;
            instrument?.Draw();
        }


        public Brush PlateBackground
        {
            get { return (Brush)GetValue(PlateBackgroundProperty); }
            set { SetValue(PlateBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlateBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlateBackgroundProperty =
            DependencyProperty.Register("PlateBackground", typeof(Brush), typeof(Instrument), new PropertyMetadata(Brushes.Orange));


        public Brush ScaleBrush
        {
            get { return (Brush)GetValue(ScaleBrushProperty); }
            set { SetValue(ScaleBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScaleBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScaleBrushProperty =
            DependencyProperty.Register("ScaleBrush", typeof(Brush), typeof(Instrument), new PropertyMetadata(Brushes.White,new PropertyChangedCallback(OnScaleBrushChanged)));

        private static void OnScaleBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Instrument instrument = d as Instrument;
            instrument?.Draw();
        }

        public int Interval
        {
            get { return (int)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Interval.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(int), typeof(Instrument), new PropertyMetadata(10,new PropertyChangedCallback(OnIntervalChanged)));

        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Instrument instrument = d as Instrument;
            instrument?.Draw();
        }

        private void Instrument_Loaded(object sender, RoutedEventArgs e)
        {
            Draw();
        }
        private void Draw()
        {
            double radius = this.backEllipse.ActualWidth / 2;
            double min = this.Minimum; double max = this.Maximum;
            if (max <= min)
                return;
            double step = 270.0 / (max - min);
            double oneAngle = Math.PI / 180;
            int scaleCount = this.Interval;
            this.mainCanvas.Children.Clear();
            for (int i = 0; i <= max - min; i++)
            {
                Line line = new Line();
                line.X1 = radius - radius * Math.Cos((i * step - 45) * oneAngle);
                line.Y1 = radius - radius * Math.Sin((i * step - 45) * oneAngle);
                if (i % scaleCount == 0)
                {
                    line.X2 = radius - (radius - 20) * Math.Cos((i * step - 45) * oneAngle);
                    line.Y2 = radius - (radius - 20) * Math.Sin((i * step - 45) * oneAngle);
                    TextBlock text = new TextBlock();
                    text.Text = (min + i).ToString();
                    text.Foreground = ScaleBrush;
                    text.Background = Brushes.Transparent;
                    text.TextAlignment = TextAlignment.Center;
                    text.FontSize = 15;
                    text.Width = 30;
                    Canvas.SetLeft(text, radius - (radius - 35) * Math.Cos((i * step - 45) * oneAngle) - 15);//-15为字体宽度的一半
                    Canvas.SetTop(text, radius - (radius - 35) * Math.Sin((i * step - 45) * oneAngle) - 7.5);//-7.5为字体size的一半
                    this.mainCanvas.Children.Add(text);
                }
                else
                {
                    line.X2 = radius - (radius - 10) * Math.Cos((i * step - 45) * oneAngle);
                    line.Y2 = radius - (radius - 10) * Math.Sin((i * step - 45) * oneAngle);
                }
                line.Stroke = ScaleBrush;
                line.StrokeThickness = 1;
                this.mainCanvas.Children.Add(line);
            }
            string pathDataTe = "M{0} {1} A{2} {2} 0 1 1 {3} {1}";//M起点 A宽高 0 1 1 终点
            pathDataTe = string.Format(pathDataTe, radius - (radius * 0.4) * Math.Cos(-45 * oneAngle), radius - (radius * 0.4) * Math.Sin(-45 * oneAngle), radius * 0.4, radius - (radius * 0.4) * Math.Cos(225 * oneAngle));
            //var converter = TypeDescriptor.GetConverter(typeof(Geometry));
            GeometryConverter geometryConverter = new GeometryConverter();
            this.circle.Data = geometryConverter.ConvertFromString(pathDataTe) as Geometry;

            pathDataTe = "M{0} {1},{1} {2},{1} {3}";//3个点
            pathDataTe = string.Format(pathDataTe, radius * 0.5, radius, 1.05 * radius, 0.95 * radius);
            //var converter = TypeDescriptor.GetConverter(typeof(Geometry));

            this.pointer.Data = geometryConverter.ConvertFromString(pathDataTe) as Geometry;
        }
        private void Instrument_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //backEllipse.Width = Math.Min(this.RenderSize.Width,this.RenderSize.Height);
            backEllipse.Width = Math.Min(this.ActualWidth, this.ActualHeight);
            backEllipse.Height = backEllipse.Width;
            Draw();
        }
    }
}
