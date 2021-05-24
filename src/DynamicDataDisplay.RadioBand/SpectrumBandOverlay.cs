using DynamicDataDisplay;
using DynamicDataDisplay.Charts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace DynamicDataDisplay.RadioBand
{
	/// <summary>
	/// Interaction logic for SpectrumBandOverlay.xaml
	/// </summary>
	public class SpectrumBandOverlay : CanvasGraph
	{
		private bool _lineReassignmentRequired = true;

		public SpectrumBandOverlay()
		{
			// Need to give each instance it's own collection of gradient stops
			GradientStops = new List<GradientStop>();
			Background = new SolidColorBrush(Colors.Transparent);
			IsHitTestVisible = false;
			Cursor = Cursors.None;
			SnapsToDevicePixels = true;
		}

		public void SelectAllAt(double coord)
		{
			foreach (var line in GetLines())
			{
				line.IsSelected = (line.Frequency-line.Bandwidth*0.5) < coord && coord < (line.Frequency+line.Bandwidth*0.5);
			}
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

			if (_lineReassignmentRequired)
            {
				ReassignLines();
				_lineReassignmentRequired = false;
			}

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

			// Performance idea. This currently renders a whole lot of rectangles using a linear gradient brush that is
			// calculated for each rectangle. Could just calculate 1 gradient brush that is used for 1 rectangle, where the
			// gradient stops are precalculated alpha blended stops at the boundaries of the rectangles, and at the stops
			// for each rectangle.

			// Viewport is the range of the data normalized to 1x1 square coordinates
			var dataMinY = new Point(0.5, 0).ViewportToData(Plotter2D.Transform).Y;
			var dataMaxY = new Point(0.5, 1).ViewportToData(Plotter2D.Transform).Y;

			var transform = Plotter2D.Transform;
			var viewPortRect = transform.ViewportRect;

			foreach (var line in GetLines())
			{
				var spectrumDataRect = new DataRect
				(
					new Point(line.Frequency - line.Bandwidth * 0.5, dataMinY),
					new Point(line.Frequency + line.Bandwidth * 0.5, dataMaxY)
				);

				var spectrumViewRect = spectrumDataRect.DataToScreen(transform);
				var gradientStops = GetGradientStopsForLine(line);

				dc.DrawRectangle(new LinearGradientBrush(gradientStops, 0.0), null, spectrumViewRect);
			}
		}

		/// <summary>
		/// Because the frequency scale isn't linear, we have to adjust the gradient stops to compensate
		/// </summary>
		/// <param name="line"></param>
		/// <returns></returns>
        private GradientStopCollection GetGradientStopsForLine(SpectrumBandLine line)
        {
			// Gradient Stop have to go from 0 to 1, but adjusted for the x scale
			var minStop = GradientStops.Min(gs => gs.Offset);
			var maxStop = GradientStops.Max(gs => gs.Offset);

			if (maxStop > minStop)
			{
				var stopWidth = maxStop - minStop;

				var transform = Plotter2D.Transform;
				var multipler = line.Bandwidth / stopWidth;
				var bandStart = line.Frequency - line.Bandwidth * 0.5;

				double GetNewOffset(double oldOffset)
				{
					var dataStop = (oldOffset - minStop) * multipler + bandStart;
					return new Point(dataStop, 0).DataToViewport(transform).X;
				}

				// Gets the offsets in viewport data
				var offsetData = GradientStops.Select(gs => GetNewOffset(gs.Offset)).ToList();

				// now normalize to 0 to 1
				var minOffsetData = offsetData.Min();
				var maxOffsetData = offsetData.Max();
				if (maxOffsetData>minOffsetData)
                {
					var offsetDataWidth = maxOffsetData - minOffsetData;
					var offsetDataNormalized = offsetData.Select(od => (od - minOffsetData) / offsetDataWidth);
					var gradientStopsForLine = GradientStops.Zip(offsetDataNormalized, (a, b) => new GradientStop(a.Color, b));
					return new GradientStopCollection(gradientStopsForLine);
				}

            }

			return new GradientStopCollection(GradientStops.Select(gs => new GradientStop(gs.Color, 0.5)));
		}

		private void ReassignLines()
        {
			var sourceLines = ItemsSource?.OfType<object>().ToList();
			if (sourceLines==null)
            {
				return;
            }
			var chartLines = GetLines().ToList();

			// Remove excess chart lines
			
			while(chartLines.Count > sourceLines.Count)
            {
				int removeIndex = chartLines.Count - 1;
				var toRemove = chartLines[removeIndex];
				chartLines.RemoveAt(removeIndex);
				Children.Remove(toRemove);
            }

			// Assign data context to the remaining chart lines
			
			for(int i=0; i< chartLines.Count; ++i)
            {
				chartLines[i].DataContext = sourceLines[i];
            }

			// Add in new chart lines if required
			while(chartLines.Count < sourceLines.Count)
            {
				var newLine = new SpectrumBandLine();
				newLine.DataContext = sourceLines[chartLines.Count];
				chartLines.Add(newLine);
				Children.Add(newLine);
            }
        }

        private IEnumerable<SpectrumBandLine> GetLines() => Children.OfType<SpectrumBandLine>();

		/// <summary>
		/// This covers all Dependency Property changes. Just force an update.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="e"></param>
		private static void UpdateAndSubscribe(DependencyObject s, DependencyPropertyChangedEventArgs e)
		{
			if (s is SpectrumBandOverlay control)
			{
				control._lineReassignmentRequired = true;
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
			Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.DataBind, (Action)(() => {
				_lineReassignmentRequired = true;
				InvalidateVisual();
			}));
		}

		public IEnumerable ItemsSource
		{
			get => (IEnumerable)GetValue(ItemsSourceProperty);
			set => SetValue(ItemsSourceProperty, value);
		}

		// Using a DependencyProperty as the backing store for ExternalSelectedItemsSource.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(SpectrumBandOverlay), new PropertyMetadata(null, UpdateAndSubscribe));

        public List<GradientStop> GradientStops
		{
            get { return (List<GradientStop>)GetValue(GradientStopsProperty); }
            set { SetValue(GradientStopsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GradientStops.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GradientStopsProperty =
            DependencyProperty.Register("GradientStops", typeof(List<GradientStop>), typeof(SpectrumBandOverlay), new PropertyMetadata(null));

    }
}
