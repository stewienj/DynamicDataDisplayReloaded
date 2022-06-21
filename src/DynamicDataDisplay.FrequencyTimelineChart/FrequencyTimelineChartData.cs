using DynamicDataDisplay.Common.Auxiliary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DynamicDataDisplay.TimelineChart
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
            set
            {
                if (SetProperty(ref _lowerFrequency, value))
                {
                    RaisePropertyChanged(nameof(Y), nameof(Height));
                }
            }
        }

        private double _upperFrequency;
        public double UpperFrequency
        {
            get => _upperFrequency;
            set
            {
                if (SetProperty(ref _upperFrequency, value))
                {
                    RaisePropertyChanged(nameof(Y), nameof(Height));
                }
            }
        }

        private DateTime _baseTime = new DateTime();
        public DateTime BaseTime
        {
            get => _baseTime;
            set
            {
                if (SetProperty(ref _baseTime, value))
                {
                    RaisePropertyChanged(nameof(X), nameof(Width));
                }
            }
        }


        private DateTime _startTime;
        public DateTime StartTime
        {
            get => _startTime;
            set
            {
                if (SetProperty(ref _startTime, value))
                {
                    RaisePropertyChanged(nameof(X), nameof(Width));
                }

            }
        }

        private DateTime _endTime;
        public DateTime EndTime
        {
            get => _endTime;
            set
            {
                if (SetProperty(ref _endTime, value))
                {
                    RaisePropertyChanged(nameof(X), nameof(Width));
                }
            }
        }

        private Color _color;
        public Color Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }

        public double X => (StartTime - BaseTime).TotalMinutes;
        public double Y => 0.5 * (Math.Log10(UpperFrequency) + Math.Log10(LowerFrequency));

        public double Width => (EndTime - StartTime).TotalMinutes;
        public double Height => Math.Log10(UpperFrequency) - Math.Log10(LowerFrequency);

    }
}