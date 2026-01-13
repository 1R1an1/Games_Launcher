using Games_Launcher.Core;
using Games_Launcher.Views;
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

		public NotifyIconController()
		{
			_icon = new NotifyIcon
			{
				Icon = Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule.FileName),
				Text = "Games Launcher",
				Visible = true
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
				menu.Items.Add(item.Name, Icon.ExtractAssociatedIcon(item.Path).ToBitmap(),
					(s, e) =>
					{
						var index = GamesInfo.Games.IndexOf(item);
						var gameView = App.window.CDU_Window.Juegos.Children[index] as GameView;
						if (gameView != null)
						{
							gameView.BTNJugar_Click();
						}
					}
				);
			}
			menu.Items.Add("-");
			menu.Items.Add("Abrir", null, (s, e) => ShowRequested?.Invoke());
			menu.Items.Add("Salir", null, (s, e) => ExitRequested?.Invoke());

			_icon.ContextMenuStrip = menu;
		}
	}
}
