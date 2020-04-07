using System;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicDataDisplay.Common.Auxiliary
{
	public class ThrottledAction
	{
		private Action _action;
		private TimeSpan _timeBetweenInvokations;
		private Task _actionTask = Task.CompletedTask;
		private int _actionsQueued = 0;

		public ThrottledAction(Action action, TimeSpan timeBetweenInvokations)
		{
			_action = action;
			_timeBetweenInvokations = timeBetweenInvokations;
		}

		public ThrottledAction(TimeSpan timeBetweenInvokations) : this(() => { }, timeBetweenInvokations)
		{

		}

		public void Join()
		{
			_actionTask.Wait();
		}
		/// <summary>
		/// Invokes the action on another thread at the appropriate time in the future
		/// </summary>
		public void InvokeAction()
		{
			InvokeAction(_action);
		}

		/// <summary>
		/// Invokes the action on another thread at the appropriate time in the future
		/// </summary>
		public void InvokeAction(Action action)
		{
			_action = action;
			if (_actionsQueued < 1)
			{
				Interlocked.Increment(ref _actionsQueued);
				_actionTask = _actionTask.ContinueWith((t) =>
				{
					Interlocked.Decrement(ref _actionsQueued);
					_action();
					using (var manualResetEvent = new ManualResetEvent(false))
					{
						manualResetEvent.WaitOne(_timeBetweenInvokations);
					}
				});
			}
		}

		/// <summary>
		/// Invokes the action in the current Synchronization context at the appropriate time in the future
		/// </summary>
		public void InvokeActionSync()
		{
			InvokeActionSync(_action);
		}

		/// <summary>
		/// Invokes the action in the current Synchronization context at the appropriate time in the future
		/// </summary>
		public void InvokeActionSync(Action action)
		{
			if (_actionsQueued < 1)
			{
				Interlocked.Increment(ref _actionsQueued);
				var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
				_actionTask = _actionTask.ContinueWith((t) =>
				{
					Interlocked.Decrement(ref _actionsQueued);
					Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.DenyChildAttach, taskScheduler).Wait();
					using (var manualResetEvent = new ManualResetEvent(false))
					{
						manualResetEvent.WaitOne(_timeBetweenInvokations);
					}
				});
			}
		}
	}
}