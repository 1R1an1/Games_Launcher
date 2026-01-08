using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Games_Launcher.Infraestructure
{
    public class SingleInstanceManager
    {
		private readonly Mutex _mutex;
		private readonly EventWaitHandle _event;
		private readonly string _eventName;

		public bool IsPrimaryInstance { get; }

		public SingleInstanceManager(string mutexName, string eventName, out bool i)
		{
			_eventName = eventName;

			bool createdNew;
			_mutex = new Mutex(true, mutexName, out createdNew);
			IsPrimaryInstance = createdNew;

			if (createdNew)
			{
				_event = new EventWaitHandle(false, EventResetMode.AutoReset, eventName);
			}

			if (!IsPrimaryInstance)
			{
				NotifyExisting();
				Application.Current.Shutdown();
				i = true;
				return;
			}

			i = false;
		}

		public void Listen(Action onSignal)
		{
			if (!IsPrimaryInstance) return;

			Task.Run(() =>
			{
				while (true)
				{
					_event.WaitOne();
					onSignal();
				}
			});
		}

		public void NotifyExisting()
		{
			EventWaitHandle.OpenExisting(_eventName).Set();
		}
	}
}
