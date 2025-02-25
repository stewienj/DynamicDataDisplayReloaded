using DynamicDataDisplay.Common.Auxiliary;
using DynamicDataDisplay.FrequencyTimeline;
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
    public partial class FrequencyTimelineChartTest : Page
    {
        public FrequencyTimelineChartTest()
        {
            this.DataContext = new TimelineChartTestViewModel();
            InitializeComponent();
        }
    }

    public class TimelineChartTestViewModel : D3NotifyPropertyChanged
    {
        private ThreadLocal<Random> _random = new ThreadLocal<Random>(() => new Random(1));

        public TimelineChartTestViewModel()
        {
            int lineCount = 20;
            var baseTime = DateTime.Now;
            var timelines = new List<IFrequencyTimelineChartData>();
            for (int i = 1; i < lineCount; ++i)
            {
                byte R = (byte)(_random.Value.NextDouble() * 255);
                byte G = (byte)(_random.Value.NextDouble() * 255);
                byte B = (byte)(255 - ((R + G) >> 1));
                Color.FromArgb(255, R, G, B).ToArgb();

                timelines.Add(new FrequencyTimelineChartData
                {
                    Id = $"Object No {i}",
                    LowerFrequency = Math.Pow(10.0, i * 10.0 / lineCount - 1.0 / lineCount),
                    UpperFrequency = Math.Pow(10.0, i * 10.0 / lineCount + 1.0 / lineCount),
                    StartTime = baseTime + TimeSpan.FromHours(i * 20.0 / lineCount - 1),
                    EndTime = baseTime + TimeSpan.FromHours(i * 20.0 / lineCount - 1 + 12),
                    ColorARGB = Color.FromArgb(255, R, G, B).ToArgb()
                });
            }
            Timelines = timelines;
        }

        public IEnumerable<IFrequencyTimelineChartData> Timelines
        {
            get;
        }
    }
}
