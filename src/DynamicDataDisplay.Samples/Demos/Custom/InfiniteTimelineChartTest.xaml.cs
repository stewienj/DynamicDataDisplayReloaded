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
            var baseTime = DateTime.Now;
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
                    StartTime = DateTime.MinValue,
                    EndTime = DateTime.MaxValue,
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
