using Games_Launcher.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Games_Launcher.Core
{
    public static class GameMonitor
    {
		private static bool _isRunning = false;
        private static readonly object _lock = new object();
        private static List<GameModel> runingGames = new List<GameModel>();

        public static void StartLoop()
        {
            _ = Task.Run(async () =>
            {
                _isRunning = true;

                while (_isRunning)
                {
                    GameModel[] currentGames;

                    // Evitamos modificar la lista mientras se recorre
                    lock (_lock)
                    {
                        currentGames = GamesInfo.Games.ToArray();
                    }

                    foreach (var game in currentGames)
                    {
                        var processes = Process.GetProcessesByName(game.ProcessName);
                        bool currentlyRunning = processes.Length > 0;

                        if (currentlyRunning && !game.IsRunning)
                        {
                            runingGames.Add(game);
                            game.IsRunning = true;
                            game.LastPlayed = DateTime.Now;
                        }
                        else if (!currentlyRunning && game.IsRunning)
                        {
                            runingGames.Remove(game);
                            game.IsRunning = false;
                            game.PlayTime += DateTime.Now - game.LastPlayed;
                        }
                    }
                    await Task.Delay(3000);
                }
            });
        }

        public static void StopLoop()
        {
            if (runingGames.Count > 0)
            {
                _isRunning = false;
                lock (_lock)
                {
                    foreach (var game in runingGames)
                    {
                        game.PlayTime += (DateTime.Now - game.LastPlayed);
                        game.IsRunning = false;
                    }
                    runingGames.Clear();
                }
            }
        }
    }
}
