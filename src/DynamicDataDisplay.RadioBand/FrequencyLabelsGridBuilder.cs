using DynamicDataDisplay;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DynamicDataDisplay.RadioBand
{
	/// <summary>
	/// Base class for build the grid that holds the labels (point labels, or range labels)
	/// </summary>
	public abstract class FrequencyLabelsGridBuilder
	{
		public virtual Grid BuildLabelsGridControl(int rowNo)
		{
			var labelGrid = new Grid();
			labelGrid.SetValue(Grid.RowProperty, rowNo);
			labelGrid.SetValue(Grid.ColumnProperty, 0);
			labelGrid.VerticalAlignment = VerticalAlignment.Stretch;
			labelGrid.HorizontalAlignment = HorizontalAlignment.Stretch;

			labelGrid
			  .ColumnDefinitions
			  .AddMany(CreateColumnDefinitions());

			labelGrid
			  .RowDefinitions
			  .AddMany(CreateRowDefinitions());

			foreach (var child in CreateGridChildControls())
			{
				labelGrid.Children.Add(child);
			}

			return labelGrid;

		}

		protected IEnumerable<ColumnDefinition> CreateColumnDefinitions()
		{
			var sizes = GetSizesForColumns().Select(x => $"{x}").Aggregate((a, b) => $"{a} {b}");

			return GetSizesForColumns()
			  .Select(w => new ColumnDefinition { Width = new GridLength(w, GridUnitType.Star) });
		}

		protected IEnumerable<RowDefinition> CreateRowDefinitions()
		{
			return GetSizesForRows()
			  .Select(w => new RowDefinition { Height = double.IsNaN(w) ? new GridLength(0, GridUnitType.Auto) : new GridLength(w, GridUnitType.Star) });
		}

		protected abstract IEnumerable<double> GetSizesForColumns();
		protected abstract IEnumerable<double> GetSizesForRows();
		protected abstract IEnumerable<UIElement> CreateGridChildControls();
	}
}
