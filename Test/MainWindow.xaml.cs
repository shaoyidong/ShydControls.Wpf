using System;
using System.Collections.Generic;
using System.IO;
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

namespace Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //this.Loaded += MainWindow_Loaded;
            //this.Initialized += MainWindow_Initialized;
            this.ContentRendered += MainWindow_ContentRendered;
            //this.ManipulationCompleted += MainWindow_ManipulationCompleted;
        }

      

        private void MainWindow_ContentRendered(object? sender, EventArgs e)
        {

            if (v.IsLoaded && v.IsInitialized && this.IsLoaded && this.IsInitialized)
            {
                v.IsRotating = true;
                v.Rpm = 30;
            }
        }

      

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)v.ActualWidth, (int)v.ActualHeight, 96, 96, PixelFormats.Default);
            //renderTargetBitmap.Render(v);
            //BitmapEncoder bitmapEncoder = new PngBitmapEncoder();
            //bitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            //using (var fs = File.Create("test.png"))
            //{
            //    bitmapEncoder.Save(fs);
            //}

            //Grid grid = GetChildControls<Grid>(mmc).First();
            //grid.Arrange(new Rect(0, 0, grid.ActualWidth, grid.ActualHeight));
            //RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)grid.Width, (int)grid.Height, 96, 96, PixelFormats.Default);
            //renderTargetBitmap.Render(grid);
            //BitmapEncoder bitmapEncoder = new PngBitmapEncoder();
            //bitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            //using (var fs = File.Create("test.png"))
            //{
            //    bitmapEncoder.Save(fs);
            //}

            //grid.InvalidateArrange();
            v.MainShutterStatus = !v.MainShutterStatus ?? false;
            await Task.Delay(500);
            v.GateValveStatus[0] =!v.GateValveStatus[0] ?? false;
            await Task.Delay(500);
            v.GateValveStatus[1] = !v.GateValveStatus[1] ?? false;
            await Task.Delay(500);
            v.GateValveStatus[2] = !v.GateValveStatus[2] ?? false;
            await Task.Delay(500);
        }
    }
}
