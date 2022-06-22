using DynamicDataDisplay.Common.Auxiliary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DynamicDataDisplay.FrequencyTimeline
{
    /// <summary>
    /// A sample implementation of the ITimelineChartDataObject inteface
    /// </summary>
    public class FrequencyTimelineChartData : D3NotifyPropertyChanged, IFrequencyTimelineChartData
    {
        private string _id;
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private double _lowerFrequency;
        public double LowerFrequency
        {
            get => _lowerFrequency;
            set => SetProperty(ref _lowerFrequency, value);
        }

        private double _upperFrequency;
        public double UpperFrequency
        {
            get => _upperFrequency;
            set => SetProperty(ref _upperFrequency, value);
        }

        private DateTime _startTime;
        public DateTime StartTime
        {
            get => _startTime;
            set => SetProperty(ref _startTime, value);
        }

        private DateTime _endTime;
        public DateTime EndTime
        {
            get => _endTime;
            set => SetProperty(ref _endTime, value);
        }

        private int _colorARGB;
        public int ColorARGB
        {
            get => _colorARGB;
            set => SetProperty(ref _colorARGB, value);
        }
    }
}