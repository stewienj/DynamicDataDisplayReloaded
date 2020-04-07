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

	public class RadioBandDemoViewModel
	{
		public RadioBandDemoViewModel()
		{
			Groups.AddMany(GroupFactory.GroupWithRandomFrequencies());
		}

		public List<Group> Groups { get; } = new List<Group>();

		public IEnumerable<object> FrequencyRanges => Groups.SelectMany(group => group.Ranges.Select(freq => new { Freq = freq, Group = group }));
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
		private const double _maximumFrequency = 1000E9;
		private static Random _random = new Random(50000);
		private const bool _useLogScale = true;

		public static IEnumerable<Group> GroupWithRandomFrequencies()
		{
			for (int groupNo = 1; groupNo <= 5; ++groupNo)
			{
				// Create group, 5 in total
				Group group = new Group { Name = $"Group {groupNo}" };

				// Create 10 Random frequencies
				for (int freqNo = 0; freqNo < 10; ++freqNo)
				{
					double freq1 = GetRandomFrequency();
					while (true)
					{
						double freq2 = GetRandomFrequency();
						if (freq2 == freq1)
							continue;
						group.Ranges.Add(new FrequencyRange { Start = Math.Min(freq1, freq2), End = Math.Max(freq1, freq2) });
						break;
					}
				}
				yield return group;
			}
		}

		private static double GetRandomFrequency()
		{
			double freq = _useLogScale ?
			  1E7 * Math.Exp(Math.Log(_maximumFrequency / 1E7) * _random.NextDouble()) :
			  _maximumFrequency * _random.NextDouble();
			return freq;
		}
	}
}
