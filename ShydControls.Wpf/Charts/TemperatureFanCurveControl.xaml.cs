using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShydControls.Wpf.Charts
{
    /// <summary>
    /// 温度-风扇转速曲线控件
    /// </summary>
    public partial class TemperatureFanCurveControl : UserControl
    {
        // 定义曲线点的数据结构，实现INotifyPropertyChanged接口以支持UI实时更新
        public class CurvePoint : INotifyPropertyChanged
        {
            private double _temperature;
            public double Temperature
            {
                get { return _temperature; }
                set
                {
                    _temperature = value;
                    OnPropertyChanged();
                }
            }

            private double _fanSpeed;
            public double FanSpeed
            {
                get { return _fanSpeed; }
                set
                {
                    _fanSpeed = value;
                    OnPropertyChanged();
                }
            }

            private Ellipse? _visual;
            public Ellipse? Visual
            {
                get { return _visual; }
                set
                {
                    _visual = value;
                    OnPropertyChanged();
                }
            }

            private bool _isSelected;
            public bool IsSelected
            {
                get { return _isSelected; }
                set
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }

            // 实现INotifyPropertyChanged接口
            public event PropertyChangedEventHandler? PropertyChanged;

            protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        // 公开的依赖属性，用于ViewModel绑定
        public static readonly DependencyProperty CurvePointsProperty =
            DependencyProperty.Register("CurvePoints", typeof(ObservableCollection<CurvePoint>),
            typeof(TemperatureFanCurveControl), new PropertyMetadata(null, OnCurvePointsChanged));

        // 依赖属性的CLR包装器
        public ObservableCollection<CurvePoint> CurvePoints
        {
            get { return (ObservableCollection<CurvePoint>)GetValue(CurvePointsProperty); }
            set { SetValue(CurvePointsProperty, value); }
        }

        // 添加新的依赖属性，用于控制曲线是否可拖动
        public static readonly DependencyProperty IsDraggableProperty =
            DependencyProperty.Register("IsDraggable", typeof(bool), typeof(TemperatureFanCurveControl),
                new PropertyMetadata(true));

        public bool IsDraggable
        {
            get { return (bool)GetValue(IsDraggableProperty); }
            set { SetValue(IsDraggableProperty, value); }
        }

        // 依赖属性变更回调
        private static void OnCurvePointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as TemperatureFanCurveControl;
            if (control != null)
            {
                // 如果旧集合存在，移除事件订阅
                if (e.OldValue is ObservableCollection<CurvePoint> oldCollection)
                {
                    oldCollection.CollectionChanged -= control.OnCollectionChanged;
                }

                // 如果新集合存在，添加事件订阅并绘制图表
                if (e.NewValue is ObservableCollection<CurvePoint> newCollection)
                {
                    newCollection.CollectionChanged += control.OnCollectionChanged;
                    control._curvePoints = newCollection;
                    control.DrawChart();
                }
            }
        }

        // 集合变更事件处理
        private void OnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DrawChart();
        }

        // 内部存储曲线点的集合
        private ObservableCollection<CurvePoint> _curvePoints = new ObservableCollection<CurvePoint>();

        // 当前正在拖动的点
        private CurvePoint? _draggingPoint = null;

        // 默认参数
        private const int MinTemperature = 0;
        private const int MaxTemperature = 100;
        private const int MinFanSpeed = 0;
        private const int MaxFanSpeed = 100;
        private const int GridLineCount = 10;
        private const int PointSize = 10;

        public TemperatureFanCurveControl()
        {
            InitializeComponent();

            // 初始化内部集合
            _curvePoints = new ObservableCollection<CurvePoint>();
            // 设置依赖属性的默认值为内部集合
            CurvePoints = _curvePoints;
        }

        /// <summary>
        /// 控件加载时初始化
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // 如果曲线点集合为空，设置默认曲线点
            if (_curvePoints.Count == 0)
            {
                SetDefaultCurvePoints();
            }
            // 绘制图表
            DrawChart();
        }

        /// <summary>
        /// 设置默认曲线点
        /// </summary>
        private void SetDefaultCurvePoints()
        {
            _curvePoints.Clear();
            // 添加一些默认的点，形成一条合理的温度-风扇曲线
            _curvePoints.Add(new CurvePoint { Temperature = 0, FanSpeed = 0 });
            _curvePoints.Add(new CurvePoint { Temperature = 40, FanSpeed = 30 });
            _curvePoints.Add(new CurvePoint { Temperature = 60, FanSpeed = 60 });
            _curvePoints.Add(new CurvePoint { Temperature = 80, FanSpeed = 100 });
        }

        public void ResetToDefaultCurve()
        {
            SetDefaultCurvePoints();
            DrawChart();
        }

        /// <summary>
        /// 公开设置自定义曲线点的方法，供ViewModel调用
        /// </summary>
        /// <param name="points">自定义的曲线点列表</param>
        public void SetCustomCurvePoints(IEnumerable<CurvePoint> points)
        {
            _curvePoints.Clear();
            foreach (var point in points)
            {
                // 确保点的值在有效范围内
                double validTemp = Math.Max(MinTemperature, Math.Min(MaxTemperature, point.Temperature));
                double validSpeed = Math.Max(MinFanSpeed, Math.Min(MaxFanSpeed, point.FanSpeed));

                _curvePoints.Add(new CurvePoint
                {
                    Temperature = validTemp,
                    FanSpeed = validSpeed
                });
            }
            DrawChart();
        }

        /// <summary>
        /// 绘制整个图表
        /// </summary>
        private void DrawChart()
        {
            // 清空画布
            ChartCanvas.Children.Clear();

            // 绘制网格线
            DrawGridLines();

            // 绘制坐标轴
            DrawAxes();

            // 绘制曲线
            DrawCurve();

            // 绘制数据点
            DrawPoints();

            //// 显示信息
            //UpdateInfoText();
        }

        /// <summary>
        /// 绘制网格线
        /// </summary>
        private void DrawGridLines()
        {
            double canvasWidth = ChartCanvas.ActualWidth;
            double canvasHeight = ChartCanvas.ActualHeight;

            // 垂直网格线
            for (int i = 0; i <= GridLineCount; i++)
            {
                double x = (i * canvasWidth) / GridLineCount;
                Line verticalLine = new Line
                {
                    X1 = x,
                    Y1 = 0,
                    X2 = x,
                    Y2 = canvasHeight,
                    Stroke = Brushes.LightGray,
                    StrokeThickness = 0.5
                };
                ChartCanvas.Children.Add(verticalLine);

                // 绘制X轴刻度标签
                if (i < GridLineCount)
                {
                    double temperature = MinTemperature + (i * (MaxTemperature - MinTemperature)) / GridLineCount;
                    TextBlock label = new TextBlock
                    {
                        Text = temperature.ToString("0"),
                        FontSize = 10,
                        Foreground = Brushes.Gray
                    };
                    Canvas.SetLeft(label, x - 10);
                    Canvas.SetTop(label, canvasHeight + 5);
                    ChartCanvas.Children.Add(label);
                }
            }

            // 水平网格线
            for (int i = 0; i <= GridLineCount; i++)
            {
                double y = canvasHeight - (i * canvasHeight) / GridLineCount;
                Line horizontalLine = new Line
                {
                    X1 = 0,
                    Y1 = y,
                    X2 = canvasWidth,
                    Y2 = y,
                    Stroke = Brushes.LightGray,
                    StrokeThickness = 0.5
                };
                ChartCanvas.Children.Add(horizontalLine);

                // 绘制Y轴刻度标签
                if (i < GridLineCount)
                {
                    double fanSpeed = MinFanSpeed + (i * (MaxFanSpeed - MinFanSpeed)) / GridLineCount;
                    TextBlock label = new TextBlock
                    {
                        Text = fanSpeed.ToString("0"),
                        FontSize = 10,
                        Foreground = Brushes.Gray
                    };
                    Canvas.SetRight(label, canvasWidth + 5);
                    Canvas.SetTop(label, y - 10);
                    ChartCanvas.Children.Add(label);
                }
            }
        }

        /// <summary>
        /// 绘制坐标轴
        /// </summary>
        private void DrawAxes()
        {
            double canvasWidth = ChartCanvas.ActualWidth;
            double canvasHeight = ChartCanvas.ActualHeight;

            // X轴
            Line xAxis = new Line
            {
                X1 = 0,
                Y1 = canvasHeight,
                X2 = canvasWidth,
                Y2 = canvasHeight,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            ChartCanvas.Children.Add(xAxis);

            // Y轴
            Line yAxis = new Line
            {
                X1 = 0,
                Y1 = 0,
                X2 = 0,
                Y2 = canvasHeight,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            ChartCanvas.Children.Add(yAxis);
        }

        /// <summary>
        /// 绘制曲线
        /// </summary>
        private void DrawCurve()
        {
            if (_curvePoints.Count < 2) return;

            double canvasWidth = ChartCanvas.ActualWidth;
            double canvasHeight = ChartCanvas.ActualHeight;

            // 对点进行排序
            var sortedPoints = _curvePoints.OrderBy(p => p.Temperature).ToList();

            // 创建路径
            Path path = new Path
            {
                Stroke = Brushes.Blue,
                StrokeThickness = 2,
                Fill = Brushes.Transparent
            };

            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure();

            // 设置起点
            CurvePoint firstPoint = sortedPoints[0];
            double startX = (firstPoint.Temperature - MinTemperature) * canvasWidth / (MaxTemperature - MinTemperature);
            double startY = canvasHeight - (firstPoint.FanSpeed - MinFanSpeed) * canvasHeight / (MaxFanSpeed - MinFanSpeed);
            figure.StartPoint = new Point(startX, startY);

            // 添加线段
            for (int i = 1; i < sortedPoints.Count; i++)
            {
                CurvePoint point = sortedPoints[i];
                double x = (point.Temperature - MinTemperature) * canvasWidth / (MaxTemperature - MinTemperature);
                double y = canvasHeight - (point.FanSpeed - MinFanSpeed) * canvasHeight / (MaxFanSpeed - MinFanSpeed);
                figure.Segments.Add(new LineSegment(new Point(x, y), true));
            }

            geometry.Figures.Add(figure);
            path.Data = geometry;
            ChartCanvas.Children.Add(path);
        }

        /// <summary>
        /// 绘制数据点
        /// </summary>
        private void DrawPoints()
        {
            double canvasWidth = ChartCanvas.ActualWidth;
            double canvasHeight = ChartCanvas.ActualHeight;

            foreach (var point in _curvePoints)
            {
                double x = (point.Temperature - MinTemperature) * canvasWidth / (MaxTemperature - MinTemperature);
                double y = canvasHeight - (point.FanSpeed - MinFanSpeed) * canvasHeight / (MaxFanSpeed - MinFanSpeed);

                // 创建点的视觉表示
                Ellipse ellipse = new Ellipse
                {
                    Width = PointSize,
                    Height = PointSize,
                    Fill = point.IsSelected ? Brushes.Red : Brushes.Blue,
                    Stroke = Brushes.White,
                    StrokeThickness = 2
                };

                // 设置点的位置
                Canvas.SetLeft(ellipse, x - PointSize / 2);
                Canvas.SetTop(ellipse, y - PointSize / 2);

                // 保存视觉对象引用
                point.Visual = ellipse;

                // 添加到画布
                ChartCanvas.Children.Add(ellipse);
            }
        }

        ///// <summary>
        ///// 更新信息文本
        ///// </summary>
        //private void UpdateInfoText()
        //{
        //    InfoText.Text = string.Format("当前点数量: {0}", _curvePoints.Count);
        //}

        /// <summary>
        /// 处理鼠标按下事件
        /// </summary>
        private void ChartCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // 如果曲线不可拖动，则不响应鼠标按下事件
            if (!IsDraggable)
                return;
            Point mousePos = e.GetPosition(ChartCanvas);

            // 检查是否点击了某个点
            foreach (var point in _curvePoints)
            {
                if (point.Visual != null)
                {
                    double pointX = Canvas.GetLeft(point.Visual) + PointSize / 2;
                    double pointY = Canvas.GetTop(point.Visual) + PointSize / 2;

                    // 计算距离判断是否点击了点
                    double distance = Math.Sqrt(Math.Pow(mousePos.X - pointX, 2) + Math.Pow(mousePos.Y - pointY, 2));
                    if (distance <= PointSize)
                    {
                        // 取消其他点的选中状态
                        foreach (var p in _curvePoints)
                        {
                            p.IsSelected = false;
                        }

                        // 设置当前点为选中状态
                        point.IsSelected = true;
                        _draggingPoint = point;
                        DrawChart();
                        return;
                    }
                }
            }

            // 如果没有点击到点，取消所有选中状态
            foreach (var point in _curvePoints)
            {
                point.IsSelected = false;
            }
            _draggingPoint = null;
            DrawChart();
        }

        /// <summary>
        /// 处理鼠标移动事件
        /// </summary>
        private void ChartCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            // 如果曲线不可拖动，则不响应拖动操作
            if (!IsDraggable)
                return;
            if (_draggingPoint != null && e.LeftButton == MouseButtonState.Pressed)
            {
                // 更新拖动点的位置
                Point mousePos = e.GetPosition(ChartCanvas);
                double canvasWidth = ChartCanvas.ActualWidth;
                double canvasHeight = ChartCanvas.ActualHeight;

                // 计算新的温度和风扇转速值，确保在有效范围内
                double temperature = MinTemperature + (mousePos.X / canvasWidth) * (MaxTemperature - MinTemperature);
                double fanSpeed = MinFanSpeed + ((canvasHeight - mousePos.Y) / canvasHeight) * (MaxFanSpeed - MinFanSpeed);

                // 限制在有效范围内
                temperature = Math.Max(MinTemperature, Math.Min(MaxTemperature, temperature));
                fanSpeed = Math.Max(MinFanSpeed, Math.Min(MaxFanSpeed, fanSpeed));

                // 更新点的值
                _draggingPoint.Temperature = temperature;
                _draggingPoint.FanSpeed = fanSpeed;

                // 重新绘制图表
                DrawChart();
            }
        }

        /// <summary>
        /// 处理鼠标释放事件
        /// </summary>
        private void ChartCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _draggingPoint = null;
        }

        /// <summary>
        /// 处理鼠标离开事件
        /// </summary>
        private void ChartCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            _draggingPoint = null;
        }
    }
}
