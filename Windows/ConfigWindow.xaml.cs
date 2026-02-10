using System.Windows;
using System.Windows.Input;

namespace Games_Launcher.Windows
{
    /// <summary>
    /// Lógica de interacción para ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        public ConfigWindow()
        {
            InitializeComponent();
			borde1.Visibility = Visibility.Visible;
        }

		private void MoveWindow(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
				DragMove();
		}
		private void b_cerrar_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		private void b_minimizar_Click(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}
	}
}
