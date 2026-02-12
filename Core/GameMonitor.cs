using Games_Launcher.Model;
using Games_Launcher.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Games_Launcher.Core
{
    public static class GameMonitor
    {
        private static bool _isRunning = false;
        private static readonly object _lock = new object();
        private static List<GameModel> runingGames = new List<GameModel>();
        private static ItemsControl ItemsHost => App.window.CDU_Window.GamesItemsControl;

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

                            GameView view = App.Current.Dispatcher.Invoke(() => GameFunctions.GetGameViewFromItem(ItemsHost, game));
                            if (view == null) continue;

                            view.Dispatcher.Invoke(() =>
                            {
                                view.BTNJugar.Margin = new Thickness(22.3, 0, 11.2, 0);
                                view.BTNJugar.IsEnabled = true;
                                view.BTNJugar.Content = "DETENER";
                                view.BTNJugar.Foreground = Brushes.White;
                                view.BTNJugar.Tag = view.FindResource("DownloadColorNormal");
                                view.BTNJugar.BorderBrush = (Brush)view.FindResource("DownloadColorMouseOver");
                                view.LBLLastOppend.Content = GameFunctions.UltimaVezJugado(view.thisGame.LastPlayed);
                                App.UpdateNIcons();
                            });
                        }
                        else if (!currentlyRunning && game.IsRunning)
                        {
                            runingGames.Remove(game);
                            game.IsRunning = false;
                            TimeSpan duration = DateTime.Now - game.LastPlayed;
                            game.PlayTime += duration;

                            GameView view = App.Current.Dispatcher.Invoke(() => GameFunctions.GetGameViewFromItem(ItemsHost, game));
                            if (view == null) continue;

                            view.Dispatcher.Invoke(() =>
                            {
                                view.BTNJugar.Margin = new Thickness(31.65, 0, 20.55, 0);
                                view.BTNJugar.IsEnabled = true;
                                view.BTNJugar.Content = "JUGAR";
                                view.BTNJugar.Foreground = Brushes.White;
                                view.BTNJugar.Tag = view.FindResource("JugarColorNormal");
                                view.BTNJugar.BorderBrush = (Brush)view.FindResource("JugarColorMouseOver");
                                view.LBLTimeOppend.Content = GameFunctions.ConvertTime(view.thisGame.PlayTime);
                            });
                        }
                    }


                    await Task.Delay(3000);
                }
            });

            App.window.CloseEvent += CloseEvent;
        }

        private static void CloseEvent()
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
