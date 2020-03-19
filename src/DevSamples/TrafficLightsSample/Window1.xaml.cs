using Microsoft.Research.DynamicDataDisplay.ViewportRestrictions;
using System.Windows;

namespace TrafficLightsSample
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();

			plotter.Viewport.Restrictions.Add(new PhysicalProportionsRestriction());
		}
	}
}
