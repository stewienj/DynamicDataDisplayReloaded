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
            this.DataContext = new InfiniteTimelineChartTestViewModel();
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

        public ViewportRestriction MaxRect => new DateTimeRestriction();

        public class DateTimeRestriction : ViewportRestriction
        {
            private readonly double minMinutes = (DateTime.Now - DateTime.MinValue.AddDays(1)).TotalMinutes;
            private readonly double maxMinutes = (DateTime.Now.AddDays(1) - DateTime.MinValue).TotalMinutes;

            public override DataRect Apply(DataRect previousDataRect, DataRect proposedDataRect, Viewport2D viewport)
            {
                // Min Max data
                var minY = 1.0 * 0.5 - 0.05;
                var maxY = 19.0 * 0.5 + 0.05;

                // Now do the adjustment the chart does to get the visible rectangle
                var midPoint = (minY + maxY) * 0.5;
                var diff = maxY - minY;
                minY = midPoint - diff * 0.5 * 1.05;
                maxY = midPoint + diff * 0.5 * 1.05;

                DataRect borderRect = DataRect.Create(minMinutes, minY, maxMinutes, maxY);
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
