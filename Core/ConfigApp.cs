using IWshRuntimeLibrary;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Games_Launcher.Core
{
	public class ConfigApp
	{
		private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
		private const string ValueName = "Games Launcher";

		public static bool IsAutoStartEnabled()
		{
			using var key = Registry.CurrentUser.OpenSubKey(RunKey, false);
			if (key == null) return false;
			var value = key.GetValue(ValueName);
			if (value == null) return false;
			string exePath = Assembly.GetExecutingAssembly().Location;
			return value.ToString().Contains(exePath);
		}

		public static void EnableAutoStart()
		{
			using var key = Registry.CurrentUser.OpenSubKey(RunKey, true);
			key.SetValue(ValueName, $"\"{Assembly.GetExecutingAssembly().Location}\" -background");
		}

		public static void DisableAutoStart()
		{
			using var key = Registry.CurrentUser.OpenSubKey(RunKey, true);
			if (key.GetValue(ValueName) != null)
				key.DeleteValue(ValueName);
		}

		public static void CreateStartMenuShortcut()
		{
			string startmenu = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs", "1R1an1");
			string startmenulnk = Path.Combine(startmenu, "Games Launcher.lnk");
			var shell = new WshShell();
			if (!Directory.Exists(startmenu)) Directory.CreateDirectory(startmenu);

			IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(startmenulnk);
			string exePath = Process.GetCurrentProcess().MainModule.FileName;

			shortcut.TargetPath = exePath;
			shortcut.WorkingDirectory = Path.GetDirectoryName(exePath);
			shortcut.Description = "Launcher de juegos";
			shortcut.IconLocation = exePath;
			shortcut.Save();
		}
	}
}
