using FortiCrypts;
using Games_Launcher.Core;
using Games_Launcher.Infraestructure;
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
		private WindowController _windowController;

		protected override void OnStartup(StartupEventArgs e)
        {
            _instance = new SingleInstanceManager("GamesLauncher_SingleInstance", "GamesLauncher_ShowWindow", out bool i);
            if (i) return;

			base.OnStartup(e);
            CryptoUtils.iterations = 2500;
            GamesInfo.CheckFileNames();
            GamesInfo.LoadGamesData();

            _ = Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(60000);
                    if (EnableAutoSave)
                        GamesInfo.SaveGamesData();
                }
            });

            window = new MainWindow();
			_windowController = new WindowController(window);
			if (e.Args.Contains("-background"))
                _windowController.Hide();
            else
                window.Show();

            MainWindow = window;
            App.Current.Exit += Current_Exit;

            GameMonitor.StartLoop();
            EnableAutoSave = true;

			_instance.Listen(_windowController.Show);
        }

		private void Current_Exit(object sender, ExitEventArgs e)
        {
            window.InvokeEvent();
            GamesInfo.SaveGamesData();
        }
    }
}