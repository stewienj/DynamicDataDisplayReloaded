using DynamicDataDisplay.Charts;
using DynamicDataDisplay.Charts.Axes.Numeric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DynamicDataDisplay.TimelineChart
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
                else
                    return freqHz.ToString("0 Hz");
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
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<IFrequencyTimelineChartData>), typeof(FrequencyTimelineChart), new PropertyMetadata(null));
    }
}
