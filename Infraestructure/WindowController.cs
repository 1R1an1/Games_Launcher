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
				if (!_window.IsVisible)
					_window.Show();

				_window.ShowInTaskbar = true;
				
				_window.WindowState = WindowState.Normal;
				_window.UpdateLayout();
				_window.InvalidateVisual();

				_window.Topmost = true;
				_window.Activate();
				_window.Topmost = false;
			});
		}

		public void Hide()
		{
			_window.Dispatcher.Invoke(() =>
			{
				_window.ShowInTaskbar = false;
				_window.Hide();
			});
		}
	}
}
