using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Research.DynamicDataDisplay
{
	public class Header : ContentControl, IPlotterElement
	{
		public Header()
		{
			FontSize = 18;
			HorizontalAlignment = HorizontalAlignment.Center;
			Margin = new Thickness(0, 0, 0, 3);
		}

		private Plotter plotter;
		public Plotter Plotter
		{
			get { return plotter; }
		}

		public void OnPlotterAttached(Plotter plotter)
		{
			this.plotter = plotter;
			plotter.HeaderPanel.Children.Add(this);
		}

		public void OnPlotterDetaching(Plotter plotter)
		{
			this.plotter = null;
			plotter.HeaderPanel.Children.Remove(this);
		}
	}
}