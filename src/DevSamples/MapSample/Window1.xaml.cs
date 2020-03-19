using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;
using System.Windows;
using System.Windows.Data;

namespace MapSample
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();

			leftPlotter.Viewport.SetBinding(Viewport2D.VisibleProperty, new Binding
			{
				Source = rightPlotter.Viewport,
				Path = new PropertyPath("Visible"),
				Mode = BindingMode.TwoWay
			});

			leftCursor.SetBinding(CursorCoordinateGraph.PositionProperty, new Binding
			{
				Source = rightCursor,
				Path = new PropertyPath("Position"),
				Mode = BindingMode.TwoWay
			});

			leftPlotter.Viewport.Visible = new DataRect(37, 55, 1, 1).DataToViewport(leftPlotter.Viewport.Transform);
		}
	}
}
