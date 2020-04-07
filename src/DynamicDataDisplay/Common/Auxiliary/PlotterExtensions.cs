namespace DynamicDataDisplay.Common.Auxiliary
{
	public static class PlotterExtensions
	{
		public static void AddChild(this Plotter plotter, IPlotterElement child)
		{
			plotter.Children.Add(child);
		}
	}
}
