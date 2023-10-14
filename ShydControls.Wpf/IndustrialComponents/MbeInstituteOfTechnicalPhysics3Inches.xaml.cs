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
    /// MbeInstituteOfTechnicalPhysisc3Inches.xaml 的交互逻辑
    /// </summary>
    public partial class MbeInstituteOfTechnicalPhysics3Inches : UserControl
    {
        public ObservableCollection<bool> GasVisibility { get; }
        public ObservableCollection<bool?> CellShutterStatus { get; } = new ObservableCollection<bool?>(new bool?[12]);
        public ObservableCollection<string> CellNames { get; } = new ObservableCollection<string>(new string[12]);
        public ObservableCollection<bool?> GateValveStatus { get; } = new ObservableCollection<bool?>(new bool?[3]);

        public bool IsRotating
        {
            get { return (bool)GetValue(IsRotatingProperty); }
            set { SetValue(IsRotatingProperty, value); }
        }
        public static readonly DependencyProperty IsRotatingProperty =
            DependencyProperty.Register("IsRotating", typeof(bool), typeof(MbeInstituteOfTechnicalPhysics3Inches), new PropertyMetadata(default(bool), new PropertyChangedCallback(OnRotatingChanged)));

        private static void OnRotatingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mbe55 = d as MbeInstituteOfTechnicalPhysics3Inches;
            var story = mbe55._story;
            if ((bool)e.NewValue == (bool)e.OldValue)
            {
                return;
            }
            if ((bool)e.NewValue)
            {
                mbe55.manipulatorRt.Stroke = mbe55._onBrush;
                story.Resume();
            }
            else
            {
                mbe55.manipulatorRt.Stroke = mbe55._offBrush;
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
            DependencyProperty.Register("Rpm", typeof(double), typeof(MbeInstituteOfTechnicalPhysics3Inches), new PropertyMetadata(60d, new PropertyChangedCallback(OnRpmChanged)));

        private static void OnRpmChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //var converter = TypeDescriptor.GetConverter(typeof(Duration));
            //converter.ConvertFrom(d)
            var newVal = (double)e.NewValue;
            var oldVal = (double)e.OldValue;
            if (newVal == oldVal) return;

            //double val = 1 / (Math.Abs(newVal) / 60);
            var mbe55 = d as MbeInstituteOfTechnicalPhysics3Inches;
            var story = mbe55._story;
            if (newVal == 0)
            {
                mbe55.manipulatorRt.Stroke = mbe55._offBrush;
                story.Pause();
                //VisualStateManager.GoToState(mbeGasRise, "StopRotationState", false);
                return;
            }
            mbe55.manipulatorRt.Stroke = mbe55._onBrush;
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



        public bool? MainShutterStatus
        {
            get { return (bool?)GetValue(MainShutterStatusProperty); }
            set { SetValue(MainShutterStatusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MainShutterStatus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MainShutterStatusProperty =
            DependencyProperty.Register("MainShutterStatus", typeof(bool?), typeof(MbeInstituteOfTechnicalPhysics3Inches), new PropertyMetadata(null, new PropertyChangedCallback(OnMainShutterStatusChanged)));

        private static void OnMainShutterStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mbe55 = d as MbeInstituteOfTechnicalPhysics3Inches;

            if (e.NewValue == e.OldValue)
            {
                return;
            }
            if (e.NewValue == null)
            {
                mbe55.canvasMainShutter.Visibility = Visibility.Collapsed;
                return;
            }
            else
            {
                mbe55.canvasMainShutter.Visibility = Visibility.Visible;
            }
            var res = mbe55.canvasMainShutter.Resources["BrushMainShutter"];
            if ((bool)e.NewValue)
            {
                mbe55._mainShutterOffSb.Stop();
                mbe55._mainShutterOnSb.Begin();
            }
            else
            {
                mbe55._mainShutterOnSb.Stop();
                mbe55._mainShutterOffSb.Begin();
            }
        }

        private Polygon[] _cellShutters;
        private Brush _radialBrush;
        private Brush _onBrush;
        private Brush _offBrush;
        private Brush _brush3;

        private TextBlock[] _txtCellNames;

        private Border[] _gateValves;

        private Storyboard _gv11OnSb;
        private Storyboard _gv11OffSb;
        private Storyboard _story;

        private Storyboard _mainShutterOnSb;
        private Storyboard _mainShutterOffSb;
        public MbeInstituteOfTechnicalPhysics3Inches()
        {
            InitializeComponent();
            this.Resources["Color1"] = null;
            this.Resources["Color2"] = null;
            this.Resources["Color3"] = null;
            this.Resources["Color4"] = null;
            this.Resources["Color5"] = null;
            CellShutterStatus.CollectionChanged += CellShutterStatus_CollectionChanged;
            _cellShutters = new Polygon[12] { cellShutter01, cellShutter02, cellShutter03, cellShutter04, cellShutter05, cellShutter06, cellShutter07, cellShutter08, cellShutter09, cellShutter10, cellShutter11, cellShutter12 };
            _radialBrush = this.Resources["BrushRadial"] as Brush;
            _onBrush = this.Resources["BurshOn"] as Brush;
            _offBrush = this.Resources["BurshOff"] as Brush;

            CellNames.CollectionChanged += CellNames_CollectionChanged;
            _txtCellNames = new TextBlock[12] { txtCell01Name, txtCell02Name, txtCell03Name, txtCell04Name, txtCell05Name, txtCell06Name, txtCell07Name, txtCell08Name, txtCell09Name, txtCell10Name, txtCell11Name, txtCell12Name };

            GateValveStatus.CollectionChanged += GateValveStatus_CollectionChanged;
            _gateValves = new Border[3] { gv01, gv02, gv03};
            _brush3 = this.Resources["Brush3"] as Brush;

            _gv11OnSb = this.Resources["gv11OnSb"] as Storyboard;
            _gv11OffSb = this.Resources["gv11OffSb"] as Storyboard;

            _story = this.Resources["rotationStoryboard"] as Storyboard;

            _mainShutterOnSb = this.Resources["mainShutterOnStoryboard"] as Storyboard;
            _mainShutterOffSb = this.Resources["mainShutterOffStoryboard"] as Storyboard;

            this.Loaded += MbeInstituteOfTechnicalPhysics3Inches_Loaded; ;
        }

        private void MbeInstituteOfTechnicalPhysics3Inches_Loaded(object sender, RoutedEventArgs e)
        {
            _story.Begin();
            _story.Pause();
        }
        private void GateValveStatus_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    for (int i = 0; i < GateValveStatus.Count; i++)
                    {
                        if (GateValveStatus[i] == null)
                        {
                            _gateValves[i].Background = _brush3;
                        }
                        else if ((bool)GateValveStatus[i]!)
                        {
                            _gateValves[i].Background = _onBrush;
                        }
                        else
                        {
                            _gateValves[i].Background = _offBrush;
                        }
                    }
                    break;
                default: break;
            }
        }

        private void CellNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    for (int i = 0; i < CellNames.Count; i++)
                    {
                        if (string.IsNullOrWhiteSpace(CellNames[i]))
                        {
                            _txtCellNames[i].Text = $"#{i + 1}";
                        }
                        else
                        {
                            _txtCellNames[i].Text = CellNames[i];
                        }
                    }
                    break;
                default: break;
            }
        }

        private void CellShutterStatus_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    for (int i = 0; i < CellShutterStatus.Count; i++)
                    {
                        if (CellShutterStatus[i] == null)
                        {
                            _cellShutters[i].Fill = _radialBrush;
                        }
                        else if ((bool)CellShutterStatus[i])
                        {
                            _cellShutters[i].Fill = _onBrush;
                        }
                        else
                        {
                            _cellShutters[i].Fill = _offBrush;
                        }
                    }
                    break;
                default: break;
            }

        }
    }
}
