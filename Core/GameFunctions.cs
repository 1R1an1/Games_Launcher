using Games_Launcher.Views;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Games_Launcher.Core
{
    public static class GameFunctions
    {
        public static BitmapImage GetGameIcon(string gamePath)
        {
            if (string.IsNullOrEmpty(gamePath) || !File.Exists(gamePath))
                return new BitmapImage(new Uri("pack://application:,,,/Img/ErrorImg.png"));

            System.Drawing.Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(gamePath);

            using (MemoryStream ms = new MemoryStream())
            {
                icon.ToBitmap().Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Seek(0, SeekOrigin.Begin);

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = ms;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();

                return image;
            }
        }
        public static string ConvertTime(TimeSpan time)
        {
            if (time.TotalSeconds > 100)
            {
                if (time.TotalMinutes > 100)
                    return $"{time.TotalHours:F1} horas";
                else
                    return $"{time.TotalMinutes:F0} minutos";
                
            }
            else
            {
                if (time.TotalSeconds < 1)
                    return "N/A";
                return $"{time.TotalSeconds:F0} segundos";
            }
            
        }

        public static bool? SelectGamePath(out string path)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Executable Files (*.exe)|*.exe";
            dialog.Title = "Selecciona un archivo ejecutable";

            bool? a = dialog.ShowDialog();
            path = dialog.FileName;

            return a;
        }

        public static string UltimaVezJugado(DateTime fechaInicio)
        {
            DateTime hoy = DateTime.Today;
            DateTime ayer = hoy.AddDays(-1);


            if (fechaInicio == new DateTime() || fechaInicio == null)
                return "N/A";
            else if (fechaInicio.Date == hoy)
                return "Hoy";
            else if (fechaInicio.Date == ayer)
                return "Ayer";
            else
                return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(fechaInicio.ToString("dd MMM yyyy", new CultureInfo("es-ES")).Replace(".", ""));
        }

		public static GameView GetGameViewFromItem(ItemsControl itemsControl, object item)
		{
			if (itemsControl == null || item == null)
				return null;

			var a = itemsControl.ItemContainerGenerator.ContainerFromItem(item) as ContentPresenter;

            if (a != null)
            {
                return FindVisualChild<GameView>(a);
            }
            else return null;
		}
		private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
			{
				var child = VisualTreeHelper.GetChild(parent, i);

				if (child is T typedChild)
					return typedChild;

				var result = FindVisualChild<T>(child);
				if (result != null)
					return result;
			}
			return null;
		}

	}

	public static class ObservableCollectionExtensions
	{
		public static void Mover<T>(this ObservableCollection<T> list, int index, int direccion)
		{
			// direccion: -1 = subir, +1 = bajar
			if (list == null || index < 0 || index >= list.Count)
				return; // índice fuera de rango

			int nuevoIndice = index + direccion;

			if (nuevoIndice < 0 || nuevoIndice >= list.Count)
				return; // no hacemos nada si se sale del rango

			// Intercambiar elementos
			list.Move(index, nuevoIndice);
		}
	}
}
