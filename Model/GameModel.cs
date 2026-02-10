using Games_Launcher.Core;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Windows.Media;

namespace Games_Launcher.Model
{
    public class GameModel : INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

		private string _name;
		public string Name
		{
			get => _name;
			set { _name = value; OnPropertyChanged(nameof(Name)); }
		}

		private string _path;
		public string Path
		{
			get => _path;
			set { _path = value; OnPropertyChanged(nameof(Path)); OnPropertyChanged(nameof(Icon)); }
		}

		[JsonIgnore]
		public ImageSource Icon => GameFunctions.GetGameIcon(Path);

		private string _parameters;
		public string Parameters
		{
			get => _parameters;
			set { _parameters = value; OnPropertyChanged(nameof(Parameters)); }
		}

		private DateTime _lastPlayed;
		public DateTime LastPlayed
		{
			get => _lastPlayed;
			set { _lastPlayed = value; OnPropertyChanged(nameof(LastPlayed)); OnPropertyChanged(nameof(PlayTimeFormatted)); }
		}

		[JsonIgnore]
		public string PlayTimeFormatted => GameFunctions.ConvertTime(PlayTime);

		private TimeSpan _playTime;
		public TimeSpan PlayTime
		{
			get => _playTime;
			set { _playTime = value; OnPropertyChanged(nameof(PlayTime)); OnPropertyChanged(nameof(LastPlayedFormatted)); }
		}

		[JsonIgnore]
		public string LastPlayedFormatted => GameFunctions.UltimaVezJugado(LastPlayed);

		public string ProcessName { get; set; }
	}
}
