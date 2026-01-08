using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace Games_Launcher.Views
{
    /// <summary>
    /// Lógica de interacción para ConfigView.xaml
    /// </summary>
    public partial class ConfigView : UserControl
    {
		private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
		private const string ValueName = "Games Launcher";

		private bool _isLoading;

		public ConfigView()
        {
            InitializeComponent();
			_isLoading = true;
			CBStartWindows.IsChecked = IsAutoStartEnabled();
			_isLoading = false;
		}

		public static bool IsAutoStartEnabled()
		{
			using (var key = Registry.CurrentUser.OpenSubKey(RunKey, false))
			{
				if (key == null)
					return false;

				var value = key.GetValue(ValueName);
				if (value == null)
					return false;

				string exePath = Assembly.GetExecutingAssembly().Location;
				return value.ToString().Contains(exePath);
			}
		}

        private void CBStartWindows_Checked(object sender, RoutedEventArgs e)
        {
			if (_isLoading) return;
			using (var key = Registry.CurrentUser.OpenSubKey(RunKey, true))
			{
				key.SetValue(
					ValueName,
					$"\"{Assembly.GetExecutingAssembly().Location}\" -background"
				);
			}
		}

        private void CBStartWindows_Unchecked(object sender, RoutedEventArgs e)
        {
			if (_isLoading) return;
			using (var key = Registry.CurrentUser.OpenSubKey(RunKey, true))
			{
				if (key.GetValue(ValueName) != null)
					key.DeleteValue(ValueName);
			}
		}
	}
}
