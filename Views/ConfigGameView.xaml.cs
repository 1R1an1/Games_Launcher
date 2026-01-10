using Games_Launcher.Core;
using Games_Launcher.Model;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Games_Launcher.Views
{
    /// <summary>
    /// Lógica de interacción para ConfigGameView.xaml
    /// </summary>
    public partial class ConfigGameView : UserControl
    {
        private GameModel _thisGame;
        private GameView _thisGameView;
        public bool isMoving;

        public ConfigGameView(GameModel game, GameView gameView)
        {
            _thisGame = game;
            _thisGameView = gameView;
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            GamePathTBX.Text = _thisGame.Path;
            GameNameTBX.Text = _thisGame.Name;
            GameParametersTBX.Text = _thisGame.Parameters;
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
            _thisGame.ProcessName = Path.GetFileNameWithoutExtension(GamePathTBX.Text);
            //App.window.CDU_Window..UpdateGames();
            _thisGameView.UpdateInfo();
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
            if (MessageBox.Show(Window.GetWindow(this), "¿Estas seguro que deseas mover el juego de carpeta? \nSolo se moveran los archivos del juego a otra carpeta, nada mas.", "ADVERTENCIA", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return;

            MessageBox.Show(Window.GetWindow(this), "Es necesario que selecciones la carpeta donde se almacena el juego.", "Mover juego", MessageBoxButton.OK, MessageBoxImage.Information);
            string gamePath = SeleccionarCarpeta("Carpeta del juego");
			if (gamePath == null) return;

            MessageBox.Show(Window.GetWindow(this), "Ahora selecciona la carpeta a donde quieres que se mueva el juego.", "Mover juego", MessageBoxButton.OK, MessageBoxImage.Information);
            string gameMovePath;
			while (true)
            {
                gameMovePath = SeleccionarCarpeta("Carpeta para mover al juego");
				if (gameMovePath == null) return;
                if (VerificarCarpetas(gamePath, gameMovePath))
                    MessageBox.Show(Window.GetWindow(this), "La carpeta destino no puede estar dentro de la carpeta del juego ni ser la misma.", "Mover juego", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    break;
			}

			Window.GetWindow(this).Focus();
			SetEnabledControl(CancelarBTN, false);
			SetEnabledControl(AplicarBTN, false);
			SetEnabledControl(MoverBTN, false);
			isMoving = true;

            bprgbr.Visibility = Visibility.Visible;
            prgbr.IsIndeterminate = true;

			long tamañoTotal = 0;
			long movidos = 0;

			// Crear una lista de archivos en paralelo, pero mover mientras se agregan
			var archivos = new ConcurrentQueue<string>();

			// Tarea que va descubriendo archivos y sumando tamaños
			var producer = Task.Run(() =>
			{
				foreach (var archivo in Directory.GetFiles(gamePath, "*", SearchOption.AllDirectories))
				{
					archivos.Enqueue(archivo);
					try
					{
						tamañoTotal += new FileInfo(archivo).Length;
					}
					catch { }
				}
			});

			string nombreCarpetaOrigen = new DirectoryInfo(gamePath).Name;

			// Detectar si el destino ya termina con el nombre de la carpeta
			string destinoFinal;
			if (Path.GetFileName(gameMovePath.TrimEnd(Path.DirectorySeparatorChar)) == nombreCarpetaOrigen)
			{
				// El destino ya incluye la carpeta origen, no agregamos nada
				destinoFinal = gameMovePath;
			}
			else
			{
				// El destino no incluye la carpeta origen, la añadimos
				destinoFinal = Path.Combine(gameMovePath, nombreCarpetaOrigen);
			}
			// Tarea que va moviendo archivos mientras se descubren
			var consumer = Task.Run(() =>
			{
				while (!producer.IsCompleted || archivos.Count > 0)
				{
					if (archivos.TryDequeue(out string archivo))
					{
						string destinoArchivo = archivo.Replace(gamePath, destinoFinal);
						Directory.CreateDirectory(Path.GetDirectoryName(destinoArchivo));
						File.Move(archivo, destinoArchivo);

						try
						{
							movidos += new FileInfo(destinoArchivo).Length;
						}
						catch { }

						// Actualizar progressbar en UI
						Application.Current.Dispatcher.Invoke(() =>
						{
							if (tamañoTotal > 0)
							{
								prgbr.IsIndeterminate = false;
								prgbr.Maximum = tamañoTotal;
								prgbr.Value = movidos;
							}
						});
					}
					else
					{
						Thread.Sleep(50); // esperar un poco si no hay archivos aún
					}
				}
			});

			await Task.WhenAll(producer, consumer);

			// Borrar carpetas vacías
			var carpetas = Directory.GetDirectories(gamePath, "*", SearchOption.AllDirectories);

			// Ordenar de la más profunda a la menos profunda
			Array.Sort(carpetas, (a, b) => b.Length.CompareTo(a.Length));

			foreach (var carpeta in carpetas)
			{
				try { Directory.Delete(carpeta, false); }
				catch { }
			}

			try { Directory.Delete(gamePath, false); }
			catch { }

			string carpetaJuegoOriginal = Path.GetFullPath(gamePath);
			GamePathTBX.Text = _thisGame.Path.Replace(carpetaJuegoOriginal, destinoFinal);

			MessageBox.Show(Window.GetWindow(this), "El juego se ha movido correctamente.", "Mover juego", MessageBoxButton.OK, MessageBoxImage.Information);
			SetEnabledControl(AplicarBTN, true);
			isMoving = false;
			bprgbr.Visibility = Visibility.Collapsed;
		}

		private string SeleccionarCarpeta(string titulo)
		{
			while (true)
			{
				CommonOpenFileDialog dialog = new CommonOpenFileDialog();
				dialog.InitialDirectory = Path.GetDirectoryName(_thisGame.Path);
				dialog.Title = titulo;
				dialog.IsFolderPicker = true;
				var d = dialog.ShowDialog();

				if (d == CommonFileDialogResult.Ok && Directory.Exists(dialog.FileName))
				{
					return dialog.FileName;
				}
				else if (d != CommonFileDialogResult.Ok)
				{
					MessageBox.Show(Window.GetWindow(this), "Se ha cancelado.", "Mover juego", MessageBoxButton.OK, MessageBoxImage.Information);
					return null;
				}
				else
				{
					MessageBox.Show(Window.GetWindow(this), "La carpeta seleccionada no existe.", "Mover juego", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
		}

		private bool VerificarCarpetas(string padre, string hijo)
		{
			string rutaPadre = Path.GetFullPath(padre.Replace('/', Path.DirectorySeparatorChar))
									.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
			string rutaHijo = Path.GetFullPath(hijo.Replace('/', Path.DirectorySeparatorChar))
									.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;

			return rutaHijo.StartsWith(rutaPadre, StringComparison.OrdinalIgnoreCase);
		}

		private void SetEnabledControl(Control element, bool SetEnabledValue)
		{
			if (!SetEnabledValue)
			{
				element.IsEnabled = false;
				element.Tag = (Brush)FindResource("NormalColorDisabled");
				element.Foreground = (Brush)FindResource("FontColorDisabled");
			}
			else
			{
				element.IsEnabled = true;
				element.Tag = (Brush)FindResource("NormalColorNormal");
				element.Foreground = (Brush)FindResource("FontColor");
			}
		}
	}
}
