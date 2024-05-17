using System;

namespace DynamicDataDisplay.FrequencyTimeline
{
    public interface IFrequencyTimelineChartData
    {
        public double LowerFrequency { get; }
        public double UpperFrequency { get; }
        public DateTime? StartTime { get; }
        public DateTime? EndTime { get; }
        public string Id { get; }
        public int ColorARGB { get; }
    }
}
