﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DynamicDataDisplay.Common.Auxiliary
{
	public static class TaskExtensions
	{
		/// <summary>
		/// Logs exceptions that occur during task execution.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <returns></returns>
		public static Task WithExceptionLogging(this Task task)
		{
			return task.ContinueWith(t =>
			{
				Exception exception = t.Exception;
				if (exception != null)
				{
					if (exception.InnerException != null)
						exception = exception.InnerException;

					Debug.WriteLine("Failure in async task: " + exception.Message);
				}
			}, TaskContinuationOptions.OnlyOnFaulted);
		}

		/// <summary>
		/// Rethrows exceptions thrown during task execution in thespecified dispatcher thread.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <param name="dispatcher">The dispatcher.</param>
		/// <returns></returns>
		public static Task WithExceptionThrowingInDispatcher(this Task task, Dispatcher dispatcher)
		{
			return task.ContinueWith(t =>
			{
				dispatcher.BeginInvoke((Action)(() =>
				{
					throw t.Exception;
				}), DispatcherPriority.Send);
			}, TaskContinuationOptions.OnlyOnFaulted);
		}
	}
}
