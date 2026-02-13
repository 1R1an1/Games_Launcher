using Games_Launcher.Model;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Games_Launcher.Views
{
	/// <summary>
	/// Lógica de interacción para MoveGameView.xaml
	/// </summary>
	public partial class MoveGameView : UserControl
	{
		public bool IsMoving = false;
		private long totalTamaño = 0;
		private int totalArchivos = 0;
		private string[] archivos;

		private GameViewModel thisGame => (GameViewModel)DataContext;
		public MoveGameView()
		{
			InitializeComponent();

			RootPathTBX.ToolTip = "Ingresa la carpeta raíz del juego, es decir, la carpeta que contiene el juego completo.\r\n\r\nPor ejemplo:\r\n\r\n- Para Borderlands2.exe en:\r\n  C:\\Borderlands 2\\Binaries\\Win32\\Borderlands2.exe\r\n  → La ruta correcta es: C:\\Borderlands 2\r\n\r\nNo selecciones:\r\n\r\n- Subcarpetas dentro del juego (por ejemplo, Binaries\\Win32)\r\n- Discos raíz (por ejemplo, D:\\)";
			DataContextChanged += (_, __) =>
			{
				MovePathTBX.ToolTip = "Selecciona la carpeta destino donde quieres mover el juego.\r\n\r\nEjemplos:\r\n\r\n- Si seleccionas:\r\n  C:\\Juegos\r\n  → El juego quedará en:\r\n  C:\\Juegos\\{GameFolderName}\r\n\r\n- Si seleccionas:\r\n  C:\\Juegos\\{GameFolderName}\r\n  → El juego quedará en:\r\n  C:\\Juegos\\{GameFolderName}\r\n  (No se creará otra carpeta {GameFolderName} dentro)\r\n\r\nSolo selecciona la carpeta donde quieres que esté el juego.";
				RootPathTBX.Text = ObtenerCarpetaMasParecida(thisGame.Path);
			};
		}

		private async void MoverBTN_Click(object sender, RoutedEventArgs e)
		{
			string rootPath = Path.GetFullPath(RootPathTBX.Text).TrimEnd(Path.DirectorySeparatorChar);
			string movePath = Path.GetFullPath(MovePathTBX.Text).TrimEnd(Path.DirectorySeparatorChar);

			if (!Directory.Exists(RootPathTBX.Text) ||
				!Directory.Exists(MovePathTBX.Text) ||
				!EsRutaValidaParaSeleccion(Path.GetDirectoryName(thisGame.Path), RootPathTBX.Text) ||
				rootPath.Equals(movePath, StringComparison.OrdinalIgnoreCase) ||
				rootPath.StartsWith(movePath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
			{
				MessageBox.Show(Window.GetWindow(this), "Por favor, asegúrate de que ambas rutas sean válidas antes de mover el juego.", "Rutas inválidas", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			string carpetaJuegoDestino = Path.Combine(movePath, new DirectoryInfo(RootPathTBX.Text).Name);
			if (Directory.Exists(carpetaJuegoDestino) && Directory.GetFiles(carpetaJuegoDestino, "*", SearchOption.AllDirectories).Length > 0)
			{
				var result = MessageBox.Show(Window.GetWindow(this),
					$"La carpeta {carpetaJuegoDestino} ya existe y contiene archivos.\n¿Desea continuar y eliminarla AHORA?",
					"Advertencia",
					MessageBoxButton.YesNo,
					MessageBoxImage.Warning);

				if (result != MessageBoxResult.Yes)
					return; // El usuario canceló

				Directory.Delete(carpetaJuegoDestino, true);
			}

			IsMoving = true;
			SetEnabledControl(MoverBTN, !IsMoving);
			SetEnabledControl(CancelarBTN, !IsMoving);
			SetEnabledControl(RootPathTBX, !IsMoving);
			SetEnabledControl(MovePathTBX, !IsMoving);
			ProgressTextBlock.Visibility = Visibility.Visible;

			rootPath = RootPathTBX.Text;
			movePath = MovePathTBX.Text;

			prgbr.IsIndeterminate = true;
			await Task.Run(() => CalcularTotal(rootPath));
			prgbr.Minimum = 0;
			prgbr.Maximum = totalTamaño;
			prgbr.Value = 0;
			prgbr.IsIndeterminate = false;

			var progreso = new Progress<(long, int, string)>(valor =>
			{
				prgbr.Value = valor.Item1;

				long restante = (long)prgbr.Maximum - valor.Item1;

				ProgressTextBlock.Text = $"Moviendo . . . {(double)valor.Item1 / prgbr.Maximum:P2} \n" +
									     $"Archivos restantes: {totalArchivos - valor.Item2} ({Core.FD.FileDownloaderUtils.FormatFileSize(restante)}) \n" +
										 $"Archivo: {valor.Item3}";
			});

			try
			{
			    thisGame.Path = Path.Combine(await Task.Run(() => MoverJuego(rootPath, movePath, progreso)), Path.GetFileName(thisGame.Path));

				MessageBox.Show(Window.GetWindow(this), "El juego se movió correctamente.");
				IsMoving = false;
				SetEnabledControl(CancelarBTN, !IsMoving);
				CancelarBTN.Content = "Cerrar";
			}
			catch (OperationCanceledException)
			{
				MessageBox.Show(Window.GetWindow(this), "Movimiento cancelado.");
			}
		}
		private void CalcularTotal(string gamePath)
		{
			archivos = Directory.GetFiles(gamePath, "*", SearchOption.AllDirectories);
			totalArchivos = archivos.Length;
			foreach (var archivo in archivos)
			{
				totalTamaño += new FileInfo(archivo).Length;
			}
		}
		private string MoverJuego(string gamePath, string gameMovePath, IProgress<(long, int, string)> progreso)
		{
			string nombreCarpetaOrigen = new DirectoryInfo(gamePath).Name;

			string destinoFinal = Path.GetFileName(gameMovePath.TrimEnd(Path.DirectorySeparatorChar))
								  .Equals(nombreCarpetaOrigen, StringComparison.OrdinalIgnoreCase)
								  ? gameMovePath : Path.Combine(gameMovePath, nombreCarpetaOrigen);


			long pesomovido = 0;
			int movidos = 0;

			foreach (var archivo in archivos)
			{
				string relativo = archivo.Substring(gamePath.Length).TrimStart(Path.DirectorySeparatorChar); ;
				string destinoArchivo = Path.Combine(destinoFinal, relativo);

				Directory.CreateDirectory(Path.GetDirectoryName(destinoArchivo)!);

				var tamañoArchivo = new FileInfo(archivo).Length;
				var a = Core.FD.FileDownloaderUtils.FormatFileSize(tamañoArchivo);
				progreso?.Report((pesomovido, movidos, $"{relativo}  ({a})"));

				try
				{
					File.Move(archivo, destinoArchivo);
				}
				catch (IOException)
				{
					if (MessageBox.Show(Window.GetWindow(this), $"Desea remplazar el archivo: {destinoArchivo}\nPor el archivo: {archivo}", "", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
					{
						File.Delete(destinoArchivo);
						File.Move(archivo, destinoArchivo);
					}
				}

				pesomovido += tamañoArchivo;
				movidos++;
				progreso?.Report((pesomovido, movidos, $"{relativo}  ({a})"));
			}

			foreach (var carpeta in Directory.GetDirectories(gamePath, "*", SearchOption.AllDirectories).OrderByDescending(c => c.Length))
			{
				if (Directory.GetFiles(carpeta).Length == 0 && Directory.GetDirectories(carpeta).Length == 0)
				{
					try { Directory.Delete(carpeta, false); }
					catch { }
				}

			}
			try { Directory.Delete(gamePath, false); }
			catch { }
			return destinoFinal;
		}

		public static string GetRelativePath(string basePath, string fullPath)
		{
			if (!basePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
				basePath += Path.DirectorySeparatorChar;

			Uri baseUri = new Uri(basePath);
			Uri fullUri = new Uri(fullPath);

			return Uri.UnescapeDataString(
				baseUri.MakeRelativeUri(fullUri)
					   .ToString()
					   .Replace('/', Path.DirectorySeparatorChar));
		}

		private void CancelarBTN_Click(object sender, RoutedEventArgs e)
		{
			Window.GetWindow(this).Close();
		}
		private void SelectGameRootPathBTN_Click(object sender, RoutedEventArgs e)
		{
			CommonOpenFileDialog dialog = new CommonOpenFileDialog();
			dialog.IsFolderPicker = true;
			dialog.InitialDirectory = ObtenerCarpetaMasParecida(thisGame.Path);
			dialog.Title = "Selecciona la carpeta raiz del juego";

			if (dialog.ShowDialog() == CommonFileDialogResult.Ok && Directory.Exists(dialog.FileName))
			{
				if (EsRutaValidaParaSeleccion(Path.GetDirectoryName(thisGame.Path), dialog.FileName))
				{
					RootPathTBX.Text = dialog.FileName;
				}
				else
				{
					MessageBox.Show(Window.GetWindow(this), "La ruta seleccionada no es valida", "error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			Window.GetWindow(this).Focus();
		}

		private void SelectGameMovePathBTN_Click(object sender, RoutedEventArgs e)
		{
			CommonOpenFileDialog dialog = new CommonOpenFileDialog();
			dialog.IsFolderPicker = true;
			dialog.Title = "Selecciona la carpeta para mover el juego";

			if (dialog.ShowDialog() == CommonFileDialogResult.Ok && Directory.Exists(dialog.FileName))
			{
				MovePathTBX.Text = dialog.FileName;
			}
			Window.GetWindow(this).Focus();
		}

		bool EsRutaValidaParaSeleccion(string rutaBase, string rutaVerificar)
		{
			// Normalizamos rutas
			rutaBase = Path.GetFullPath(rutaBase).TrimEnd(Path.DirectorySeparatorChar);
			rutaVerificar = Path.GetFullPath(rutaVerificar).TrimEnd(Path.DirectorySeparatorChar);

			// Primero, rechazamos discos directamente
			if (Path.GetPathRoot(rutaVerificar).Equals(rutaVerificar, StringComparison.OrdinalIgnoreCase))
				return false;

			// True si la carpeta seleccionada es la misma que la ruta base
			if (rutaVerificar.Equals(rutaBase, StringComparison.OrdinalIgnoreCase))
				return true;

			// True si la carpeta está por encima de la ruta base
			return rutaBase.StartsWith(rutaVerificar + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
		}
		string ObtenerCarpetaMasParecida(string rutaExe)
		{
			if (string.IsNullOrEmpty(rutaExe))
				return null;

			// Nombre del exe sin extensión
			string nombreExe = Path.GetFileNameWithoutExtension(rutaExe);

			// Carpeta que contiene el exe
			string carpetaActual = Path.GetDirectoryName(rutaExe);
			string mejorCoincidente = carpetaActual;
			int maxCoincidencia = 2;

			while (!string.IsNullOrEmpty(carpetaActual))
			{
				string nombreCarpeta = Path.GetFileName(carpetaActual);

				// Contamos cuántos caracteres coinciden desde el principio
				int coincidencia = ContarCoincidenciaInicial(nombreExe, nombreCarpeta);

				if (coincidencia > maxCoincidencia)
				{
					maxCoincidencia = coincidencia;
					mejorCoincidente = carpetaActual;
				}

				carpetaActual = Path.GetDirectoryName(carpetaActual); // subimos un nivel
			}

			return mejorCoincidente;
		}
		int ContarCoincidenciaInicial(string a, string b)
		{
			a = a.ToLower();
			b = b.ToLower();
			int count = 0;
			int minLen = Math.Min(a.Length, b.Length);
			for (int i = 0; i < minLen; i++)
			{
				if (a[i] == b[i])
					count++;
				else
					break;
			}
			return count;
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
