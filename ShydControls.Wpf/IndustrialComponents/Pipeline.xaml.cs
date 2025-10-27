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
            // Ensure the initial state is set based on the default Direction value
            OnDirectionProperty(this, new DependencyPropertyChangedEventArgs(DirectionProperty, null, Direction));
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


        public int Direction
        {
            get { return (int)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Direction.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(int), typeof(Pipeline), new PropertyMetadata(0, new PropertyChangedCallback(OnDirectionProperty)));

        private static void OnDirectionProperty(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VisualStateManager.GoToState(d as Pipeline, (int)e.NewValue == 0 ? "WEFlowState" : "EWFlowState", false);
        }

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
