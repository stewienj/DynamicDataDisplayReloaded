using DynamicDataDisplay;
using DynamicDataDisplay.Charts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace DynamicDataDisplay.RadioBand
{
	/// <summary>
	/// Interaction logic for SpectrumBandVerticalLineRenderer.xaml
	/// </summary>
	public partial class SpectrumBandVerticalLineRenderer : CanvasGraph
	{
		private bool _lineReassignmentRequired = false;

		public SpectrumBandVerticalLineRenderer()
		{
			SnapsToDevicePixels = true;
			InitializeComponent();
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

				/*
				if (exportMode)
				{
					var text = new FormattedText(line.Text, System.Globalization.CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Verdana"), 12, Brushes.Black);

					dc.DrawText(text, leftPoint + 0.5 * (rightPoint - leftPoint) - new Vector(0.5 * text.Width, 0.5 * text.Height));
				}
				*/
			}
		}

        private void ReassignLines()
        {
			var sourceLines = ItemsSource.OfType<object>().ToList();
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
			if (s is SpectrumBandVerticalLineRenderer control)
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
			DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(SpectrumBandVerticalLineRenderer), new PropertyMetadata(null, UpdateAndSubscribe));

	}
}
