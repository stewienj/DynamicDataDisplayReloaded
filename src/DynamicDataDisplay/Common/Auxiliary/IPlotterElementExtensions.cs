using System;

namespace DynamicDataDisplay
{
	public static class IPlotterElementExtensions
	{
		public static void RemoveFromPlotter(this IPlotterElement element, Plotter plotter)
		{
			if (element == null)
				throw new ArgumentNullException("element");

			if (plotter != null)
			{
				plotter.Children.Remove(element);
			}
		}

		public static void AddToPlotter(this IPlotterElement element, Plotter plotter)
		{
			if (element == null)
				throw new ArgumentNullException("element");
			if (plotter == null)
				throw new ArgumentNullException("plotter");


			plotter.Children.Add(element);
		}
	}
}
