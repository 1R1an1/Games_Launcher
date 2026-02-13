using Games_Launcher.Model;
using System.Windows;
using System.Windows.Input;

namespace Games_Launcher.Windows
{
	/// <summary>
	/// Lógica de interacción para MoveGameWindow.xaml
	/// </summary>
	public partial class MoveGameWindow : Window
	{
		public MoveGameWindow(GameViewModel game)
		{
			InitializeComponent();
			borde1.Visibility = Visibility.Visible;
			CDU_Window.DataContext = game;
		}

		private void MoveWindow(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
				DragMove();
		}
		private void b_cerrar_Click(object sender, RoutedEventArgs e)
		{
            if (!CDU_Window.IsMoving)
				Close();
		}
		private void b_minimizar_Click(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}
	}
}
