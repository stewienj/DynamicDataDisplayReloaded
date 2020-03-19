using System;
using System.Windows;

namespace DynamicDataDisplay.Markers
{
	public static class PieChartStyles
	{
		private static Style defaultStyle = null;
		public static Style Default
		{
			get
			{
				if (defaultStyle == null)
				{
					ResourceDictionary genericDict = (ResourceDictionary)Application.LoadComponent(new Uri("/DynamicDataDisplay.Markers;component/Themes/Generic.xaml", UriKind.Relative));
					defaultStyle = (Style)genericDict[typeof(PieChartItem)];
				}
				return defaultStyle;
			}
		}

		private static Style donut = null;
		public static Style Donut
		{
			get
			{
				if (donut == null)
				{
					ResourceDictionary genericDict = (ResourceDictionary)Application.LoadComponent(new Uri("/DynamicDataDisplay.Markers;component/Themes/Generic.xaml", UriKind.Relative));
					donut = (Style)genericDict["donutPieChartItemStyle"];
				}
				return donut;
			}
		}
	}
}
