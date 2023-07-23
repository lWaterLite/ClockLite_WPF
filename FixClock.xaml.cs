using System;
using System.Configuration;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace ClockLite
{
    public partial class FixClock : Window
    {
        private DispatcherTimer _timer;
        
        public FixClock(double left, double top, double height, double width)
        {
            InitializeComponent();
            ClockInit();
            Left = left;
            Top = top;
            Height = height;
            Width = width;
            string hexColor = ConfigurationManager.AppSettings["FontColor"];
            var color = ColorConverter.ConvertFromString(hexColor);
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

        private void FixClock_OnDeactivated(object sender, EventArgs e)
        {
            Topmost = true;
        }

        private void LabelTime_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}