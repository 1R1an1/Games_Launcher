using Games_Launcher.Model;
using System.Diagnostics;
using System.IO;

namespace Games_Launcher.Core
{
	public class GameController
	{
		private readonly GameModel _game;

		public GameController(GameModel game) => _game = game;

		public void StartGame() => StartGameInternal(_game);
		public static void StartGame(GameModel game) => StartGameInternal(game);

		private static void StartGameInternal(GameModel game)
		{
			try
			{
				if (game.IsRunning)
					return;

				Process.Start(new ProcessStartInfo
				{
					FileName = game.Path,
					Arguments = game.Parameters,
					UseShellExecute = false,
					WorkingDirectory = Path.GetDirectoryName(game.Path)
				});
			} catch { throw; }
		}


		public void StopGame() => StopGameInternal(_game);
		public static void StopGame(GameModel game) => StopGameInternal(game);

		private static void StopGameInternal(GameModel game)
		{
			foreach (var process in Process.GetProcessesByName(game.ProcessName))
				process.Kill();
		}
	}
}
