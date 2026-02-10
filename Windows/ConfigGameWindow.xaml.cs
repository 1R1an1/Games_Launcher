using Games_Launcher.Model;
using System.Windows;
using System.Windows.Input;

namespace Games_Launcher.Windows
{
    /// <summary>
    /// Lógica de interacción para MainConfigGameWindowWindow.xaml
    /// </summary>
    public partial class ConfigGameWindow : Window
    {
        public ConfigGameWindow(GameViewModel game)
        {
            InitializeComponent();
            CDU_Window.DataContext = game;
			borde1.Visibility = Visibility.Visible;
		}

		private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        private void b_cerrar_Click(object sender, RoutedEventArgs e)
        {
            if (!CDU_Window.isMoving)
                Close();
        }
        private void b_minimizar_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
