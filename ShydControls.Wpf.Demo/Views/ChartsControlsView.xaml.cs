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

namespace ShydControls.Wpf.Demo.Views
{
    /// <summary>
    /// ChartsControlsView.xaml 的交互逻辑
    /// </summary>
    public partial class ChartsControlsView : UserControl
    {
        public ChartsControlsView()
        {
            InitializeComponent();
        }

        private void btnAddPoint_Click(object sender, RoutedEventArgs e)
        {
            // 在中间温度位置添加一个点
            double midTemp = 50;
            double midSpeed = 50;

            temperatureFanCurveControl.CurvePoints.Add(new Charts.TemperatureFanCurveControl.CurvePoint
            {
                Temperature = midTemp,
                FanSpeed = midSpeed
            });           
        }

        private void btnDelPoint_Click(object sender, RoutedEventArgs e)
        {
            // 保留至少2个点以保持曲线有效
            if (temperatureFanCurveControl.CurvePoints.Count > 2)
            {
                var selectedPoints = temperatureFanCurveControl.CurvePoints.Where(p => p.IsSelected).ToList();
                foreach (var point in selectedPoints)
                {
                    temperatureFanCurveControl.CurvePoints.Remove(point);
                }
            }
        }
    }
}