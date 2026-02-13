using FortiCrypts;
using Games_Launcher.Core;
using Games_Launcher.Infraestructure;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Games_Launcher
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static MainWindow window;
        private bool EnableAutoSave = false;

        private SingleInstanceManager _instance;
        private static WindowController _windowController;
        private static NotifyIconController _notifyIcon;

        protected override void OnStartup(StartupEventArgs e)
        {

			if (!InitializeSingleInstance())
				return;

			base.OnStartup(e);

			InitializeCore();
			InitializeWindow();
			InitializeTray();
			HandleStartupArgs(e.Args);

			MainWindow = window;
			Current.Exit += Current_Exit;

			GameMonitor.StartLoop();
			StartAutoSave();
		}

        public static void Show() { _windowController.Show(); _notifyIcon.HideNIcon(); }
        public static void Hide() { _windowController.Hide(); _notifyIcon.ShowNIcon(); }

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            window.InvokeEvent();
            GamesInfo.SaveGamesData();
            _notifyIcon.Dispose();
        }
        public static void UpdateNIcons() => _notifyIcon.UpdateNICons();


		private bool InitializeSingleInstance()
		{
			_instance = new SingleInstanceManager("GamesLauncher_SingleInstance", "GamesLauncher_ShowWindow", out bool isSecondInstance);

			if (isSecondInstance)
				return false;

			_instance.Listen(Show);
			return true;
		}

		private void InitializeCore()
		{
			CryptoUtils.iterations = 2500;
#if !DEBUG
			GamesInfo.CheckFileNames();
#endif
			GamesInfo.LoadGamesData();
		}

		private void InitializeWindow()
		{
			window = new MainWindow();
			_windowController = new WindowController(window);
		}

		private void InitializeTray()
		{
			_notifyIcon = new NotifyIconController();
			_notifyIcon.ShowRequested += Show;
			_notifyIcon.ExitRequested += Shutdown;
		}

		private void HandleStartupArgs(string[] args)
		{
			if (args.Any(a => a.Equals("-background", StringComparison.OrdinalIgnoreCase)))
			{
				Hide();
				_notifyIcon.ShowNIcon();
			}
			else
			{
				window.Show();
				_notifyIcon.HideNIcon();
			}
		}

		private void StartAutoSave()
		{
			EnableAutoSave = true;

			_ = Task.Run(async () =>
			{
				while (true)
				{
					await Task.Delay(TimeSpan.FromMinutes(1));
					if (EnableAutoSave)
						GamesInfo.SaveGamesData();
				}
			});
		}
	}
}