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
    /// Valve.xaml 的交互逻辑
    /// </summary>
    public partial class Valve : UserControl
    {
        public bool TextVisibility
        {
            get { return (bool)GetValue(TextVisibilityProperty); }
            set { SetValue(TextVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextVisibilityProperty =
            DependencyProperty.Register("TextVisibility", typeof(bool), typeof(Valve), new PropertyMetadata(false,new PropertyChangedCallback(TextVisibilityPropertyChanged)));

        private static void TextVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue.Equals(e.OldValue))
            {
                return;
            }
            if (d is Valve valve)
            {
                valve.txt.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
            }
        }


        public string Theme
        {
            get { return (string)GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Theme.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThemeProperty =
            DependencyProperty.Register("Theme", typeof(string), typeof(Valve), new PropertyMetadata("default",new PropertyChangedCallback(ThemePropertyChanged)));

        private static void ThemePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue.Equals(e.OldValue))
            {
                return;
            }
            if (d is Valve valve)
            {
              
                switch (e.NewValue.ToString())
                {
                    case "default":
                        valve.Resources["Color1"] = ColorConverter.ConvertFromString("#FF5B5C5F");
                        valve.Resources["Color2"] = ColorConverter.ConvertFromString("#FFECECED");
                        valve.Resources["Color3"] = ColorConverter.ConvertFromString("#FFB1B3B6");
                        valve.Resources["Color4"] = ColorConverter.ConvertFromString("#FF5B5C5F");
                        valve.Resources["Color5"] = ColorConverter.ConvertFromString("#FFCECECE");
                        break;
                    case "green":
                        valve.Resources["Color1"] = ColorConverter.ConvertFromString("#FF005B00");
                        valve.Resources["Color2"] = ColorConverter.ConvertFromString("#FFD6EBD6");
                        valve.Resources["Color3"] = ColorConverter.ConvertFromString("#FF65B265");
                        valve.Resources["Color4"] = ColorConverter.ConvertFromString("#FF005B00");
                        valve.Resources["Color5"] = ColorConverter.ConvertFromString("#FF9BCD9B");
                        break;
                    case "red":
                        valve.Resources["Color1"] = ColorConverter.ConvertFromString("#FFB70000");
                        valve.Resources["Color2"] = ColorConverter.ConvertFromString("#FFFFD6D6");
                        valve.Resources["Color3"] = ColorConverter.ConvertFromString("#FFFF6565");
                        valve.Resources["Color4"] = ColorConverter.ConvertFromString("#FFB70000");
                        valve.Resources["Color5"] = ColorConverter.ConvertFromString("#FFFF9B9B");
                        break;
                    case "yellow":
                        break;
                    default:
                        break;
                }
            }
        }

        public Valve()
        {
            InitializeComponent();
          
        }
    }
}
