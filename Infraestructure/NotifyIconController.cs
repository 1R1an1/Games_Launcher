using System;
using System.Diagnostics;
using System.Drawing;
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

			var menu = new ContextMenuStrip();
			menu.Items.Add("Abrir", null, (s, e) => ShowRequested?.Invoke());
			menu.Items.Add("-");
			menu.Items.Add("Salir", null, (s, e) => ExitRequested?.Invoke());

			_icon.ContextMenuStrip = menu;
			_icon.DoubleClick += (s, e) => ShowRequested?.Invoke();
		}

		public void Dispose()
		{
			_icon.Visible = false;
			_icon.Dispose();
		}

		public void HideNIcon() => _icon.Visible = false;
		public void ShowNIcon() => _icon.Visible = true;
	}
}
