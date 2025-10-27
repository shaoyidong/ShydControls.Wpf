using ShydControls.Wpf.Demo.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShydControls.Wpf.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // 默认显示基础控件页面
            contentControl.Content = new BasicControlsView();
        }
        
        private void btnBasic_Click(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new BasicControlsView();
        }

        private void btnAperture_Click(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new ApertureControlsView();
        }

        private void btnDashboard_Click(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new DashboardControlsView();
        }

        private void btnIndustrial_Click(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new IndustrialComponentsView();
        }

        private void btnOther_Click(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new OtherControlsView();
        }

        private void btnCharts_Click(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new ChartsControlsView();
        }
    }
}