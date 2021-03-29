using System;
using System.Windows;

namespace DynamicDataDisplay.RadioBand
{
	/// <summary>
	/// RadioBandLine objects to be rendered by the FrequencyRangeLineRenderer.
	/// These are just data objects, but I add them as children to a panel so bindings can be
	/// applied. The direct parent is a Canvas type. Note the "AffectsParentMeasure = true"
	/// on the dependency properties. This forces a render pass in the parent if the property
	/// changes.
	/// </summary>
	public class RadioBandLine : FrameworkElement
	{
		private Size _zeroSize = new Size(0, 0);
		public RadioBandLine()
		{
			// This isn't visible anyway, so just turn stuff off
			IsHitTestVisible = false;
			Visibility = Visibility.Collapsed;
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			return _zeroSize;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			return _zeroSize;
		}

		public double Start
		{
			get { return (double)GetValue(StartProperty); }
			set { SetValue(StartProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Start.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty StartProperty =
			DependencyProperty.Register("Start", typeof(double), typeof(RadioBandLine), new FrameworkPropertyMetadata { DefaultValue = 0.0, AffectsParentMeasure = true });


		public double End
		{
			get { return (double)GetValue(EndProperty); }
			set { SetValue(EndProperty, value); }
		}

		// Using a DependencyProperty as the backing store for End.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty EndProperty =
			DependencyProperty.Register("End", typeof(double), typeof(RadioBandLine), new FrameworkPropertyMetadata { DefaultValue = 0.0, AffectsParentMeasure = true });


		public double GroupAxisCoord
		{
			get { return (double)GetValue(GroupAxisCoordProperty); }
			set { SetValue(GroupAxisCoordProperty, value); }
		}

		// Using a DependencyProperty as the backing store for GroupAxisCoord.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty GroupAxisCoordProperty =
			DependencyProperty.Register("GroupAxisCoord", typeof(double), typeof(RadioBandLine), new FrameworkPropertyMetadata { DefaultValue = 0.0, AffectsParentMeasure = true });


		public IComparable Group
		{
			get { return (IComparable)GetValue(GroupProperty); }
			set { SetValue(GroupProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Group.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty GroupProperty =
			DependencyProperty.Register("Group", typeof(IComparable), typeof(RadioBandLine), new FrameworkPropertyMetadata((s, e) =>
			{
				if (s is RadioBandLine line)
				{
					line.GroupChanged?.Invoke(line, (e.OldValue as IComparable, e.NewValue as IComparable));
				}
			}) { DefaultValue = null, AffectsParentMeasure = true });

		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}

		// Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register("IsSelected", typeof(bool), typeof(RadioBandLine), new FrameworkPropertyMetadata { DefaultValue = false, AffectsParentMeasure = true, BindsTwoWayByDefault = true });

		public string Text { get; set; } = "";

		public event EventHandler<(IComparable OldGroup, IComparable NewGroup)> GroupChanged;
	}

}
