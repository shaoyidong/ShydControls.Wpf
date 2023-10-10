using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ShydControls.Wpf.IndustrialComponents
{
    /// <summary>
    /// MbeMainChamber.xaml 的交互逻辑
    /// </summary>
    public partial class MbeMainChamber : UserControl
    {
        private List<DoubleBeamGas> _singleBeamGasList;
        private Storyboard _story;
        public ObservableCollection<bool> GasVisibility { get; } = new ObservableCollection<bool>(new bool[12]);
        public MbeMainChamber()
        {
            InitializeComponent();
            this.Loaded += MbeGasRise_Loaded;
        }
        private void MbeGasRise_Loaded(object sender, RoutedEventArgs e)
        {
            _singleBeamGasList = GetChildControls <DoubleBeamGas>(this);
            _story = this.Resources["rotationStoryboard"] as Storyboard;
            _story.Begin();
            _story.Pause();
        }

        private int _direction = 0;
        /// <summary>
        /// 液体流向，接受0和1两个值
        /// </summary>
        public int Direction
        {
            get { return _direction; }
            set
            {
                _direction = value;
                foreach (var item in _singleBeamGasList)
                {
                    item.Direction = value;
                }
            }
        }
        public bool IsRotating
        {
            get { return (bool)GetValue(IsRotatingProperty); }
            set { SetValue(IsRotatingProperty, value); }
        }
        public static readonly DependencyProperty IsRotatingProperty =
            DependencyProperty.Register("IsRotating", typeof(bool), typeof(MbeMainChamber), new PropertyMetadata(default(bool), new PropertyChangedCallback(OnRotatingChanged)));

        private static void OnRotatingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var story = (d as MbeMainChamber)._story;
            if ((bool)e.NewValue == (bool)e.OldValue)
            {
                return;
            }
            if ((bool)e.NewValue)
            {
                story.Resume();
            }
            else
            {
                story.Pause();
            }
            //不使用VisualStateManager是因为无法动态控制动画启停及速度
            //VisualStateManager.GoToState(d as MbeGasRise, (bool)e.NewValue ? "RotationState" : "StopRotationState", false);


        }


        public double Rpm
        {
            get { return (double)GetValue(RpmProperty); }
            set { SetValue(RpmProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Rpm.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RpmProperty =
            DependencyProperty.Register("Rpm", typeof(double), typeof(MbeMainChamber), new PropertyMetadata(60d, new PropertyChangedCallback(OnRpmChanged)));

        private static void OnRpmChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //var converter = TypeDescriptor.GetConverter(typeof(Duration));
            //converter.ConvertFrom(d)
            var newVal = (double)e.NewValue;
            var oldVal = (double)e.OldValue;
            if (newVal == oldVal) return;

            //double val = 1 / (Math.Abs(newVal) / 60);
            MbeMainChamber mbeGasRise = d as MbeMainChamber;
            var story = mbeGasRise._story;
            if (newVal == 0)
            {
                story.Pause();
                //VisualStateManager.GoToState(mbeGasRise, "StopRotationState", false);
                return;
            }
            if (newVal > 0 && oldVal <= 0)
            {
                story.Pause();
                var time = story.GetCurrentTime();
                (story.Children[0] as DoubleAnimation).To = 360;
                story.Begin();
                story.Seek(story.Duration.TimeSpan - time);

            }
            else if (newVal < 0 && oldVal >= 0)
            {
                story.Pause();

                var time = story.GetCurrentTime();
                (story.Children[0] as DoubleAnimation).To = -360;

                story.Begin();
                story.Seek(story.Duration.TimeSpan - time);
            }


            //mbeGasRise.rotationStoryboard.SpeedRatio = (Math.Abs(newVal) / 60);

            story.SetSpeedRatio(Math.Abs(newVal) / 60);


        }






        //public Brush LiquidColor
        //{
        //    get { return (Brush)GetValue(LiquidColorProperty); }
        //    set { SetValue(LiquidColorProperty, value); }
        //}
        //public static readonly DependencyProperty LiquidColorProperty =
        //    DependencyProperty.Register("LiquidColor", typeof(Brush), typeof(MbeGasRise), new PropertyMetadata(Brushes.Orange));

        //public int CapRadius
        //{
        //    get { return (int)GetValue(CapRadiusProperty); }
        //    set { SetValue(CapRadiusProperty, value); }
        //}
        //public static readonly DependencyProperty CapRadiusProperty =
        //    DependencyProperty.Register("CapRadius", typeof(int), typeof(MbeGasRise), new PropertyMetadata(5));


        /// <summary>
        /// 获取T类型的子控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private List<T> GetChildControls<T>(DependencyObject obj) where T : FrameworkElement
        {
            List<T> results = new List<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child is T)
                {
                    results.Add(child as T);
                }
                results.AddRange(GetChildControls<T>(child));
            }
            return results;
        }
    }
}
