using DynamicDataDisplay.Common;
using System;
using System.Linq;
using System.Windows.Threading;

namespace DynamicDataDisplay
{
	public static class PlotterChildrenCollectionExtensions
	{
		public static void RemoveAll<T>(this PlotterChildrenCollection children)
		{
			var childrenToDelete = children.OfType<T>().ToList();

			foreach (var child in childrenToDelete)
			{
				children.Remove(child as IPlotterElement);
			}
		}

		public static void BeginAdd(this PlotterChildrenCollection children, IPlotterElement child)
		{
			children.Plotter.Dispatcher.BeginInvoke(((Action)(() => { children.Add(child); })), DispatcherPriority.Send);
		}

		public static void BeginRemove(this PlotterChildrenCollection children, IPlotterElement child)
		{
			children.Plotter.Dispatcher.BeginInvoke(((Action)(() => { children.Remove(child); })), DispatcherPriority.Send);
		}
	}
}
