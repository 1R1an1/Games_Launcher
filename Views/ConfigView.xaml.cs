using Games_Launcher.Core;
using System.Windows;
using System.Windows.Controls;

namespace Games_Launcher.Views
{
	/// <summary>
	/// Lógica de interacción para ConfigView.xaml
	/// </summary>
	public partial class ConfigView : UserControl
	{
		private bool _isLoading;

		public ConfigView()
		{
			InitializeComponent();
			_isLoading = true;
			CBStartWindows.IsChecked = ConfigApp.IsAutoStartEnabled();
			_isLoading = false;
		}

		private void CBStartWindows_Checked(object sender, RoutedEventArgs e)
		{
			if (_isLoading) return;
			ConfigApp.EnableAutoStart();
		}

		private void CBStartWindows_Unchecked(object sender, RoutedEventArgs e)
		{
			if (_isLoading) return;
			ConfigApp.DisableAutoStart();
		}

		private void BTNStartMenu_Click(object sender, RoutedEventArgs e) => ConfigApp.CreateStartMenuShortcut();
	}
}
