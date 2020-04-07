using System.Windows;
using System.Windows.Data;

namespace DynamicDataDisplay.Charts.Legend_items
{
	public static class LegendItemsHelper
	{
		public static NewLegendItem BuildDefaultLegendItem(IPlotterElement chart)
		{
			DependencyObject dependencyChart = (DependencyObject)chart;

			NewLegendItem result = new NewLegendItem();
			SetCommonBindings(result, chart);
			return result;
		}

		public static void SetCommonBindings(NewLegendItem legendItem, object chart)
		{
			legendItem.DataContext = chart;
			legendItem.SetBinding(NewLegend.VisualContentProperty, new Binding { Path = new PropertyPath("(0)", NewLegend.VisualContentProperty) });
			legendItem.SetBinding(NewLegend.DescriptionProperty, new Binding { Path = new PropertyPath("(0)", NewLegend.DescriptionProperty) });
		}

	}
}
