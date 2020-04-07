using DynamicDataDisplay.DataSources;
using DynamicDataDisplay.PointMarkers;
using DynamicDataDisplay.ViewportRestrictions;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DynamicDataDisplay.Demo
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			Loaded += new RoutedEventHandler(MainWindow_Loaded);
		}

		LineGraph line;
		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			// Prepare data in arrays
			const int N = 1000;
			double[] x = new double[N];
			double[] y = new double[N];

			for (int i = 0; i < N; i++)
			{
				x[i] = i * 0.1;
				y[i] = Math.Cos(x[i]);
			}

			// Add data sources:
			var yDataSource = new EnumerableDataSource<double>(y);
			yDataSource.SetYMapping(Y => Y);
			yDataSource.AddMapping(ShapeElementPointMarker.ToolTipTextProperty,
				Y => string.Format("Value is {0}", Y));

			var xDataSource = new EnumerableDataSource<double>(x);
			xDataSource.SetXMapping(X => X);


			CompositeDataSource compositeDataSource = new CompositeDataSource(xDataSource, yDataSource);

			Matrix m = Matrix.Identity;
			m.RotateAt(45, 10, 10);
			line = new LineGraph
			{
				Stroke = Brushes.Green,
				StrokeThickness = 2,
				DataTransform =
				//new MatrixDataTransform(m) 
				new RotateDataTransform(45.0.DegreesToRadians())
			};
			line.DataSource = compositeDataSource;
			line.AddToPlotter(plotter);

			plotter.Viewport.Restrictions.Add(new PhysicalProportionsRestriction { ProportionRatio = 1 });


			// adding graph to plotter
			plotter.AddLineGraph(compositeDataSource,
				new Pen(Brushes.Goldenrod, 3),
				new SampleMarker(),
				new PenDescription("Cosine"));

			//plotter.Viewport.FitToViewRestrictions.Add(new FollowDataWidthRestriction { Width = 1 });

			plotter.PreviewKeyDown += plotter_KeyDown;
		}

		void plotter_KeyDown(object sender, KeyEventArgs e)
		{
			if ((e.Key == Key.OemPlus || e.Key == Key.OemMinus) &&
				(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
			{
				double sign = e.Key == Key.OemPlus ? +1 : -1;
				RotateDataTransform transform = (RotateDataTransform)line.DataTransform;
				line.DataTransform = new RotateDataTransform(transform.Angle + sign * 0.01);

				e.Handled = true;
			}
		}
	}

	public class SampleMarker : ShapeElementPointMarker
	{
		public override UIElement CreateMarker()
		{
			Canvas result = new Canvas()
			{
				Width = 10,
				Height = Size
			};
			SetMarkerProperties(result);
			return result;
		}

		public override void SetMarkerProperties(UIElement marker)
		{
			Canvas result = (Canvas)marker;
			result.Width = Size;
			result.Height = Size;
			result.Background = Brush;
			if (ToolTipText != string.Empty)
			{
				ToolTip tt = new ToolTip();
				tt.Content = ToolTipText;
				result.ToolTip = tt;
			}
		}

		public override void SetPosition(UIElement marker, Point screenPoint)
		{
			Canvas.SetLeft(marker, screenPoint.X - Size / 2);
			Canvas.SetTop(marker, screenPoint.Y - Size / 2);
		}
	}

}
