using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Research.DynamicDataDisplay.BitmapGraphs
{
	/// <summary>
	/// Interaction logic for HeatmapProgressBar.xaml
	/// </summary>
	public partial class GroupedMarkersProgressBar : UserControl, INotifyPropertyChanged
	{
		public GroupedMarkersProgressBar()
		{
			InitializeComponent();

			GroupedMarkers.GroupedMarkersCalculationProgress += (s, e) =>
			{
				var progress = e?.Progress;
				if (progress.HasValue)
				{
					ProgressBarVisibility = Visibility.Visible;
					ProgressBarPosition = Math.Round(progress.Value * 100);
				}
				else
				{
					ProgressBarVisibility = Visibility.Hidden;
					ProgressBarPosition = 0;
				}
				LastTimeTaken = e.CalculationTime.TotalSeconds.ToString() + " seconds";
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastTimeTaken)));
			};
		}

		private Visibility _progressBarVisibility = Visibility.Hidden;
		public Visibility ProgressBarVisibility
		{
			get => _progressBarVisibility;
			set
			{
				if (_progressBarVisibility != value)
				{
					_progressBarVisibility = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProgressBarVisibility)));
				}
			}
		}

		private double _progressBarPosition = 0;
		public double ProgressBarPosition
		{
			get => _progressBarPosition;
			set
			{
				if (_progressBarPosition != value)
				{
					_progressBarPosition = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProgressBarPosition)));
				}
			}
		}

		public string LastTimeTaken { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
