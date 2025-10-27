using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.ComponentModel;

namespace ShydControls.Wpf.Charts
{
    /// <summary>
    /// 五维图表控件，用于展示CPU、GPU、散热、静音、节能五个维度的数据
    /// 使用WPF内置动画系统实现流畅动画效果
    /// </summary>
    public partial class PentagonChart : Control
    {
        #region 依赖属性定义
        
        // 定义五个维度的依赖属性
        public static readonly DependencyProperty OneValueProperty = DependencyProperty.Register(
            "OneValue", typeof(double), typeof(PentagonChart), new PropertyMetadata(70.0, OnPropertyChanged));

        public static readonly DependencyProperty TwoValueProperty = DependencyProperty.Register(
            "TwoValue", typeof(double), typeof(PentagonChart), new PropertyMetadata(60.0, OnPropertyChanged));

        public static readonly DependencyProperty FiveValueProperty = DependencyProperty.Register(
            "FiveValue", typeof(double), typeof(PentagonChart), new PropertyMetadata(80.0, OnPropertyChanged));

        public static readonly DependencyProperty ThreeValueProperty = DependencyProperty.Register(
            "ThreeValue", typeof(double), typeof(PentagonChart), new PropertyMetadata(50.0, OnPropertyChanged));

        public static readonly DependencyProperty FourValueProperty = DependencyProperty.Register(
            "FourValue", typeof(double), typeof(PentagonChart), new PropertyMetadata(40.0, OnPropertyChanged));

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            "MaxValue", typeof(double), typeof(PentagonChart), new PropertyMetadata(100.0, OnPropertyChanged));

        public static readonly DependencyProperty BackgroundColorProperty = DependencyProperty.Register(
            "BackgroundColor", typeof(Color), typeof(PentagonChart), new PropertyMetadata(Color.FromRgb(53, 59, 71), OnPropertyChanged));

        public static readonly DependencyProperty LineColorProperty = DependencyProperty.Register(
            "LineColor", typeof(Color), typeof(PentagonChart), new PropertyMetadata(Color.FromRgb(200, 50, 80), OnPropertyChanged));

        public static readonly DependencyProperty AnimationEnabledProperty = DependencyProperty.Register(
            "AnimationEnabled", typeof(bool), typeof(PentagonChart), new PropertyMetadata(true));
            
        /// <summary>
        /// 动画进度依赖属性，用于驱动WPF内置动画系统
        /// </summary>
        protected static readonly DependencyProperty AnimatedProgressProperty = 
            DependencyProperty.Register("AnimatedProgress", typeof(double), typeof(PentagonChart), 
                new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender,new PropertyChangedCallback(OnAnimatedProgressChanged)));
        
        #endregion

        #region 属性访问器
        
        public double OneValue
        { get { return (double)GetValue(OneValueProperty); } set { SetValue(OneValueProperty, value); } }

        public double TwoValue
        { get { return (double)GetValue(TwoValueProperty); } set { SetValue(TwoValueProperty, value); } }

        public double FiveValue
        { get { return (double)GetValue(FiveValueProperty); } set { SetValue(FiveValueProperty, value); } }

        public double ThreeValue
        { get { return (double)GetValue(ThreeValueProperty); } set { SetValue(ThreeValueProperty, value); } }

        public double FourValue
        { get { return (double)GetValue(FourValueProperty); } set { SetValue(FourValueProperty, value); } }

        public double MaxValue
        { get { return (double)GetValue(MaxValueProperty); } set { SetValue(MaxValueProperty, value); } }

        public Color BackgroundColor
        { get { return (Color)GetValue(BackgroundColorProperty); } set { SetValue(BackgroundColorProperty, value); } }

        public Color LineColor
        { get { return (Color)GetValue(LineColorProperty); } set { SetValue(LineColorProperty, value); } }

        public bool AnimationEnabled
        { get { return (bool)GetValue(AnimationEnabledProperty); } set { SetValue(AnimationEnabledProperty, value); } }
            
        protected double AnimatedProgress
        { get { return (double)GetValue(AnimatedProgressProperty); } set { SetValue(AnimatedProgressProperty, value); } }
        
        #endregion

        #region 私有字段
        
        // 当前实际动画值（用于渲染）
        private double[] _animatedValues = new double[5];
        
        // 标记控件是否已加载
        private bool _isLoaded = false;
        
        // 缓存笔刷以提高性能
        private LinearGradientBrush _backgroundGradientBrush;
        private LinearGradientBrush _valueGradientBrush;
        private SolidColorBrush _backgroundStrokeBrush;
        private SolidColorBrush _valueStrokeBrush;
        private SolidColorBrush _pointFillBrush;
        
        // 用于动画的目标值数组
        private double[] _targetValues = new double[5];
        
        #endregion

        #region 构造函数与初始化
        
        /// <summary>
        /// 构造函数，初始化控件并设置事件监听
        /// </summary>
        public PentagonChart()
        {
            this.SizeChanged += PentagonChart_SizeChanged;
            this.Loaded += PentagonChart_Loaded;
            
            // 初始化缓存的笔刷
            InitializeBrushes();
            
            //// 添加AnimatedProgress属性变更事件处理
            //DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(AnimatedProgressProperty, typeof(PentagonChart));
            //dpd.AddValueChanged(this, OnAnimatedProgressChanged);
        }
        
        /// <summary>
        /// 初始化缓存的笔刷资源
        /// </summary>
        private void InitializeBrushes()
        {
            // 创建缓存的笔刷实例，避免每次绘制都创建新对象
            _backgroundStrokeBrush = new SolidColorBrush(BackgroundColor);
            _valueStrokeBrush = new SolidColorBrush(LineColor);
            _pointFillBrush = Brushes.Red;
            
            // 背景渐变笔刷
            _backgroundGradientBrush = new LinearGradientBrush
            {
                StartPoint = new Point(0.5, 1),
                EndPoint = new Point(0.5, 0),
                GradientStops = new GradientStopCollection()
                {
                    new GradientStop(Color.FromArgb(180, BackgroundColor.R, BackgroundColor.G, BackgroundColor.B), 0.0),
                    new GradientStop(Color.FromArgb(80, BackgroundColor.R, BackgroundColor.G, BackgroundColor.B), 0.5),
                    new GradientStop(Color.FromArgb(30, BackgroundColor.R, BackgroundColor.G, BackgroundColor.B), 1.0)
                }
            };
            
            // 值区域渐变笔刷
            _valueGradientBrush = new LinearGradientBrush
            {
                StartPoint = new Point(0.5, 1),
                EndPoint = new Point(0.5, 0),
                GradientStops = new GradientStopCollection()
                {
                    new GradientStop(Color.FromArgb(180, LineColor.R, LineColor.G, LineColor.B), 0.0),
                    new GradientStop(Color.FromArgb(80, LineColor.R, LineColor.G, LineColor.B), 0.5),
                    new GradientStop(Color.FromArgb(30, LineColor.R, LineColor.G, LineColor.B), 1.0)
                }
            };
        }
        
        /// <summary>
        /// 更新笔刷颜色
        /// </summary>
        private void UpdateBrushes()
        {
            // 更新笔刷颜色，避免重新创建笔刷对象
            _backgroundStrokeBrush.Color = BackgroundColor;
            _valueStrokeBrush.Color = LineColor;
            
            // 更新背景渐变笔刷颜色
            _backgroundGradientBrush.GradientStops[0].Color = Color.FromArgb(180, BackgroundColor.R, BackgroundColor.G, BackgroundColor.B);
            _backgroundGradientBrush.GradientStops[1].Color = Color.FromArgb(80, BackgroundColor.R, BackgroundColor.G, BackgroundColor.B);
            _backgroundGradientBrush.GradientStops[2].Color = Color.FromArgb(30, BackgroundColor.R, BackgroundColor.G, BackgroundColor.B);
            
            // 更新值区域渐变笔刷颜色
            _valueGradientBrush.GradientStops[0].Color = Color.FromArgb(180, LineColor.R, LineColor.G, LineColor.B);
            _valueGradientBrush.GradientStops[1].Color = Color.FromArgb(80, LineColor.R, LineColor.G, LineColor.B);
            _valueGradientBrush.GradientStops[2].Color = Color.FromArgb(30, LineColor.R, LineColor.G, LineColor.B);
        }
        
        #endregion

        #region 事件处理
        
        ///// <summary>
        ///// 当AnimatedProgress属性变化时更新动画值
        ///// </summary>
        //private void OnAnimatedProgressChanged(object sender, EventArgs e)
        //{
        //    // 直接使用动画系统提供的进度值，不再需要自定义缓动函数
        //    // 因为我们已经在StartAnimation中使用了内置的CubicEase缓动函数
        //    double progress = AnimatedProgress;
            
        //    // 根据当前动画进度计算动画值
        //    _animatedValues[0] = progress * _targetValues[0];
        //    _animatedValues[1] = progress * _targetValues[1];
        //    _animatedValues[2] = progress * _targetValues[2];
        //    _animatedValues[3] = progress * _targetValues[3];
        //    _animatedValues[4] = progress * _targetValues[4];
            
        //    // 触发重绘
        //   // this.InvalidateVisual();
        //}

        /// <summary>
        /// 当AnimatedProgress属性变化时更新动画值
        /// </summary>
        private static void OnAnimatedProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PentagonChart chart = (PentagonChart)d;
            // 直接使用动画系统提供的进度值
            double progress = (double)e.NewValue;

            // 根据当前动画进度计算动画值
            chart._animatedValues[0] = progress * chart._targetValues[0];
            chart._animatedValues[1] = progress * chart._targetValues[1];
            chart._animatedValues[2] = progress * chart._targetValues[2];
            chart._animatedValues[3] = progress * chart._targetValues[3];
            chart._animatedValues[4] = progress * chart._targetValues[4];

            // 无需调用InvalidateVisual，因为AnimatedProgressProperty已设置FrameworkPropertyMetadataOptions.AffectsRender
        }

        /// <summary>
        /// 控件加载完成时的事件处理
        /// </summary>
        private void PentagonChart_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
            
            // 初始化目标值和动画值
            UpdateTargetValues();
            
            // 控件加载完成后，如果启用了动画，启动动画
            if (AnimationEnabled)
            {
                StartAnimation();
            }
            else
            {
                // 如果不启用动画，直接设置动画值为目标值
                _animatedValues[0] = _targetValues[0];
                _animatedValues[1] = _targetValues[1];
                _animatedValues[2] = _targetValues[2];
                _animatedValues[3] = _targetValues[3];
                _animatedValues[4] = _targetValues[4];
                this.InvalidateVisual();
            }
        }
        
        /// <summary>
        /// 控件大小变化时重绘
        /// </summary>
        private void PentagonChart_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.InvalidateVisual();
        }
        
        /// <summary>
        /// 依赖属性变化时的处理
        /// </summary>
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chart = (PentagonChart)d;
            
            // 如果是颜色属性变化，更新缓存的笔刷
            if (e.Property == BackgroundColorProperty || e.Property == LineColorProperty)
            {
                chart.UpdateBrushes();
            }
            
            // 如果不启用动画或控件未加载，直接更新动画值
            if (!chart.AnimationEnabled || !chart._isLoaded)
            {
                if (e.Property == OneValueProperty) chart._animatedValues[0] = chart.OneValue;
                else if (e.Property == TwoValueProperty) chart._animatedValues[1] = chart.TwoValue;
                else if (e.Property == ThreeValueProperty) chart._animatedValues[2] = chart.ThreeValue;
                else if (e.Property == FourValueProperty) chart._animatedValues[3] = chart.FourValue;
                else if (e.Property == FiveValueProperty) chart._animatedValues[4] = chart.FiveValue;
            }
            
            // 如果是值属性变化，更新目标值
            if (e.Property == OneValueProperty || e.Property == TwoValueProperty || 
                e.Property == ThreeValueProperty || e.Property == FourValueProperty || 
                e.Property == FiveValueProperty)
            {
                chart.UpdateTargetValues();
            }
            
            chart.InvalidateVisual();

            // 如果启用了动画且控件已加载，启动动画
            if (chart.AnimationEnabled && chart._isLoaded &&
                (e.Property == OneValueProperty || e.Property == TwoValueProperty || 
                 e.Property == ThreeValueProperty || e.Property == FourValueProperty || 
                 e.Property == FiveValueProperty))
            {
                chart.StartAnimation();
            }
        }
        
        #endregion

        #region 动画方法
        
        /// <summary>
        /// 更新动画目标值
        /// </summary>
        private void UpdateTargetValues()
        {
            _targetValues[0] = OneValue;
            _targetValues[1] = TwoValue;
            _targetValues[2] = ThreeValue;
            _targetValues[3] = FourValue;
            _targetValues[4] = FiveValue;
        }
        
        /// <summary>
        /// 启动WPF内置动画系统的动画
        /// </summary>
        private void StartAnimation()
        {
            // 停止之前可能正在运行的动画
            Storyboard sb = Resources["AnimateProgress"] as Storyboard;
            if (sb != null)
            {
                sb.Stop();
                Resources.Remove("AnimateProgress");
            }
            
            // 创建新的Storyboard动画
            sb = new Storyboard();
            
            // 创建DoubleAnimation动画
            DoubleAnimation animation = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = new Duration(TimeSpan.FromMilliseconds(500)), // 0.5秒完成动画
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut } // 使用内置的缓动函数
            };
            
            // 设置动画目标
            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, new PropertyPath(AnimatedProgressProperty));
            
            // 添加到Storyboard
            sb.Children.Add(animation);
            
            // 存储Storyboard以便稍后可以停止
            Resources["AnimateProgress"] = sb;
            
            // 开始动画
            sb.Begin();
        }
        
        #endregion

        #region 绘制方法
        
        /// <summary>
        /// 重写OnRender方法进行绘制
        /// </summary>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            double width = ActualWidth;
            double height = ActualHeight;
            double centerX = width / 2;
            double centerY = height / 2;
            double radius = Math.Min(width, height) * 0.45; // 半径为控件最小边长的45%

            // 绘制五边形
            DrawPentagon(drawingContext, centerX, centerY, radius);
        }

        /// <summary>
        /// 绘制五边形图表的核心方法
        /// </summary>
        private void DrawPentagon(DrawingContext drawingContext, double centerX, double centerY, double radius)
        {
            int sides = 5;
            Point[] points = new Point[sides];
            Point[] valuePoints = new Point[sides];

            // 计算五个顶点的坐标
            for (int i = 0; i < sides; i++)
            {
                double angle = 90 - i * (360.0 / sides); // 从顶部开始，顺时针方向
                double radians = angle * Math.PI / 180;

                // 计算五边形的顶点坐标
                double x = centerX + radius * Math.Cos(radians);
                double y = centerY - radius * Math.Sin(radians);
                points[i] = new Point(x, y);

                // 计算当前值对应的点坐标（根据当前值缩放）
                double value = _animatedValues[i];
                double scaledRadius = radius * (Math.Min(value, MaxValue) / MaxValue);
                double valueX = centerX + scaledRadius * Math.Cos(radians);
                double valueY = centerY - scaledRadius * Math.Sin(radians);
                valuePoints[i] = new Point(valueX, valueY);
            }

            // 绘制背景五边形 - 使用PathGeometry直接绘制几何图形以提高性能
            PathFigure backgroundFigure = new PathFigure(points[0], points.Skip(1).Select(p => new LineSegment(p, true)).ToArray(), true);
            PathGeometry backgroundGeometry = new PathGeometry(new PathFigure[] { backgroundFigure });
            drawingContext.DrawGeometry(_backgroundGradientBrush, new Pen(_backgroundStrokeBrush, 1), backgroundGeometry);

            // 绘制值区域 - 使用PathGeometry直接绘制几何图形以提高性能
            PathFigure valueFigure = new PathFigure(valuePoints[0], valuePoints.Skip(1).Select(p => new LineSegment(p, true)).ToArray(), true);
            PathGeometry valueGeometry = new PathGeometry(new PathFigure[] { valueFigure });
            drawingContext.DrawGeometry(_valueGradientBrush, new Pen(_valueStrokeBrush, 0.5), valueGeometry);

            // 绘制顶点圆圈
            double circleRadius = 2; // 圆圈半径
            for (int i = 0; i < sides; i++)
            {
                drawingContext.DrawEllipse(
                    _pointFillBrush, // 使用缓存的填充颜色
                    null,            // 无边框
                    valuePoints[i],  // 圆心位置
                    circleRadius,    // 水平方向半径
                    circleRadius     // 垂直方向半径
                );
            }
        }
        
        #endregion
    }
}
