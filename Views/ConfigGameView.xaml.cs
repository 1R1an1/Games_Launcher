using Games_Launcher.Core;
using Games_Launcher.Model;
using Games_Launcher.Windows;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Games_Launcher.Views
{
    /// <summary>
    /// Lógica de interacción para ConfigGameView.xaml
    /// </summary>
    public partial class ConfigGameView : UserControl
    {
        private GameViewModel _thisGame => (GameViewModel)DataContext;
        public bool isMoving;

        public ConfigGameView()
        {
            InitializeComponent();
		}


        private void SelectGamePathBTN_Click(object sender, RoutedEventArgs e)
        {
            if (GameFunctions.SelectGamePath(out string path) == true)
                GamePathTBX.Text = path;
        }
        private void AplicarBTN_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(GamePathTBX.Text) || !GamePathTBX.Text.EndsWith(".exe"))
            {
                MessageBox.Show("La ruta del juego no es válida.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _thisGame.Parameters = GameParametersTBX.Text;
            _thisGame.Name = GameNameTBX.Text;
            _thisGame.Path = GamePathTBX.Text;
            _thisGame._game.ProcessName = Path.GetFileNameWithoutExtension(GamePathTBX.Text);
            Window.GetWindow(this).Close();
        }
        private void CancelarBTN_Click(object sender, RoutedEventArgs e) => Window.GetWindow(this).Close();
        private void GamePathTBX_TextChanged(object sender, TextChangedEventArgs e)
        {
            GameIconIMG.Source = GameFunctions.GetGameIcon(GamePathTBX.Text);
        }
        private void OpenGamePathBTN_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(Path.GetDirectoryName(_thisGame.Path)))
            {
                MessageBox.Show("La carpeta del juego no fue encontrada", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Process.Start("explorer.exe", Path.GetDirectoryName(_thisGame.Path));
        }

		private async void MoverBTN_Click(object sender, RoutedEventArgs e)
		{
			new MoveGameWindow(_thisGame).ShowDialog();

		}
	}
}
