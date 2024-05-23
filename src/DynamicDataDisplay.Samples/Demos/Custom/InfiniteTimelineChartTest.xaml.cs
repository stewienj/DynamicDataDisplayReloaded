using DynamicDataDisplay.Common.Auxiliary;
using DynamicDataDisplay.FrequencyTimeline;
using DynamicDataDisplay.ViewportRestrictions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;

namespace DynamicDataDisplay.Samples.Demos.Custom
{
    /// <summary>
    /// Interaction logic for TimelineChartTest.xaml
    /// </summary>
    public partial class InfiniteTimelineChartTest : Page
    {
        public InfiniteTimelineChartTest()
        {
            DataContext = new InfiniteTimelineChartTestViewModel();
            InitializeComponent();
        }
    }

    public class InfiniteTimelineChartTestViewModel : D3NotifyPropertyChanged
    {
        private ThreadLocal<Random> _random = new ThreadLocal<Random>(() => new Random(1));

        public InfiniteTimelineChartTestViewModel()
        {
            var timelines = new List<IFrequencyTimelineChartData>();
            for (int i = 1; i < 20; ++i)
            {
                byte R = (byte)(_random.Value.NextDouble() * 255);
                byte G = (byte)(_random.Value.NextDouble() * 255);
                byte B = (byte)(255 - ((R + G) >> 1));
                Color.FromArgb(255, R, G, B).ToArgb();

                timelines.Add(new FrequencyTimelineChartData
                {
                    Id = $"Object No {i}",
                    LowerFrequency = Math.Pow(10.0, i * 0.5 - 0.05),
                    UpperFrequency = Math.Pow(10.0, i * 0.5 + 0.05),
                    StartTime = null,
                    EndTime = null,
                    ColorARGB = Color.FromArgb(255, R, G, B).ToArgb()
                });
            }
            Timelines = timelines;
        }

        public IEnumerable<IFrequencyTimelineChartData> Timelines
        {
            get;
        }

        private DateTime _startTime = DateTime.Now.Subtract(TimeSpan.FromDays(1));
        public DateTime StartTime
        {
            get => _startTime;
            set
            {
                if (SetProperty(ref _startTime, value))
                {
                    RaisePropertyChanged(nameof(MaxRect));
                }
            }
        }

        private DateTime _endTime = DateTime.Now.Add(TimeSpan.FromDays(1));
        public DateTime EndTime
        {
            get => _endTime;
            set
            {
                if (SetProperty(ref _endTime, value))
                {
                    RaisePropertyChanged(nameof(MaxRect));
                }
            }
        }

        public DataRect MaxRect
        {
            get
            {
                double minMinutes = (StartTime - DateTime.MinValue).TotalMinutes;
                double maxMinutes = (EndTime - DateTime.MinValue).TotalMinutes;

                return new DataRect(Math.Min(minMinutes, maxMinutes), double.NaN, Math.Abs(maxMinutes - minMinutes), double.NaN);
            }
        }

        public ViewportRestriction MaxRectRestriction => new DateTimeRestriction();

        public class DateTimeRestriction : ViewportRestriction
        {
            private readonly double minMinutes = 0;
            private readonly double maxMinutes = (DateTime.MaxValue - DateTime.MinValue).TotalMinutes;

            private readonly double minLogFrequency = -2;
            private readonly double maxLogFrequency = 12;


            public DateTimeRestriction()
            {
                AdjustMinMax(ref minMinutes, ref maxMinutes);
                AdjustMinMax(ref minLogFrequency, ref maxLogFrequency);
            }

            /// <summary>
            /// Mimics the adjustment the chart does to get the visible rectangle
            /// </summary>
            private void AdjustMinMax(ref double min, ref double max)
            {
                var midPoint = (min + max) * 0.5;
                var diff = max - min;
                min = midPoint - diff * 0.5 * 1.05;
                max = midPoint + diff * 0.5 * 1.05;
            }

            public override DataRect Apply(DataRect previousDataRect, DataRect proposedDataRect, Viewport2D viewport)
            {
                DataRect borderRect = DataRect.Create(minMinutes, minLogFrequency, maxMinutes, maxLogFrequency);
                if (proposedDataRect.IntersectsWith(borderRect))
                {
                    DataRect croppedRect = DataRect.Intersect(proposedDataRect, borderRect);
                    return croppedRect;
                }

                return previousDataRect;
            }
        }
    }
}
