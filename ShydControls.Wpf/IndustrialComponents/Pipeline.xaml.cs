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

namespace ShydControls.Wpf.IndustrialComponents
{
    /// <summary>
    /// Pipeline.xaml 的交互逻辑
    /// </summary>
    public partial class Pipeline : UserControl
    {
        public Pipeline()
        {
            InitializeComponent();
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

                VisualStateManager.GoToState(this, value == 0 ? "WEFlowState" : "EWFlowState", false);
            }
        }

        public Brush LiquidColor
        {
            get { return (Brush)GetValue(LiquidColorProperty); }
            set { SetValue(LiquidColorProperty, value); }
        }
        public static readonly DependencyProperty LiquidColorProperty =
            DependencyProperty.Register("LiquidColor", typeof(Brush), typeof(Pipeline), new PropertyMetadata(Brushes.Orange));

        public int CapRadius
        {
            get { return (int)GetValue(CapRadiusProperty); }
            set { SetValue(CapRadiusProperty, value); }
        }
        public static readonly DependencyProperty CapRadiusProperty =
            DependencyProperty.Register("CapRadius", typeof(int), typeof(Pipeline), new PropertyMetadata(5));


        public bool IsRunning
        {
            get { return (bool)GetValue(IsRunningProperty); }
            set { SetValue(IsRunningProperty, value); }
        }
        public static readonly DependencyProperty IsRunningProperty =
            DependencyProperty.Register("IsRunning", typeof(bool), typeof(Pipeline), new PropertyMetadata(true, new PropertyChangedCallback(OnRunningChanged)));

        private static void OnRunningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VisualStateManager.GoToState(d as Pipeline, (bool)e.NewValue ? "RunState" : "StopState", false);
        }


    }
}
