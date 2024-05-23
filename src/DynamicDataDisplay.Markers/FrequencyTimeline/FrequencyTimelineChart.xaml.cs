using DynamicDataDisplay.Charts;
using DynamicDataDisplay.Charts.Axes.Numeric;
using DynamicDataDisplay.ViewportRestrictions;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DynamicDataDisplay.FrequencyTimeline
{
    /// <summary>
    /// Interaction logic for TimelineChartView.xaml
    /// </summary>
    public partial class FrequencyTimelineChart : ChartPlotter
    {
        public FrequencyTimelineChart()
        {
            InitializeComponent();

            RemoveOldAxii();
            SetupHorizontalHourAxis();
            SetupVerticalFrequencyAxis();
        }

        private void RemoveOldAxii()
        {
            var oldAxii = Children.OfType<NumericAxis>().ToList();
            foreach (var oldAxis in oldAxii)
            {
                Children.Remove(oldAxis);
            }
        }

        private void SetupHorizontalHourAxis()
        {
            var utcAxis = new HorizontalAxis
            {
                TicksProvider = new CustomBaseNumericTicksProvider(60),
                LabelProvider = new UtcDateTimeLabelProvider(),
                Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#667E9F")),
                Foreground = Brushes.White,
            };
            utcAxis.AxisControl.TicksPath.Stroke = Brushes.White;

            Children.Add(utcAxis);

            Children.Add(new HorizontalAxisTitle
            {
                Margin = new Thickness(0, 5, 0, 2),
                FontFamily = new FontFamily("Georgia"),
                FontSize = 14,
                Content = "UTC Time"
            });
        }

        private void SetupVerticalFrequencyAxis()
        {
            VerticalAxis verticalAxis = new VerticalAxis();

            verticalAxis.TicksProvider = new LogarithmNumericTicksProvider();
            verticalAxis.LabelProvider = new ToStringLabelProvider();
            verticalAxis.LabelProvider.SetCustomFormatter((info) =>
            {
                double freqHz = info.Tick;
                if (freqHz >= 1e12)
                    return (freqHz / 1e12).ToString("0 THz");
                else if (freqHz >= 1e9)
                    return (freqHz / 1e9).ToString("0 GHz");
                else if (freqHz >= 1e6)
                    return (freqHz / 1e6).ToString("0 MHz");
                else if (freqHz >= 1e3)
                    return (freqHz / 1e3).ToString("0 KHz");
                else if (freqHz >= 1)
                    return freqHz.ToString("0 Hz");
                else if (freqHz >= 0.1)
                    return freqHz.ToString("0.0 Hz");
                else if (freqHz >= 0.01)
                    return freqHz.ToString("0.00 Hz");
                else if (freqHz >= 0.001)
                    return freqHz.ToString("0.000 Hz");
                else
                    return "?";
            });
            verticalAxis.LabelProvider.SetCustomView((info, ui) =>
            {
                TextBlock text = (TextBlock)ui;
                text.Background = Brushes.Black;
                text.Foreground = Brushes.White;
                text.FontWeight = FontWeights.Bold;
            });

            Children.Add(verticalAxis);

            Children.Add(new VerticalAxisTitle
            {
                Margin = new Thickness(5, 0, 5, 0),
                FontFamily = new FontFamily("Georgia"),
                FontSize = 14,
                Content = "Frequency (Hz)"
            });
        }

        public IEnumerable<IFrequencyTimelineChartData> ItemsSource
        {
            get { return (IEnumerable<IFrequencyTimelineChartData>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<IFrequencyTimelineChartData>), typeof(FrequencyTimelineChart), new PropertyMetadata(null, (s,e)=>
            {
                // For some reason data binding was failing internally on this, so just push it from code behind
                if (s is FrequencyTimelineChart chart)
                {
                    chart._horizontalBarsMarkers.ItemsSource = e.NewValue;
                    chart._labelsMarkers.ItemsSource = e.NewValue;
                }
            }));

        public bool ShowLabels
        {
            get { return (bool)GetValue(ShowLabelsProperty); }
            set { SetValue(ShowLabelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowLabels.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowLabelsProperty =
            DependencyProperty.Register("ShowLabels", typeof(bool), typeof(FrequencyTimelineChart), new PropertyMetadata(true, (s, e) => 
            {
                if (s is FrequencyTimelineChart chart && e.NewValue is bool showLabels)
                {
                    chart._labelsMarkers.Visibility = showLabels ? Visibility.Visible : Visibility.Collapsed;
                }
            }));

        public ViewportRestriction ViewportRestriction
        {
            get { return (ViewportRestriction)GetValue(ViewportRestrictionProperty); }
            set { SetValue(ViewportRestrictionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxRectRestriction.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewportRestrictionProperty =
            DependencyProperty.Register("ViewportRestriction", typeof(ViewportRestriction), typeof(FrequencyTimelineChart), new PropertyMetadata(null, (s,e) =>
            {
                if (s is FrequencyTimelineChart chart)
                {
                    if (e.OldValue is ViewportRestriction oldRestriction)
                    {
                        chart.Viewport.Restrictions.Remove(oldRestriction);
                    }
                    if (e.NewValue is ViewportRestriction newRestriction)
                    {
                        chart.Viewport.Restrictions.Add(newRestriction);
                    }
                }
            }));

        public DataRect? ContentBoundsOverride
        {
            get { return (DataRect?)GetValue(ContentBoundsOverrideProperty); }
            set { SetValue(ContentBoundsOverrideProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentBoundsOverride.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentBoundsOverrideProperty =
            DependencyProperty.Register("ContentBoundsOverride", typeof(DataRect?), typeof(FrequencyTimelineChart), new PropertyMetadata(null, (s, e) =>
            {
                if (s is FrequencyTimelineChart chart)
                {
                    chart.Viewport.Visible = new DataRect(0, 0, 1, 1);
                    chart.Viewport.FitToView();
                }
            }));

        private void Markers_OverrideContentBoundsEvent(object sender, Markers.OverrideContentBoundsArgs e)
        {
            if (ContentBoundsOverride is DataRect dataRect)
            {
                e.ContentBounds = new DataRect
                (
                    dataRect.XMin.IsNaN() ? e.ContentBounds.XMin : dataRect.XMin,
                    dataRect.YMin.IsNaN() ? e.ContentBounds.YMin : dataRect.YMin,
                    dataRect.Width.IsNaN() ? e.ContentBounds.Width : dataRect.Width,
                    dataRect.Height.IsNaN() ? e.ContentBounds.Height : dataRect.Height
                );
            }
        }
    }
}
