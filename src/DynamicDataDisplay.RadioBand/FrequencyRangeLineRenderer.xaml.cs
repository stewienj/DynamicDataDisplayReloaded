using DynamicDataDisplay;
using DynamicDataDisplay.Charts;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace DynamicDataDisplay.RadioBand
{
	/// <summary>
	/// Interaction logic for FrequencyRangeLineRenderer.xaml
	/// </summary>
	public partial class FrequencyRangeLineRenderer : CanvasGraph
	{
		private Brush _rangeBrush;
		private Pen _rangePen;
		private Brush _rangeBrushHighlight;
		private Pen _rangePenHighlight;
		private Brush _rangeBrushSelected;
		private Pen _rangePenSelected;
		private Brush _rangeBrushHighlightSelected;
		private Pen _rangePenHighlightSelected;

		public FrequencyRangeLineRenderer()
		{
			SnapsToDevicePixels = true;
			SetBrushes(ColorStyleType.PaleGreen);
			InitializeComponent();
		}

		public void SetBrushes(ColorStyleType colorStyleType)
		{
			double penThickness = 2;
			switch (colorStyleType)
			{
				case ColorStyleType.PaleGreen:
					_rangeBrush = new SolidColorBrush(Color.FromArgb(60, 0, 50, 0));
					_rangePen = new Pen(new SolidColorBrush(Color.FromArgb(180, 0, 150, 0)), penThickness);
					_rangeBrushHighlight = new SolidColorBrush(Color.FromArgb(120, 0, 100, 0));
					_rangePenHighlight = new Pen(new SolidColorBrush(Color.FromArgb(240, 0, 255, 0)), penThickness);
					_rangeBrushSelected = new SolidColorBrush(Color.FromArgb(60, 50, 0, 50));
					_rangePenSelected = new Pen(new SolidColorBrush(Color.FromArgb(180, 150, 0, 150)), penThickness);
					_rangeBrushHighlightSelected = new SolidColorBrush(Color.FromArgb(120, 100, 0, 100));
					_rangePenHighlightSelected = new Pen(new SolidColorBrush(Color.FromArgb(240, 255, 0, 255)), penThickness);
					break;
				case ColorStyleType.Red:
					_rangeBrush = new SolidColorBrush(Color.FromArgb(60, 50, 0, 0));
					_rangePen = new Pen(new SolidColorBrush(Color.FromArgb(180, 150, 0, 0)), penThickness);
					_rangeBrushHighlight = new SolidColorBrush(Color.FromArgb(120, 100, 0, 0));
					_rangePenHighlight = new Pen(new SolidColorBrush(Color.FromArgb(240, 255, 0, 0)), penThickness);
					_rangeBrushSelected = new SolidColorBrush(Color.FromArgb(60, 0, 50, 50));
					_rangePenSelected = new Pen(new SolidColorBrush(Color.FromArgb(180, 0, 150, 150)), penThickness);
					_rangeBrushHighlightSelected = new SolidColorBrush(Color.FromArgb(120, 0, 100, 100));
					_rangePenHighlightSelected = new Pen(new SolidColorBrush(Color.FromArgb(240, 0, 255, 255)), penThickness);
					break;
				case ColorStyleType.Orange:
					_rangeBrush = new SolidColorBrush(Colors.Coral);
					_rangePen = new Pen(new SolidColorBrush(Colors.Coral), penThickness);
					_rangeBrushHighlight = new SolidColorBrush(Colors.Orange);
					_rangePenHighlight = new Pen(new SolidColorBrush(Colors.Orange), penThickness);
					_rangeBrushSelected = new SolidColorBrush(Colors.DarkOrange);
					_rangePenSelected = new Pen(new SolidColorBrush(Colors.DarkOrange), penThickness);
					_rangeBrushHighlightSelected = new SolidColorBrush(Colors.Orange);
					_rangePenHighlightSelected = new Pen(new SolidColorBrush(Colors.Orange), penThickness);
					break;
				case ColorStyleType.Brown:
					_rangeBrush = new SolidColorBrush(Colors.DarkGoldenrod);
					_rangePen = new Pen(new SolidColorBrush(Colors.DarkGoldenrod), penThickness);
					_rangeBrushHighlight = new SolidColorBrush(Colors.BurlyWood);
					_rangePenHighlight = new Pen(new SolidColorBrush(Colors.BurlyWood), penThickness);
					_rangeBrushSelected = new SolidColorBrush(Colors.Brown);
					_rangePenSelected = new Pen(new SolidColorBrush(Colors.Brown), penThickness);
					_rangeBrushHighlightSelected = new SolidColorBrush(Colors.BurlyWood);
					_rangePenHighlightSelected = new Pen(new SolidColorBrush(Colors.BurlyWood), penThickness);
					break;
				default:
					break;
			}
		}

		public void ClearLines() => Children.Clear();

		public void AddLine(RadioBandLine radioBandLine) => Children.Add(radioBandLine);

		public void SelectAllAt(double coord)
		{
			foreach (var line in GetLines())
			{
				line.IsSelected = line.Start <= coord && line.End >= coord;
			}
		}

		public void SelectNearest(Point coord)
		{
			// convert everything to screen coordinates to get the visually closest line
			var cursor = coord.DataToScreen(Plotter2D.Transform);
			RadioBandLine closest = null;
			double closestDistance = double.MaxValue;
			foreach (var line in GetLines())
			{
				Point leftPoint = new Point(line.Start, line.GroupAxisCoord).DataToScreen(Plotter2D.Transform);
				Point rightPoint = new Point(line.End, line.GroupAxisCoord).DataToScreen(Plotter2D.Transform);
				double distance = MathHelper.DistancePointToSegment(cursor, (leftPoint, rightPoint)).distance;
				if (distance < closestDistance)
				{
					closestDistance = distance;
					closest = line;
				}
				line.IsSelected = false;
			}
			closest.IsSelected = true;
		}

		protected override Size MeasureOverride(Size constraint)
		{
			this.InvalidateVisual();
			return base.MeasureOverride(constraint);
		}

		protected override void OnViewportPropertyChanged(DynamicDataDisplay.ExtendedPropertyChangedEventArgs e)
		{
			InvalidateVisual();
		}

		protected override void OnRender(DrawingContext dc)
		{
			if (Plotter2D == null) return;

			// Viewport is the range of the data normalized to 1x1 square coordinates
			var corner1 = new Point(0, 0).ViewportToData(Plotter2D.Transform);
			var corner2 = new Point(1, 1).ViewportToData(Plotter2D.Transform);

			var bounds = new DataRect(corner1, corner2);

			Viewport2D.SetContentBounds(this, bounds);
			ViewportPanel.SetViewportBounds(this, bounds);

			bool exportMode = false;
			if (Plotter2D is ChartPlotter p)
			{
				exportMode = p.ExportMode;
			}

			foreach (var line in GetLines())
			{
				Point leftPoint = new Point(line.Start, line.GroupAxisCoord).DataToScreen(Plotter2D.Transform);
				Point rightPoint = new Point(line.End, line.GroupAxisCoord).DataToScreen(Plotter2D.Transform);
				if (leftPoint.X > rightPoint.X)
				{
					var temp = leftPoint;
					leftPoint = rightPoint;
					rightPoint = temp;
				}

				double minThickness = line.IsSelected ? 10.0 : 5.0;
				var leftRightPadding = Math.Max(minThickness * 0.5 - (rightPoint.X - leftPoint.X) * minThickness, exportMode ? 10 : 0);
				leftPoint = new Point(leftPoint.X - leftRightPadding, Math.Round(leftPoint.Y));
				rightPoint = new Point(rightPoint.X + leftRightPadding, Math.Round(rightPoint.Y));

				dc.DrawLine(line.IsSelected ? _rangePenSelected : _rangePen, leftPoint, rightPoint);

				if (exportMode)
				{
					var text = new FormattedText(line.Text, System.Globalization.CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Verdana"), 12, Brushes.Black);

					dc.DrawText(text, leftPoint + 0.5 * (rightPoint - leftPoint) - new Vector(0.5 * text.Width, 0.5 * text.Height));
				}
			}
		}

		private IEnumerable<RadioBandLine> GetLines() => Children.OfType<RadioBandLine>();


		/// <summary>
		/// This covers all Dependency Property changes. Just force an update.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="e"></param>
		private static void UpdateAndSubscribe(DependencyObject s, DependencyPropertyChangedEventArgs e)
		{
			if (s is FrequencyRangeLineRenderer control)
			{
				control.InvalidateVisual();

				if (e.OldValue is INotifyCollectionChanged oldCollection)
				{
					oldCollection.CollectionChanged -= control.Source_CollectionChanged;
				}
				if (e.NewValue is INotifyCollectionChanged newCollection)
				{
					newCollection.CollectionChanged += control.Source_CollectionChanged;
				}
			}
		}

		private void Source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.DataBind, (Action)(() => InvalidateVisual()));
		}

	}
}
