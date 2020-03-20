using System.Collections.Generic;
using System.Windows;

namespace DynamicDataDisplay.RadioBand
{
	/// <summary>
	/// Source for lines to be checked against to see if the cursor is near them.
	/// Has to be a FrameworkElement else binding to other elements in the Xaml isn't possible
	/// </summary>
	public class NearestVisualSource : FrameworkElement
	{
		public IEnumerable<(Point start, Point end, string label)> ItemsSource
		{
			get { return (IEnumerable<(Point, Point, string)>)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		// Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register("ItemsSource", typeof(IEnumerable<(Point, Point, string)>), typeof(NearestVisualSource), new PropertyMetadata(null));
	}

	public class NearestLineSource : NearestVisualSource { }

	public class NearestRectangleSource : NearestVisualSource { }

}
