using Games_Launcher.Core;
using Games_Launcher.Model;
using Games_Launcher.Windows;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Games_Launcher.Views
{
    /// <summary>
    /// Lógica de interacción para Window.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void BTNAgregar_Click(object sender, RoutedEventArgs e)
        {
            if (GameFunctions.SelectGamePath(out string path) == true)
            {
                GameModel newGame = new GameModel
                {
                    Name = Path.GetFileNameWithoutExtension(path),
                    ProcessName = Path.GetFileNameWithoutExtension(path),
                    Path = path,
                    Parameters = "",
                    PlayTime = new TimeSpan(0)
                };
                GamesInfo.Games.Add(newGame);
            }
        }

        private void BTNOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", AppDomain.CurrentDomain.BaseDirectory);
        }

        private void BTNFileDownloader_Click(object sender, RoutedEventArgs e)
        {
            new FileDownloaderWindow().Show();
        }

        private void BTNConfig_Click(object sender, RoutedEventArgs e)
        {
            var configWindow = new ConfigWindow();
            configWindow.ShowDialog();
		}
	}
}
