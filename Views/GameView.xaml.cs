using Games_Launcher.Core;
using Games_Launcher.Model;
using Games_Launcher.Windows;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Games_Launcher.Views
{
    /// <summary>
    /// Lógica de interacción para GameView.xaml
    /// </summary>
    public partial class GameView : UserControl
    {
        public GameModel thisGame;
        public Process[] _gameProcess;
        public bool IsRunning = false;
        public DateTime starterTime = DateTime.Now;

        public GameView()
        {
            InitializeComponent();
            DataContextChanged += (s, e) => { if (e.NewValue is GameModel game) { thisGame = game; UpdateInfo(); } };

			Loaded += (_, __) => GameMonitor.Register(this);
			Unloaded += (_, __) => GameMonitor.Unregister(this);
		}

        public void BTNJugar_Click(object sender = null, RoutedEventArgs ea = null)
        {
            if (!IsRunning)
            {
                try
                {
                    Process.Start(new ProcessStartInfo()
                    {
                        FileName = thisGame.Path,
                        Arguments = thisGame.Parameters,
                        UseShellExecute = false,
                        WorkingDirectory = Path.GetDirectoryName(thisGame.Path),
                    });
                    BTNJugar.IsEnabled = false;
                    BTNJugar.Tag = (Brush)FindResource("NormalColorNormal2");
                    BTNJugar.Foreground = (Brush)FindResource("FontColorDisabled2");
                }
                catch (Exception e)
                {
                    MessageBox.Show($"No se pudo iniciar el juego. Verifica que la ruta y los parámetros sean correctos.\n\nError: {e.Message} ({e.HResult})", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (IsRunning && App.window.IsVisible)
                foreach (var process in _gameProcess)
                    process.Kill();


        }
        private void BTNEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"¿Estás seguro de que deseas eliminar {thisGame.Name}?", "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return;

            GamesInfo.Games.Remove(thisGame);
        }


        public void UpdateInfo()
        {
            GameIconIMG.Source = GameFunctions.GetGameIcon(thisGame.Path);
            GameTitleTB.Text = thisGame.Name;
            LBLTimeOppend.Content = GameFunctions.ConvertTime(thisGame.PlayTime);
            LBLLastOppend.Content = GameFunctions.UltimaVezJugado(thisGame.LastPlayed);
        }

        private void GameIconIMG_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ConfigGameWindow window = new ConfigGameWindow(thisGame, this);
            window.ShowDialog();
        }

        private void BTNUp_Click(object sender, RoutedEventArgs e) => GamesInfo.Games.Mover(GamesInfo.Games.IndexOf(thisGame), -1);
        

        private void BTNDown_Click(object sender, RoutedEventArgs e) => GamesInfo.Games.Mover(GamesInfo.Games.IndexOf(thisGame), +1);
        
    }
}
