using Games_Launcher.Core;
using Games_Launcher.Model;
using Games_Launcher.Windows;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using W = System.Windows;

namespace Games_Launcher.Infraestructure
{
    public class NotifyIconController : IDisposable
    {
        private readonly NotifyIcon _icon;

        public event Action ShowRequested;
        public event Action ExitRequested;

		public NotifyIconController()
		{
			_icon = new NotifyIcon
			{
				Icon = Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule.FileName),
				Text = "Games Launcher",
				Visible = false
			};

			// Suscribirse a los juegos existentes
			foreach (var game in GamesInfo.Games)
				game.PropertyChanged += Game_PropertyChanged;

			// Si la lista cambia (se agrega o quita un juego)
			GamesInfo.Games.CollectionChanged += (s, e) =>
			{
				if (e.NewItems != null)
					foreach (GameModel g in e.NewItems)
						g.PropertyChanged += Game_PropertyChanged;

				if (e.OldItems != null)
					foreach (GameModel g in e.OldItems)
						g.PropertyChanged -= Game_PropertyChanged;

				UpdateNICons();
			};

			UpdateNICons();
			_icon.MouseClick += (s, e) => { if (e.Button == MouseButtons.Left) ShowRequested?.Invoke(); };
		}

		private void Game_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(GameModel.LastPlayed) || e.PropertyName == nameof(GameModel.Name)|| e.PropertyName == nameof(GameModel.Path))
				UpdateNICons();
		}

		public void Dispose()
		{
			_icon.Visible = false;
			_icon.Dispose();
		}

		public void HideNIcon() => _icon.Visible = false;
		public void ShowNIcon() => _icon.Visible = true;
		public void UpdateNICons()
		{
			var menu = new ContextMenuStrip();
			foreach (var item in GamesInfo.Games.OrderByDescending(x => x.LastPlayed).Take(5).ToList())
			{
				menu.Items.Add(
					item.Name,
					GameFunctions.Try(() => Icon.ExtractAssociatedIcon(item.Path)?.ToBitmap()) ?? new Bitmap(App.GetResourceStream(new Uri("pack://application:,,,/Img/ErrorImg.png")).Stream),
					(s, e) =>
					{
						try { GameController.StartGame(item); } catch (Exception ex) { W.MessageBox.Show($"No se pudo iniciar el juego. Verifica que la ruta y los parámetros sean correctos.\n\nError: {ex.Message} ({ex.HResult})", "Error", W.MessageBoxButton.OK, W.MessageBoxImage.Error); }
					}
				);
			}
			menu.Items.Add("-");


			menu.Items.Add(new ToolStripMenuItem("Abrir", null, (s, e) => ShowRequested?.Invoke()) { ToolTipText = "Abre la ventana principal" });
			menu.Items.Add(new ToolStripMenuItem("File Downloader", null, (s, e) =>
			{
				var fdWindow = new FileDownloaderWindow();
				fdWindow.Show();
				fdWindow.Focus();
			}) { ToolTipText = "Abre la ventana de file downloader" });
			menu.Items.Add(new ToolStripMenuItem("Salir", null, (s, e) => ExitRequested?.Invoke()) { ToolTipText = "Cierra la aplicacion" });

			_icon.ContextMenuStrip = menu;
		}
	}
}
