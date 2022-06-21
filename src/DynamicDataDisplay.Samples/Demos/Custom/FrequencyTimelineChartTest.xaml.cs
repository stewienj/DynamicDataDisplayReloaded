using DynamicDataDisplay.Common.Auxiliary;
using DynamicDataDisplay.TimelineChart;
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
        public TimelineChartTestViewModel()
        {
            var baseTime = DateTime.Now;
            var timelines = new List<IFrequencyTimelineChartData>();
            for (int i = 1; i < 20; ++i)
            {
                timelines.Add(new FrequencyTimelineChartData
                {
                    Id = $"Object No {i}",
                    LowerFrequency = Math.Pow(10.0, i * 0.5 - 0.05),
                    UpperFrequency = Math.Pow(10.0, i * 0.5 + 0.05),
                    StartTime = baseTime + TimeSpan.FromHours(i-1),
                    EndTime = baseTime + TimeSpan.FromHours(i-1+12),
                    Color = Colors.Green
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
