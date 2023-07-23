using System;
using System.Windows;
using System.Windows.Controls;
using HandyControl.Controls;
using System.Configuration;
using System.Globalization;
using Application = System.Windows.Application;
using Window = System.Windows.Window;

namespace ClockLite
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App: Application
    {
        private static Window _currentWindow;
        private static TrayIcon _trayIcon;
        private static bool _isFixed;
        private static ClockConfiguration _clockConfiguration;
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            ClockConfiguration.TestEvent = MainWindowReload;
            
            if (! Double.TryParse(ConfigurationManager.AppSettings["WindowLeft"], out var left)) left = 20;
            if (! Double.TryParse(ConfigurationManager.AppSettings["WindowTop"], out var top)) top = 20;
            if (! Double.TryParse(ConfigurationManager.AppSettings["WindowHeight"], out var height)) height = 300;
            if (! Double.TryParse(ConfigurationManager.AppSettings["WindowWidth"], out var width)) width = 400;
            _isFixed = ConfigurationManager.AppSettings["IsFixed"] != "0";

            if (_isFixed) _currentWindow = new FixClock(left, top, height, width);
            else _currentWindow = new MainWindow(left, top, height, width);
            _trayIcon = new TrayIcon();
            TrayIcon.ChangeFixHandler(_isFixed);
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            _currentWindow = null;
            _trayIcon = null;
        }

        private void MainWindowReload()
        {
            _currentWindow.Close();
            if (! Double.TryParse(ConfigurationManager.AppSettings["WindowLeft"], out var left)) left = 20;
            if (! Double.TryParse(ConfigurationManager.AppSettings["WindowTop"], out var top)) top = 20;
            if (! Double.TryParse(ConfigurationManager.AppSettings["WindowHeight"], out var height)) height = 300;
            if (! Double.TryParse(ConfigurationManager.AppSettings["WindowWidth"], out var width)) width = 400;
            _currentWindow = new MainWindow(left, top, height, width);
        }

        #region TrayIcon

        public class TrayIcon
        {
            private static NotifyIcon _notifyIcon;

            public TrayIcon()
            {
                _notifyIcon = Current.FindResource("TrayIcon") as NotifyIcon;
                if (_notifyIcon != null)
                {
                    _notifyIcon.Init();
                    if (_notifyIcon != null) _notifyIcon.MouseDoubleClick += TrayIcon_DoubleClick;
                }
            }

            private void TrayIcon_DoubleClick(object sender, RoutedEventArgs e)
            {
                if (_currentWindow == null || !_currentWindow.IsLoaded)
                {
                    if (! Double.TryParse(ConfigurationManager.AppSettings["WindowLeft"], out var left)) left = 20;
                    if (! Double.TryParse(ConfigurationManager.AppSettings["WindowTop"], out var top)) top = 20;
                    if (! Double.TryParse(ConfigurationManager.AppSettings["WindowHeight"], out var height)) height = 300;
                    if (! Double.TryParse(ConfigurationManager.AppSettings["WindowWidth"], out var width)) width = 400;
                    _currentWindow = new MainWindow(left, top, height, width);
                    _currentWindow.Show();
                }
            }

            public static void ChangeFixHandler(bool isFixed)
            {
                if (isFixed)
                {
                    if (_notifyIcon.ContextMenu != null && LogicalTreeHelper.FindLogicalNode(_notifyIcon.ContextMenu, "MenuItemFixed") is MenuItem fixedMenuItem) 
                        fixedMenuItem.Header = "移动窗口";
                    if (_notifyIcon.ContextMenu != null && LogicalTreeHelper.FindLogicalNode(_notifyIcon.ContextMenu, "MenuItemSetting") is MenuItem settingMenuItem) 
                        settingMenuItem.IsEnabled = false;
                }
                else
                {
                    if (_notifyIcon.ContextMenu != null && LogicalTreeHelper.FindLogicalNode(_notifyIcon.ContextMenu, "MenuItemFixed") is MenuItem fixedMenuItem) 
                        fixedMenuItem.Header = "固定窗口";
                    if (_notifyIcon.ContextMenu != null && LogicalTreeHelper.FindLogicalNode(_notifyIcon.ContextMenu, "MenuItemSetting") is MenuItem settingMenuItem) 
                        settingMenuItem.IsEnabled = true;
                }
                
            }

            ~TrayIcon()
            {
                _notifyIcon.Dispose();
                _notifyIcon = null;
            }
        
        }

        #endregion

        private void MenuItemExit_OnClick(object sender, RoutedEventArgs e)
        {
            if (_currentWindow is MainWindow)
            {
                double left = _currentWindow.Left;
                double top = _currentWindow.Top;
                double height = _currentWindow.Height;
                double width = _currentWindow.Width;
                Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                configuration.AppSettings.Settings["WindowLeft"].Value = left.ToString(CultureInfo.CurrentCulture);
                configuration.AppSettings.Settings["WindowTop"].Value = top.ToString(CultureInfo.CurrentCulture);
                configuration.AppSettings.Settings["WindowHeight"].Value = height.ToString(CultureInfo.CurrentCulture);
                configuration.AppSettings.Settings["WindowWidth"].Value = width.ToString(CultureInfo.CurrentCulture);
                configuration.AppSettings.Settings["IsFixed"].Value = "0";
                configuration.Save();
                ConfigurationManager.RefreshSection("appSettings");
            }
            else
            {
                Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                configuration.AppSettings.Settings["IsFixed"].Value = "1";
                configuration.Save();
                ConfigurationManager.RefreshSection("appSettings");
            }
            
            Current.Shutdown();
            Environment.Exit(0);
        }

        private void MenuItemFixed_OnClick(object sender, RoutedEventArgs e)
        {
            if (_clockConfiguration == null) {}
            else if (_clockConfiguration.IsLoaded) _clockConfiguration.Close();

            double left = _currentWindow.Left;
            double top = _currentWindow.Top;
            double height = _currentWindow.Height;
            double width = _currentWindow.Width;
            if (!_isFixed)
            {
                _currentWindow.Close();
                _currentWindow = new FixClock(left, top, height, width);
                _isFixed = true;
            }
            else
            {
                _currentWindow.Close();
                _currentWindow = new MainWindow(left, top, height, width);
                _isFixed = false;
            }
            
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings["WindowLeft"].Value = left.ToString(CultureInfo.CurrentCulture);
            configuration.AppSettings.Settings["WindowTop"].Value = top.ToString(CultureInfo.CurrentCulture);
            configuration.AppSettings.Settings["WindowHeight"].Value = height.ToString(CultureInfo.CurrentCulture);
            configuration.AppSettings.Settings["WindowWidth"].Value = width.ToString(CultureInfo.CurrentCulture);
            configuration.Save();
            ConfigurationManager.RefreshSection("appSettings");
            TrayIcon.ChangeFixHandler(_isFixed);
        }

        private void MenuItemConfig_OnClick(object sender, RoutedEventArgs e)
        {
            if (_clockConfiguration == null || !_clockConfiguration.IsLoaded)
            {
                _clockConfiguration = new ClockConfiguration();
                _clockConfiguration.Show();
            }
        }
    }
}