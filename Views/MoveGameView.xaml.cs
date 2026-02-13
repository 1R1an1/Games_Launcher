using Games_Launcher.Model;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Games_Launcher.Views
{
	/// <summary>
	/// Lógica de interacción para MoveGameView.xaml
	/// </summary>
	public partial class MoveGameView : UserControl
	{
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

		private void MoverBTN_Click(object sender, RoutedEventArgs e)
		{

		}

		private void CancelarBTN_Click(object sender, RoutedEventArgs e)
		{

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
					MessageBox.Show("La ruta seleccionada no es valida", "error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
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

	}
}
