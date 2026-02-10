using Games_Launcher.Core;
using System;
using System.ComponentModel;
using System.Windows.Media;

namespace Games_Launcher.Model
{
    public class GameViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

		public readonly GameModel _game;

		public GameViewModel(GameModel game) => _game = game;
		

		public string Name
		{
			get => _game.Name;
			set { _game.Name = value; OnPropertyChanged(nameof(Name)); }
		}

		public string Path
		{
			get => _game.Path;
			set { _game.Path = value; OnPropertyChanged(nameof(Path)); OnPropertyChanged(nameof(Icon)); }
		}

		public ImageSource Icon => GameFunctions.GetGameIcon(Path);

		public string Parameters
		{
			get => _game.Parameters;
			set { _game.Parameters = value; OnPropertyChanged(nameof(Parameters)); }
		}

		public DateTime LastPlayed
		{
			get => _game.LastPlayed;
			set { _game.LastPlayed = value; OnPropertyChanged(nameof(LastPlayed)); OnPropertyChanged(nameof(LastPlayedFormatted)); }
		}

		public TimeSpan PlayTime
		{
			get => _game.PlayTime;
			set { _game.PlayTime = value; OnPropertyChanged(nameof(PlayTime)); OnPropertyChanged(nameof(PlayTimeFormatted)); }
		}

		public string LastPlayedFormatted => GameFunctions.UltimaVezJugado(LastPlayed);

		public string PlayTimeFormatted => GameFunctions.ConvertTime(PlayTime);
	}
}
