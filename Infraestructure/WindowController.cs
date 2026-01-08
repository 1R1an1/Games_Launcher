using System.Windows;

namespace Games_Launcher.Infraestructure
{
    public class WindowController
    {
		private readonly Window _window;

		public WindowController(Window window)
		{
			_window = window;
		}

		public void Show()
		{
			_window.Dispatcher.Invoke(() =>
			{
				_window.ShowInTaskbar = true;
				_window.Visibility = Visibility.Visible;
				_window.WindowState = WindowState.Normal;
				_window.Activate();
			});
		}

		public void Hide()
		{
			_window.ShowInTaskbar = false;
			_window.WindowState = WindowState.Minimized;
			_window.Visibility = Visibility.Hidden;
			_window.Hide();
		}
	}
}
