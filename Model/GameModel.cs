using Newtonsoft.Json;
using System;

namespace Games_Launcher.Model
{
	public class GameModel : ObservableObject
	{
		public string Name;
		public string ProcessName;
		public string Parameters;
		public string Path;
		public DateTime LastPlayed;
		public TimeSpan PlayTime;

		private bool _isRunning;

		[JsonIgnore]
		public bool IsRunning { get => _isRunning; set => SetProperty(ref _isRunning, value); }
	}
}
