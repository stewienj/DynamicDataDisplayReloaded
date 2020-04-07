using DynamicDataDisplay.Charts.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace DynamicDataDisplay.Charts
{
	/// <summary>
	/// This class allows the binding of a INotifyCollectionChanged/IEnumerable object that holds multiple line sources.
	/// Binding of the line sources to LineGraph objects is done through a Style in the resource of this host.
	/// </summary>
	public class MultipleLineSourceHost : CanvasGraph
	{
		private List<LineGraph> _lineGraphs = new List<LineGraph>();

		private bool _previousAutoAssignColors = false;
		// Keep a backup copy of the value so we can read it on a non Dispatcher thread
		private bool _autoAssignColors = false;

		private Color[] GetAsignedColors(int numberOfColors)
		{
			return Enumerable.Range(0, numberOfColors)
			  .Select(i => i * 360 / (double)numberOfColors)
			  .Select(h => ColorFromHSV(h, 1, 1))
			  .ToArray();
		}

		private void RebuildLines(IEnumerable<object> source)
		{
			// Make a copy of the lines list in case it changes under us
			var lines = (source ?? Enumerable.Empty<object>()).ToList();

			ManualResetEvent manualResetEvent = new ManualResetEvent(false);
			// Have to do this in a dispatcher
			Dispatcher.InvokeAsync(() =>
			{
				// Make the number of lines we are tracking equal to the number of data sources we have
				while (_lineGraphs.Count < lines.Count)
				{
					LineGraph lineGraph = new LineGraph();
					lineGraph.Filters.Add(new FrequencyFilter());
					_lineGraphs.Add(lineGraph);
					Plotter2D.Children.Add(lineGraph);
				}
				while (_lineGraphs.Count > lines.Count)
				{
					int lastIndex = _lineGraphs.Count - 1;
					Plotter2D.Children.Remove(_lineGraphs[lastIndex]);
					_lineGraphs.RemoveAt(lastIndex);
				}
				double multiplier = 360.0 / lines.Count;
				for (int i = 0; i < lines.Count; ++i)
				{
					if (!_autoAssignColors && _previousAutoAssignColors)
					{
						// Make sure data context is changed so Styles can be applied if not auto assigning colors
						// but they were auto assigned on the last pass
						_lineGraphs[i].DataContext = null;
					}

					_lineGraphs[i].DataContext = lines[i];

					if (_autoAssignColors)
					{
						_lineGraphs[i].Stroke = new SolidColorBrush(ColorFromHSV(i * multiplier, 1, 1));
						_lineGraphs[i].LinePen = new Pen(_lineGraphs[i].Stroke, 1.0);
					}
				}
				manualResetEvent.Set();
			}, DispatcherPriority.Normal);

			manualResetEvent.WaitOne(TimeSpan.FromSeconds(1));
			_previousAutoAssignColors = _autoAssignColors;
		}

		public static Color ColorFromHSV(double hue, double saturation, double value)
		{
			int hi = System.Convert.ToInt32(Math.Floor(hue / 60)) % 6;
			double f = hue / 60 - Math.Floor(hue / 60);

			value = value * 255;
			byte v = (byte)Convert.ToInt32(value);
			byte p = (byte)Convert.ToInt32(value * (1 - saturation));
			byte q = (byte)Convert.ToInt32(value * (1 - f * saturation));
			byte t = (byte)Convert.ToInt32(value * (1 - (1 - f) * saturation));

			if (hi == 0)
				return Color.FromArgb(255, v, t, p);
			else if (hi == 1)
				return Color.FromArgb(255, q, v, p);
			else if (hi == 2)
				return Color.FromArgb(255, p, v, t);
			else if (hi == 3)
				return Color.FromArgb(255, p, q, v);
			else if (hi == 4)
				return Color.FromArgb(255, t, p, v);
			else
				return Color.FromArgb(255, v, p, q);
		}

		public bool AutoAsignColors
		{
			get { return (bool)GetValue(AutoAsignColorsProperty); }
			set { SetValue(AutoAsignColorsProperty, value); }
		}

		// Using a DependencyProperty as the backing store for AutoAsignColors.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty AutoAsignColorsProperty =
			DependencyProperty.Register("AutoAsignColors", typeof(bool), typeof(MultipleLineSourceHost), new PropertyMetadata(false, (d, e) =>
			 {
				 if (d is MultipleLineSourceHost control)
				 {
					 // When set to true auto asign the colors
					 if (e.NewValue is bool autoAssignColors && autoAssignColors == true)
					 {
						 control._autoAssignColors = autoAssignColors;
						 control.RebuildLines(control.MultipleLineSource?.Cast<object>());
					 }
				 }
			 }));

		public IEnumerable MultipleLineSource
		{
			get { return (IEnumerable)GetValue(MultipleLineSourceProperty); }
			set { SetValue(MultipleLineSourceProperty, value); }
		}

		// Using a DependencyProperty as the backing store for MultipleLineSource.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MultipleLineSourceProperty =
		  DependencyProperty.Register("MultipleLineSource", typeof(IEnumerable), typeof(MultipleLineSourceHost), new PropertyMetadata(null, (d, e) =>
		  {
			  // So need to map each source to a LineGraph object, setting the LineGraph DataContext accordingly
			  if (d is MultipleLineSourceHost control)
			  {
				  if (e.OldValue is INotifyCollectionChanged oldCollection)
				  {
					  oldCollection.CollectionChanged -= control.NewCollection_CollectionChanged;
				  }
				  if (e.NewValue is INotifyCollectionChanged newCollection)
				  {
					  newCollection.CollectionChanged += control.NewCollection_CollectionChanged;
					  control.RebuildLines(control.MultipleLineSource?.Cast<object>());
				  }
			  }
		  }));

		private void NewCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			RebuildLines(sender as IEnumerable<object>);
		}
	}
}
