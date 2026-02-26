using Games_Launcher.Core;
using System.Windows.Media;

namespace Games_Launcher.Model
{
    public class GameViewModel : ObservableObject
	{
		public GameModel _game { get; }

		public GameViewModel(GameModel game)
		{
			_game = game;

			_game.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == nameof(GameModel.Path))
					OnPropertyChanged(nameof(Icon));

				if (e.PropertyName == nameof(GameModel.LastPlayed))
					OnPropertyChanged(nameof(LastPlayedFormatted));

				if (e.PropertyName == nameof(GameModel.PlayTime))
					OnPropertyChanged(nameof(PlayTimeFormatted));
			};
		}

		public ImageSource Icon => GameFunctions.GetGameIcon(_game.Path);
		public string LastPlayedFormatted => GameFunctions.UltimaVezJugado(_game.LastPlayed);
		public string PlayTimeFormatted => GameFunctions.ConvertTime(_game.PlayTime);
	}
}
