using Games_Launcher.Core;
using Games_Launcher.Windows;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Games_Launcher.Infraestructure
{
    public class NotifyIconController : IDisposable
    {
        private readonly NotifyIcon _icon;

        public event Action ShowRequested;
        public event Action ExitRequested;
	    private T Try<T>(Func<T> f) { try { return f(); } catch { return default; } }

		public NotifyIconController()
		{
			_icon = new NotifyIcon
			{
				Icon = Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule.FileName),
				Text = "Games Launcher",
				Visible = false
			};

			UpdateNICons();
			
			_icon.DoubleClick += (s, e) => ShowRequested?.Invoke();
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
				menu.Items.Add(item.Name, Try(() => Icon.ExtractAssociatedIcon(item.Path)?.ToBitmap()) ?? new Bitmap(App.GetResourceStream(new Uri("pack://application:,,,/Img/ErrorImg.png")).Stream),
					(s, e) =>
					{
						var gameView = GameFunctions.GetGameViewFromItem(App.window.CDU_Window.GamesItemsControl, item);
						if (gameView != null)
						{
							gameView.BTNJugar_Click();
						}
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
