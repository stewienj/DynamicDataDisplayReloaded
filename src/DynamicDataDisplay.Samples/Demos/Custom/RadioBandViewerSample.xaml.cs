﻿using DynamicDataDisplay.Common.Auxiliary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;

namespace DynamicDataDisplay.Samples.Demos.Custom
{
	/// <summary>
	/// Interaction logic for RadioBandViewerSample.xaml
	/// </summary>
	public partial class RadioBandViewerSample : Page
	{
		public RadioBandViewerSample()
		{
			InitializeComponent();
		}

		public RadioBandDemoViewModel RadioBandData { get; } = new RadioBandDemoViewModel();

	}

	public class RadioBandDemoViewModel : D3NotifyPropertyChanged
	{
		public RadioBandDemoViewModel()
		{
			Groups.AddMany(GroupFactory.GroupWithRandomFrequencies());
		}

		public List<Group> Groups { get; } = new List<Group>();

		public IEnumerable<object> FrequencyRanges => Groups.SelectMany(group => group.Ranges.Select(freq => new { Freq = freq, Group = group }));

		public IEnumerable<object> SpectrumBars { get; } = SpectrumFactory.GetRandomFrequenciesAndBandwidths(50).Select(fnb => new { Freq = fnb.Frequency, BW = fnb.Bandwidth }).ToList();

		private bool _showSpectrumOverlay = true;
		public bool ShowSpectrumOverlay
        {
			get => _showSpectrumOverlay;
			set => SetProperty(ref _showSpectrumOverlay, value);
        }
	}

	public class Group : IComparable
	{
		public string Name { get; set; }
		public List<FrequencyRange> Ranges { get; } = new List<FrequencyRange>();

		public int CompareTo(object obj)
		{
			if (obj is Group group)
			{
				return Name.CompareTo(group.Name);
			}
			else
			{
				return Name.CompareTo(obj?.ToString());
			}
		}

		public override string ToString()
		{
			return Name;
		}

	}

	public class FrequencyRange : INotifyPropertyChanged
	{
		public double Start { get; set; }
		public double End { get; set; }

		public string Description => $"{Start.ToString("G3")} to {End.ToString("G3")}";

		private bool _isSelected = false;
		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				if (value != _isSelected)
				{
					_isSelected = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}

	public static class GroupFactory
	{
		public static IEnumerable<Group> GroupWithRandomFrequencies()
		{
			for (int groupNo = 1; groupNo <= 5; ++groupNo)
			{
				// Create group, 5 in total
				Group group = new Group { Name = $"Group {groupNo}" };

				// Create 10 Random frequencies
				for (int freqNo = 0; freqNo < 10; ++freqNo)
				{
					double freq1 = SpectrumFactory.GetRandomFrequency();
					while (true)
					{
						double freq2 = SpectrumFactory.GetRandomFrequency();
						if (freq2 == freq1)
							continue;
						group.Ranges.Add(new FrequencyRange { Start = Math.Min(freq1, freq2), End = Math.Max(freq1, freq2) });
						break;
					}
				}
				yield return group;
			}
		}
	}

	public static class SpectrumFactory
    {
		private const double _maximumFrequency = 1000E9;
		private static Random _random = new Random(50000);
		private const bool _useLogScale = true;

		public static double GetRandomFrequency()
		{
			double freq = _useLogScale ?
			  1E7 * Math.Exp(Math.Log(_maximumFrequency / 1E7) * _random.NextDouble()) :
			  _maximumFrequency * _random.NextDouble();
			return freq;
		}

		public static double GetRandomBandwidth(double frequency)
        {
			// Bandwidth should be less than 10% of the frequency
			return frequency * 0.1 * _random.NextDouble();
        }

		public static (double Frequency, double Bandwidth) GetRandomFrequencyAndBandwidth()
        {
			var frequency = GetRandomFrequency();
			var bandwidth = GetRandomBandwidth(frequency);
			return (frequency, bandwidth);
        }

		public static IEnumerable<(double Frequency, double Bandwidth)> GetRandomFrequenciesAndBandwidths(int count)
        {
			for(int i=0; i<count;++i)
            {
				yield return GetRandomFrequencyAndBandwidth();

			}
        }

	}

}
