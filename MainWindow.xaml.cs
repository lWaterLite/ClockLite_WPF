using System;
using System.Configuration;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace ClockLite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private DispatcherTimer _timer;

        public MainWindow(double left, double top, double height, double width)
        {
            InitializeComponent();
            ClockInit();
            Left = left;
            Top = top;
            Height = height;
            Width = width;
            var color = ColorConverter.ConvertFromString(ConfigurationManager.AppSettings["FontColor"]);
            if (color is Color color1)
            {
                LabelTime.Foreground = new SolidColorBrush(color1);
            }
            if (! Double.TryParse(ConfigurationManager.AppSettings["FontSize"], out var fontSize)) fontSize = 80;
            LabelTime.FontSize = fontSize;
            Show();
        }

        private void ClockInit()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (sender, args) =>
            {
                LabelTime.Content = DateTime.Now.ToString("HH:mm:ss");
            };
            _timer.Start();
        }
        
    }
}