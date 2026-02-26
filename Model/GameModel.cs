using Newtonsoft.Json;
using System;

namespace Games_Launcher.Model
{
	public class GameModel : ObservableObject
	{
		private string _name;
		public string Name
		{
			get => _name;
			set => SetProperty(ref _name, value);
		}

		private string _processName;
		public string ProcessName
		{
			get => _processName;
			set => SetProperty(ref _processName, value);
		}

		private string _parameters;
		public string Parameters
		{
			get => _parameters;
			set => SetProperty(ref _parameters, value);
		}

		private string _path;
		public string Path
		{
			get => _path;
			set => SetProperty(ref _path, value);
		}

		private DateTime _lastPlayed;
		public DateTime LastPlayed
		{
			get => _lastPlayed;
			set => SetProperty(ref _lastPlayed, value);
		}

		private TimeSpan _playTime;
		public TimeSpan PlayTime
		{
			get => _playTime;
			set => SetProperty(ref _playTime, value);
		}

		private bool _isRunning;

		[JsonIgnore]
		public bool IsRunning { get => _isRunning; set => SetProperty(ref _isRunning, value); }
	}
}
