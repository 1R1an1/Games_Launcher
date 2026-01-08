using FortiCrypts;
using Games_Launcher.Core;
using System.Linq;
using System.Threading;
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

		private static Mutex _mutex;
		private static EventWaitHandle _showEvent;

		private const string MutexName = "GamesLauncher_SingleInstance";
		private const string ShowEventName = "GamesLauncher_ShowWindow";

		protected override void OnStartup(StartupEventArgs e)
        {
			bool createdNew;
			_mutex = new Mutex(true, MutexName, out createdNew);

			if (!createdNew)
			{
				NotifyInstance();
				Shutdown();
				return;
			}


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
            if (e.Args.Contains("-background"))
                Ocultar();
            else
                window.Show();

            MainWindow = window;
            App.Current.Exit += Current_Exit;

            GameMonitor.StartLoop();
            EnableAutoSave = true;

            CreateListener();

            //reference windoww = new reference();
            ////MainWindow = windoww;
            //windoww.Show();
        }

        private void CreateListener()
        {
            _showEvent = new EventWaitHandle(false, EventResetMode.AutoReset, ShowEventName);

			Task.Run(() =>
			{
				while (true)
				{
					_showEvent.WaitOne();
                    Mostrar();
				}
			});
		}
        private void NotifyInstance()
        {
			EventWaitHandle.OpenExisting(ShowEventName).Set();
		}

        private void Mostrar()
        {
			Dispatcher.Invoke(() =>
			{
				window.ShowInTaskbar = true;
				window.Visibility = Visibility.Visible;
				window.WindowState = WindowState.Normal;
				window.Activate();
			});
		}

        private void Ocultar()
        {
			window.ShowInTaskbar = false;
			window.WindowState = WindowState.Minimized;
			window.Visibility = Visibility.Hidden;
            window.Hide();
		}

		private void Current_Exit(object sender, ExitEventArgs e)
        {
            window.InvokeEvent();
            GamesInfo.SaveGamesData();
        }
    }
}