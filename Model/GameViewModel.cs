using Games_Launcher.Core;
using System;
using System.Windows.Media;

namespace Games_Launcher.Model
{
    public class GameViewModel : ObservableObject
	{

		public readonly GameModel _game;

		public GameViewModel(GameModel game) => _game = game;
		

		public string Name
		{
			get => _game.Name;
			set => SetProperty(ref _game.Name, value);
		}

		public string Path
		{
			get => _game.Path;
			set { SetProperty(ref _game.Path, value); OnPropertyChanged(nameof(Icon)); }
		}

		public ImageSource Icon => GameFunctions.GetGameIcon(Path);

		public string Parameters
		{
			get => _game.Parameters;
			set => SetProperty(ref _game.Parameters, value);
		}

		public DateTime LastPlayed
		{
			get => _game.LastPlayed;
			set { SetProperty(ref _game.LastPlayed, value); OnPropertyChanged(nameof(LastPlayedFormatted)); }
		}

		public TimeSpan PlayTime
		{
			get => _game.PlayTime;
			set { SetProperty(ref _game.PlayTime, value); OnPropertyChanged(nameof(PlayTimeFormatted)); }
		}

		public string LastPlayedFormatted => GameFunctions.UltimaVezJugado(LastPlayed);

		public string PlayTimeFormatted => GameFunctions.ConvertTime(PlayTime);
	}
}
