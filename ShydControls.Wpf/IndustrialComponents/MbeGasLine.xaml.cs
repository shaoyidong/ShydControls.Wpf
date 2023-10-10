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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShydControls.Wpf.IndustrialComponents
{
    /// <summary>
    /// MbeGasLine.xaml 的交互逻辑
    /// </summary>
    public partial class MbeGasLine : UserControl
    {
        public ObservableCollection<bool> ValvesStatus { get; set; } =new ObservableCollection<bool>(new bool[6]);
       
        private int regulationStatus;
        /// <summary>
        /// 调节阀状态 0:手动; 1:pid关; 2:pid开
        /// </summary>      
        public int RegulationStatus
        {
            get { return regulationStatus; }
            set {
               
                switch (value)
                {
                    case 0:
                        processValve.Theme = "default";
                        ventingValve.Theme = "default";
                        break;
                    case 1:
                        processValve.Theme = "red";
                        ventingValve.Theme = "red";
                        break;
                    case 2:
                        processValve.Theme = "green";
                        ventingValve.Theme = "green";
                        break;
                    default:
                        return;
                }
                regulationStatus = value;
            }
        }

        public MbeGasLine()
        {
            InitializeComponent();
        }
    }
}
