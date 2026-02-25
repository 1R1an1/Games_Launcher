using Games_Launcher.Core;
using Games_Launcher.Model;
using Games_Launcher.Windows;
using System;
using System.ComponentModel;
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
		public GameModel thisGame => (GameModel)DataContext;
		public GameViewModel _viewModel { get; set; }

		public GameView()
        {
            InitializeComponent();
            Init();
		}
        private void Init()
        {
            DataContextChanged += (_, e) =>
            {
				if (e.OldValue is GameModel oldGame)
					oldGame.PropertyChanged -= Game_PropertyChanged;

				if (e.NewValue is GameModel newGame)
				{
                    _viewModel = new GameViewModel(newGame);
					newGame.PropertyChanged += Game_PropertyChanged;

					if (newGame.IsRunning)
						ChangeToStopBTN();
					else
						ChangeToPlayBTN();
				}
            };
		}

		private void Game_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(GameModel.IsRunning))
			{
				Dispatcher.Invoke(() =>
				{
					if (thisGame.IsRunning)
						ChangeToStopBTN();
					else
						ChangeToPlayBTN();
				});
			}
		}

		public void BTNJugar_Click(object sender = null, RoutedEventArgs ea = null)
        {
            if (!thisGame.IsRunning)
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
            else if (thisGame.IsRunning && App.window.IsVisible)
                foreach (var process in Process.GetProcessesByName(thisGame.ProcessName))
                    process.Kill();


        }
        private void BTNEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"¿Estás seguro de que deseas eliminar {thisGame.Name}?", "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return;

            GamesInfo.Games.Remove(thisGame);
        }

        private void GameIconIMG_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ConfigGameWindow window = new ConfigGameWindow(_viewModel);
            window.ShowDialog();
        }

        private void BTNUp_Click(object sender, RoutedEventArgs e) => GamesInfo.Games.Mover(GamesInfo.Games.IndexOf(thisGame), -1);
        private void BTNDown_Click(object sender, RoutedEventArgs e) => GamesInfo.Games.Mover(GamesInfo.Games.IndexOf(thisGame), +1);
        
        private void ChangeToPlayBTN()
        {
			Dispatcher.Invoke(() =>
			{
				BTNJugar.Margin = new Thickness(31.65, 0, 20.55, 0);
				BTNJugar.IsEnabled = true;
				BTNJugar.Content = "JUGAR";
				BTNJugar.Foreground = Brushes.White;
				BTNJugar.Tag = FindResource("JugarColorNormal");
				BTNJugar.BorderBrush = (Brush)FindResource("JugarColorMouseOver");
				LBLTimeOppend.Content = GameFunctions.ConvertTime(thisGame.PlayTime);
			});
		}

        private void ChangeToStopBTN()
        {
			Dispatcher.Invoke(() =>
			{
				BTNJugar.Margin = new Thickness(22.3, 0, 11.2, 0);
				BTNJugar.IsEnabled = true;
				BTNJugar.Content = "DETENER";
				BTNJugar.Foreground = Brushes.White;
				BTNJugar.Tag = FindResource("DownloadColorNormal");
				BTNJugar.BorderBrush = (Brush)FindResource("DownloadColorMouseOver");
				LBLLastOppend.Content = GameFunctions.UltimaVezJugado(thisGame.LastPlayed);
				App.UpdateNIcons();
			});
		}
    }
}
