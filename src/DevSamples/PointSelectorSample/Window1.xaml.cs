using DynamicDataDisplay;
using System.Windows;

namespace PointSelectorSample
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
		}

		private void ClearPointsBtn_Click(object sender, RoutedEventArgs e)
		{
			selector.Points.Clear();
		}
	}

	public class NegativeYDataTransform : DataTransform
	{
		public override Point DataToViewport(Point pt)
		{
			return new Point(pt.X, -pt.Y);
		}

		public override Point ViewportToData(Point pt)
		{
			return new Point(pt.X, -pt.Y);
		}
	}

}
