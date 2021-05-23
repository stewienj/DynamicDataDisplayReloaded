using System;
using System.Windows;
using System.Windows.Media;

namespace DynamicDataDisplay.RadioBand
{
	/// <summary>
	/// SpectrumBandLine objects to be rendered by the FrequencyRangeLineRenderer.
	/// These are just data objects, but I add them as children to a panel so bindings can be
	/// applied. The direct parent is a Canvas type. Note the "AffectsParentMeasure = true"
	/// on the dependency properties. This forces a render pass in the parent if the property
	/// changes.
	/// </summary>
	public class SpectrumBandLine : FrameworkElement
	{
		private Size _zeroSize = new Size(0, 0);
		public SpectrumBandLine()
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


		/// <summary>
		/// Frequency is the position of the line
		/// </summary>
		public double Frequency
		{
			get { return (double)GetValue(FrequencyProperty); }
			set { SetValue(FrequencyProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Start.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty FrequencyProperty =
			DependencyProperty.Register("Frequency", typeof(double), typeof(SpectrumBandLine), new FrameworkPropertyMetadata{ DefaultValue = 0.0, AffectsParentMeasure = true, PropertyChangedCallback = new PropertyChangedCallback((s,e)=>
			{
				if (s is SpectrumBandLine spectrumBandLine)
                {
					spectrumBandLine.BrushNeedsRecaculating = true;
				}
			})});


		/// <summary>
		/// Bandwidth is the thickness of the line
		/// </summary>
		public double Bandwidth
		{
			get { return (double)GetValue(BandwidthProperty); }
			set { SetValue(BandwidthProperty, value); }
		}

		// Using a DependencyProperty as the backing store for End.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty BandwidthProperty =
			DependencyProperty.Register("Bandwidth", typeof(double), typeof(SpectrumBandLine), new FrameworkPropertyMetadata { DefaultValue = 0.0, AffectsParentMeasure = true,
				PropertyChangedCallback = new PropertyChangedCallback((s, e) =>
				{
					if (s is SpectrumBandLine spectrumBandLine)
					{
						spectrumBandLine.BrushNeedsRecaculating = true;
					}
				})
			});

		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}

		// Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register("IsSelected", typeof(bool), typeof(SpectrumBandLine), new FrameworkPropertyMetadata { DefaultValue = false, AffectsParentMeasure = true, BindsTwoWayByDefault = true });

		/// <summary>
		/// Line Brush, which will be a gradient brush that has to be calculated for a Log scale
		/// </summary>
		internal Brush LineBrush 
		{
			get => _lineBrush;
			set
            {
				_lineBrush = value;
				BrushNeedsRecaculating = false;
            }
		}
		private Brush _lineBrush = new SolidColorBrush(Colors.Blue);

		/// <summary>
		/// Recalculate the LineBrush which will need doing whenever the frequency or bandwidth changes because it is being displayed on a log scale.
		/// </summary>
		internal bool BrushNeedsRecaculating { get; private set; } = false;
	}

}
