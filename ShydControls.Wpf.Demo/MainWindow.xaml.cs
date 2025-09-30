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
        }       

        private void ConventionalBtn_Click(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new ConventionalControls();
        }

        private void ApertureBtn_Click(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new ApertureControls();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ConventionalBtn_Click(this, null);
        }
    }
}