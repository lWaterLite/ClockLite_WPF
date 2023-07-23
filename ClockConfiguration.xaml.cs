using System;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace ClockLite
{
    public partial class ClockConfiguration : Window
    {

        private static ClockArgs _clockArgs;
        public ClockConfiguration()
        {
            InitializeComponent();
            if (! Double.TryParse(ConfigurationManager.AppSettings["WindowLeft"], out var left)) left = 20;
            if (! Double.TryParse(ConfigurationManager.AppSettings["WindowTop"], out var top)) top = 20;
            if (! Double.TryParse(ConfigurationManager.AppSettings["WindowHeight"], out var height)) height = 300;
            if (! Double.TryParse(ConfigurationManager.AppSettings["WindowWidth"], out var width)) width = 400;
            if (! Double.TryParse(ConfigurationManager.AppSettings["FontSize"], out var fontSize)) fontSize = 80;
            var color = ColorConverter.ConvertFromString(ConfigurationManager.AppSettings["FontColor"]);
            Color fontColor;
            if (color is Color o) fontColor = o;
            else fontColor = Color.FromRgb(0, 0, 0);
            _clockArgs = new ClockArgs()
            {
                WindowLeft = left,
                WindowTop = top,
                WindowHeight = height,
                WindowWidth = width,
                FontSize = fontSize,
                FontColor = fontColor,
            };
            DataContext = _clockArgs;
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings["WindowLeft"].Value = _clockArgs.WindowLeft.ToString(CultureInfo.CurrentCulture);
            configuration.AppSettings.Settings["WindowTop"].Value = _clockArgs.WindowTop.ToString(CultureInfo.CurrentCulture);
            configuration.AppSettings.Settings["WindowHeight"].Value = _clockArgs.WindowHeight.ToString(CultureInfo.CurrentCulture);
            configuration.AppSettings.Settings["WindowWidth"].Value = _clockArgs.WindowWidth.ToString(CultureInfo.CurrentCulture);
            configuration.AppSettings.Settings["FontSize"].Value =
                _clockArgs.FontSize.ToString(CultureInfo.CurrentCulture);
            configuration.AppSettings.Settings["FontColor"].Value =
                $"#{_clockArgs.FontColor.R:X2}{_clockArgs.FontColor.G:X2}{_clockArgs.FontColor.B:X2}";
            configuration.Save();
            ConfigurationManager.RefreshSection("appSettings");
            TestEvent();

            Close();
        }

        public delegate void AppDelegate();

        public static AppDelegate TestEvent;
        
    }
    
    public class ClockArgs: INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private double _left;
            public double WindowLeft
            {
                get => _left;
                set
                {
                    _left = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WindowLeft"));
                }
            }
            
            private double _top;
            public double WindowTop
            {
                get => _top;
                set
                {
                    _top = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WindowTop"));
                }
            }
            
            private double _height;
            public double WindowHeight  
            {
                get => _height;
                set
                {
                    _height = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WindowHeight"));
                }
            }

            private  double _width;
            public double WindowWidth
            {
                get => _width;
                set
                {
                    _width = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WindowWidth"));
                }
            }

            private Color _fontColor;
            public Color FontColor
            {
                get => _fontColor;
                set
                {
                    _fontColor = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FontColor"));
                }
            }

            private double _fontSize;
            public double FontSize
            {
                get => _fontSize;
                set
                {
                    _fontSize = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FontSize"));
                }
            }

        }
    
    [ValueConversion(typeof(Color), typeof(string))]
    class FontColorConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                Color color = (Color)value;
                return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
            }
            return "#000000";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var o = ColorConverter.ConvertFromString((string)value);
                if (o is Color color) return color;
            }
            return Color.FromRgb(0, 0, 0);
        }
    }
}