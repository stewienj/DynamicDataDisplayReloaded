using DynamicDataDisplay.SharpDX9.DataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace DynamicDataDisplay.Samples.Demos.SharpDX
{
	/// <summary>
	/// Interaction logic for WaveyLineSample.xaml
	/// </summary>
	public partial class WaveyLineSample : Page
	{
		public WaveyLineSample()
		{
			this.DataContext = new WaveyLineViewModel();
			InitializeComponent();
		}
	}

	public class WaveyLineViewModel : INotifyPropertyChanged
	{
		int pointCount = 10_000;
		double scaler = 1.0 / 10_000;

		public WaveyLineViewModel()
		{
			StartCalculatingPoints1();
			StartCalculatingPoints2();
			StartCalculatingPoints3();
		}

		private void StartCalculatingPoints1()
		{
			Task.Factory.StartNew(()=> 
			{
				DateTime startTime = DateTime.Now;
				while (true)
				{
					float inversePointMax = 1f / (pointCount - 1);
					double phase = -2.0 * Math.PI * (startTime - DateTime.Now).TotalSeconds / 20.0;

					Points1 =
						Enumerable.Range(0, pointCount)
						.Select(i =>
						{
							float x = (float)(2.0 * Math.PI * i * scaler);
							float y = (float)(15*Math.Sin(x + phase) * Math.Sin(x * 101.0));
							float colorIndex = (y+15f)/5f % 1f;
							return new DxPointAndColor(x, y, 3, 0, colorIndex, 1f - colorIndex);
						});
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Points1)));
					Thread.Sleep(10);
				}
			}, TaskCreationOptions.LongRunning);
		}

		private void StartCalculatingPoints2()
		{
			Task.Factory.StartNew(() =>
			{
				DateTime startTime = DateTime.Now;
				while (true)
				{
					float inversePointMax = 1f / (pointCount - 1);
					double phase = 2.0 * Math.PI * (startTime - DateTime.Now).TotalSeconds / 10.0;

					Points2 =
						Enumerable.Range(0, pointCount)
						.Select(i =>
						{
							float x = (float)(2.0 * Math.PI * i * scaler);
							float y = (float)(10*Math.Sin(x + phase) * Math.Sin(x * 102.0));
							float colorIndex = (y + 10f) / 5f % 1f;
							return new DxPointAndColor(x, y, 2, colorIndex, 1f - colorIndex, 0);
						});
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Points2)));
					Thread.Sleep(10);
				}
			}, TaskCreationOptions.LongRunning);
		}
		private void StartCalculatingPoints3()
		{
			Task.Factory.StartNew(() =>
			{
				DateTime startTime = DateTime.Now;
				while (true)
				{
					float inversePointMax = 1f / (pointCount - 1);
					double phase = 2.0 * Math.PI * (startTime - DateTime.Now).TotalSeconds / 5.0;

					Points3 =
						Enumerable.Range(0, pointCount)
						.Select(i =>
						{
							float x = (float)(2.0 * Math.PI * i * scaler);
							float y = (float)(20.0 * Math.Sin(x) * Math.Sin(x * 103.0) * Math.Sin(phase));
							float colorIndex = (y + 20f) / 5f % 1f;
							return new DxPointAndColor(x, y, 1, 1f - colorIndex, 0, colorIndex);
						});
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Points3)));
					Thread.Sleep(10);
				}
			}, TaskCreationOptions.LongRunning);
		}


		public IEnumerable<DxPointAndColor> Points1 { get; set; }
		public IEnumerable<DxPointAndColor> Points2 { get; set; }
		public IEnumerable<DxPointAndColor> Points3 { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
