using IWshRuntimeLibrary;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

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

		private void BTNStartMenu_Click(object sender, RoutedEventArgs e)
		{
			string startmenu = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs", "1R1an1");
			string startmenulnk = Path.Combine(startmenu, "Games Launcher.lnk");
			var shell = new WshShell();

			if (!Directory.Exists(startmenu)) Directory.CreateDirectory(startmenu);
			IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(startmenulnk);

			string exePath = Process.GetCurrentProcess().MainModule.FileName;

			Console.WriteLine(startmenulnk);
			Console.WriteLine(Directory.GetCurrentDirectory());

			shortcut.TargetPath = exePath;
			shortcut.WorkingDirectory = Path.GetDirectoryName(exePath);
			shortcut.Description = "Launcher de juegos";
			shortcut.IconLocation = exePath;

			shortcut.Save();
		}
	}
}
