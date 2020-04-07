using System.Windows;
using System.Windows.Controls;

namespace DynamicDataDisplay
{
	/// <summary>
	/// Represents a text in ChartPlotter's footer.
	/// </summary>
	public class Footer : ContentControl, IPlotterElement
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Footer"/> class.
		/// </summary>
		public Footer()
		{
			HorizontalAlignment = HorizontalAlignment.Center;
			Margin = new Thickness(0, 0, 0, 3);
		}

		void IPlotterElement.OnPlotterAttached(Plotter plotter)
		{
			this.plotter = plotter;
			plotter.FooterPanel.Children.Add(this);
		}

		void IPlotterElement.OnPlotterDetaching(Plotter plotter)
		{
			plotter.FooterPanel.Children.Remove(this);
			this.plotter = null;
		}

		private Plotter plotter;
		Plotter IPlotterElement.Plotter
		{
			get { return plotter; ; }
		}
	}
}
