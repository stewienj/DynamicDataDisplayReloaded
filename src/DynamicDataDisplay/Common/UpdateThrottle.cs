using System;
using System.Timers;
using System.Windows.Threading;

namespace Microsoft.Research.DynamicDataDisplay.Common
{
	public class UpdateThrottle
	{
		private Timer _updateTimer;
		private Dispatcher _dispatcher;
		private Action _action;
		public UpdateThrottle(int rateMilliseconds)
		{
			_updateTimer = new Timer(rateMilliseconds);
			_updateTimer.Elapsed += _updateTimer_Elapsed;
			_updateTimer.AutoReset = false;
		}

		public void Throttle(Dispatcher dispatcher, Action action)
		{
			_dispatcher = dispatcher;
			_dispatcher.ShutdownStarted += (s, e) =>
			{
				_dispatcher = null;
				_updateTimer.Enabled = false;

			};
			_action = action;
			_updateTimer.Enabled = true;
		}

		private void _updateTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			var localAction = _action;
			if (localAction != null)
			{
				try
				{
					_dispatcher?.Invoke(localAction);
				}
				catch (Exception)
				{

				}
			}
		}
	}
}
