using DynamicDataDisplay.Charts;
using DynamicDataDisplay.Charts.Navigation;
using DynamicDataDisplay.Common.Auxiliary;
using DynamicDataDisplay.RadioBand.ConfigLoader;
using DynamicDataDisplay.ViewportRestrictions;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DynamicDataDisplay.RadioBand
{
    public class RadioBandChartPlotter : ChartPlotter
	{
		private Dictionary<IComparable, List<RadioBandLine>> _groupToRadioBandLines = new Dictionary<IComparable, List<RadioBandLine>>();
		private List<RadioBandLine> _ungroupedRadioBandLines = new List<RadioBandLine>();

		private RadioBandPlotConfig _radioBandPlotConfig;
		private RadioBandGroupAxis _groupAxis;
		private RadioBandFrequencyAxis _radioBandFrequencyAxis;
		private FrequencyRangeLineRenderer _lineRenderer;

		public RadioBandChartPlotter()
		{
			Children.Remove(Legend);

			SetFrequencyAxisFromConfig(RadioBandPlotConfig.SpectrumDisplayDefault);

			MainVerticalAxis.Visibility = Visibility.Collapsed;
			MainHorizontalAxis.Visibility = Visibility.Collapsed;
			AxisGrid.DrawHorizontalsShaded = true;

			_groupAxis = new RadioBandGroupAxis();
			Children.Add(_groupAxis);

			_lineRenderer = new FrequencyRangeLineRenderer();
			Children.Add(_lineRenderer);

			DefaultContextMenu.StaticMenuItems.Insert(0, new MenuItem { Header = "Select All At This Frequency", Command = SelectAllLinesAtFrequencyCommand });
			DefaultContextMenu.StaticMenuItems.Insert(1, new MenuItem { Header = "Select Nearest Line", Command = SelectNearestCommand });
		}

		public string GetGroupID(double yValue)
		{
			var groups = GetSortedGroups();
			if (yValue >= 0 && yValue < groups.Count)
			{
				return groups[(groups.Count - 1) - (int)Math.Truncate(yValue)]?.ToString();
			}
			else
			{
				return null;
			}
		}

		protected override void OnChildAdded(IPlotterElement child)
		{
			base.OnChildAdded(child);

			if (child is CursorCoordinateGraph graph)
			{
				graph.YTextMapping = (y) => "";
				graph.XTextMapping = (x) => x.ToEngineeringNotation() + "Hz";
			}
		}

		private void SetFrequencyAxisFromConfig(RadioBandPlotConfig config)
		{
			_radioBandPlotConfig = config;
			DataTransform = new RadioBandTransform(_radioBandPlotConfig);
			((HorizontalAxis)MainHorizontalAxis).TicksProvider = new RadioBandFrequencyTicksProvider(_radioBandPlotConfig); ;
			Children.RemoveAll<RadioBandFrequencyAxis>();
			_radioBandFrequencyAxis = new RadioBandFrequencyAxis(_radioBandPlotConfig);
			Children.Add(_radioBandFrequencyAxis);
		}

		private RelayCommandFactoryD3 _importChartAxisCommand = new RelayCommandFactoryD3();
		public ICommand ImportChartAxisCommand => _importChartAxisCommand.GetCommand(() =>
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.CheckFileExists = true;
			dialog.DefaultExt = ".xml";
			dialog.Filter = "XML Documents (.xml)|*.xml|All Files|*.*";
			dialog.Title = "Import Chart Axis Configuration";
			if (dialog.ShowDialog() == true)
			{
				var config = RadioBandPlotConfig.FromXmlFile(dialog.FileName);
				SetFrequencyAxisFromConfig(config);
				ResetRadioBandLines();
			}
		});

		private RelayCommandFactoryD3 _exportChartAxisCommand = new RelayCommandFactoryD3();
		public ICommand ExportChartAxisCommand => _exportChartAxisCommand.GetCommand(() =>
		{
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.DefaultExt = ".xml";
			dialog.Filter = "XML Documents (.xml)|*.xml|All Files|*.*";
			dialog.Title = "Export Chart Axis Configuration";
			if (dialog.ShowDialog() == true)
			{
				_radioBandPlotConfig.SaveToXml(dialog.FileName);
			}
		});

		private RelayCommandFactoryD3 _selectAllLinesAtFrequencyCommand = new RelayCommandFactoryD3();
		private ICommand SelectAllLinesAtFrequencyCommand => _selectAllLinesAtFrequencyCommand.GetCommand(() =>
		{
			// So what frequency are we at?
			var plotter = this;
			var mousePos = plotter.TranslatePoint(plotter.DefaultContextMenu.MousePositionOnClick, plotter.CentralGrid);
			var coord = mousePos.ScreenToData(plotter.Transform);
			_lineRenderer.SelectAllAt(coord.X);
		});

		private RelayCommandFactoryD3 _selectNearestCommand = new RelayCommandFactoryD3();
		private ICommand SelectNearestCommand => _selectNearestCommand.GetCommand(() =>
		{
			// TODO make this Async and do to nearest line calc

			// So what frequency are we at?
			var plotter = this;
			var mousePos = plotter.TranslatePoint(plotter.DefaultContextMenu.MousePositionOnClick, plotter.CentralGrid);
			var coord = mousePos.ScreenToData(plotter.Transform);
			_lineRenderer.SelectNearest(coord);
		});

		// ************************************************************************
		// Dependency Properties
		// ************************************************************************

		#region Dependency Properties

		public IEnumerable ItemsSource
		{
			get => (IEnumerable)GetValue(ItemsSourceProperty);
			set => SetValue(ItemsSourceProperty, value);
		}

		// Using a DependencyProperty as the backing store for ExternalSelectedItemsSource.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(RadioBandChartPlotter), new PropertyMetadata(null, (d, e) =>
			{
				RadioBandChartPlotter control = (RadioBandChartPlotter)d;
				control.SetItemsSource((IEnumerable)e.NewValue);
			}));

		#endregion Dependency Properties

		public ConcurrentStack<(Point start, Point end, string label)> LineLabelsSource { get; } = new ConcurrentStack<(Point start, Point end, string label)>();

		/// <summary>
		/// Keep a copy of the items source that can be accessed from a non-GUI thread
		/// </summary>
		private IEnumerable _itemsSource = null;

		private void SetItemsSource(IEnumerable itemsSource)
		{
			if (_itemsSource is INotifyCollectionChanged collectionOld)
			{
				collectionOld.CollectionChanged -= ItemsSource_CollectionChanged;
			}
			_itemsSource = itemsSource;
			if (_itemsSource is INotifyCollectionChanged collectionNew)
			{
				collectionNew.CollectionChanged += ItemsSource_CollectionChanged;
			}
			ResetRadioBandLines();
		}

		/// <summary>
		/// Clears the current lines on the plot, and rebuilds the data chain from the ItemsSource
		/// </summary>
		private void ResetRadioBandLines()
		{
			// Updates are throttled to 20 times per second
			LineLabelsSource.Clear();
			_groupToRadioBandLines.Clear();
			_ungroupedRadioBandLines.Clear();
			if (_itemsSource == null)
			{
				_groupAxis.UpdateGroups(Enumerable.Empty<object>());
				return;
			}

			_lineRenderer.ClearLines();
			// build up the lines and groups
			foreach (var item in (IEnumerable)_itemsSource)
			{
				// Create a RadioBandLine
				// The style for this is in the client control, for some reason we have to set DataContext to null,
				// add the control, and then set the DataContext to get the binding to be applied before we can
				// read it back out.
				RadioBandLine radioBandLine = new RadioBandLine();
				radioBandLine.DataContext = null;
				_lineRenderer.AddLine(radioBandLine);
				radioBandLine.DataContext = item;
				radioBandLine.Text = item.ToString();


				if (radioBandLine.Group != null)
				{
					var group = _groupToRadioBandLines.TryGetValueOrNew(radioBandLine.Group, () => new List<RadioBandLine>());
					group.Add(radioBandLine);
				}
				else
                {
					_ungroupedRadioBandLines.Add(radioBandLine);
                }

				// NOTE: If the RadioBandChartPlotter is created but not rendered (i.e. brought into view),
				// the depedency properties on the RadioBandLines are not set. Which means the code after this
				// will not work as radioBandLine.Group will always return null, so need to handle when
				// the Group property changes.
				radioBandLine.GroupChanged += (s, e) =>
				{
					if (s is RadioBandLine changingRadioBandLine)
					{
						if (e.OldGroup != null && _groupToRadioBandLines.TryGetValue(e.OldGroup, out var oldGroup))
						{
							oldGroup.Remove(changingRadioBandLine);
							if (oldGroup.Count < 1)
							{
								_groupToRadioBandLines.Remove(e.OldGroup);
							}
						}
						else if (e.OldGroup == null)
						{
							// Need to do this because the custom comparer doesn't ever return 0 for equal
							_ungroupedRadioBandLines.Remove(changingRadioBandLine);
						}

						if (e.NewGroup != null)
						{
							var newGroup = _groupToRadioBandLines.TryGetValueOrNew(e.NewGroup, () => new List<RadioBandLine>());
							newGroup.Add(changingRadioBandLine);
						}
						else
						{
							_ungroupedRadioBandLines.Add(changingRadioBandLine);

						}
						ResetChartFromGroups();
					}
				};
			}
			ResetChartFromGroups();
		}

		private void ResetChartFromGroups()
        {
			int indexMax = _groupToRadioBandLines.Count;
			if (DataTransform is RadioBandTransform rbt)
			{
				rbt.GroupCount = indexMax;
			}

			CalculateRadioBandYCoords();
			foreach (var radioBandLine in _groupToRadioBandLines.Values.SelectMany(x => x))
			{
				LineLabelsSource.Push((new Point(radioBandLine.Start, radioBandLine.GroupAxisCoord), new Point(radioBandLine.End, radioBandLine.GroupAxisCoord), radioBandLine.ToolTip?.ToString()));
			}

			_lineRenderer.InvalidateVisual();

			Restrictions.Clear();
			if (_radioBandPlotConfig.Ticks.Any() && _groupToRadioBandLines.Count > 0)
			{
				var bounds = new DataRect(
					_radioBandPlotConfig.Ticks.First(),
					0,
					_radioBandPlotConfig.Ticks.Last() - _radioBandPlotConfig.Ticks.First(),
					indexMax);

				Restrictions.Add(new MaximalDataRectRestriction(bounds));
			}
			_groupAxis.UpdateGroups(GetSortedGroups());

			((VerticalAxis)MainVerticalAxis).TicksProvider = new GroupTicksProvider(_groupToRadioBandLines.Count);
		}

		private List<IComparable> GetSortedGroups() => _groupToRadioBandLines.Keys.OrderBy(x => x).ToList();

		/// <summary>
		/// This goes through all the groups, counts the lines in each group and evenly spaces the lines,
		/// which are ordered by start frequency.
		/// </summary>
		private void CalculateRadioBandYCoords()
		{
			// Iterate through all the groups, spacing the lines accordingly
			// Each group should take up a range of 1 on the Y axis, so if there are 5 groups then the total range of the y axis will be 5
			var sortedGroups = GetSortedGroups();
			foreach (var groupAndLines in _groupToRadioBandLines)
			{
				int index = sortedGroups.Count - sortedGroups.IndexOf(groupAndLines.Key);
				// Now have the index, so add to that the fraction. Lines are sorted by start position
				var lineCount = groupAndLines.Value.Count;
				var scaler = 1.0 / lineCount;
				// Have a gap at the top and bottom, so add 2 to line count, and add 1 to the index
				int lineNo = 0;
				foreach (var lineSpec in groupAndLines.Value.OrderBy(l=>l.Start))
				{
					double yCoord = index - (0.5 + lineNo) * scaler;
					lineSpec.GroupAxisCoord = yCoord;
					++lineNo;
				}
			}
			foreach(var lines in _ungroupedRadioBandLines)
            {
				lines.GroupAxisCoord = 0;
			}
		}

		private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			// TODO Need to keep count by group, update line heights as new ones get added or removed
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
				case NotifyCollectionChangedAction.Move:
				case NotifyCollectionChangedAction.Remove:
				case NotifyCollectionChangedAction.Replace:
				case NotifyCollectionChangedAction.Reset:
					ResetRadioBandLines();
					break;
			}
		}
	}
}