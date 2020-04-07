using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DynamicDataDisplay.BitmapGraphs
{
	/// <summary>
	/// Interaction logic for HeatmapProgressBar.xaml
	/// </summary>
	public partial class HeatmapProgressBar : UserControl, INotifyPropertyChanged
	{
		public HeatmapProgressBar()
		{
			InitializeComponent();

			Heatmap.HeatmapCalculationProgress += (s, e) =>
			{
				var progress = e?.Progress;
				if (progress.HasValue)
				{
					ProgressBarVisibility = Visibility.Visible;
					ProgressBarPosition = Math.Round(progress.Value * 100);
				}
				else
				{
					ProgressBarVisibility = Visibility.Collapsed;
					ProgressBarPosition = 0;
				}
			};
		}

		private Visibility _progressBarVisibility = Visibility.Collapsed;
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

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
