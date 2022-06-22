using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DynamicDataDisplay.FrequencyTimeline
{
    public interface IFrequencyTimelineChartData
    {
        public double LowerFrequency { get; }
        public double UpperFrequency { get; }
        public DateTime StartTime { get; }
        public DateTime EndTime { get; }
        public string Id { get; }
        public int ColorARGB { get; }
    }
}
