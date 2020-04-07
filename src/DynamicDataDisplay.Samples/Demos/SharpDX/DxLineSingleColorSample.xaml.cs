using DynamicDataDisplay.SharpDX9.DataTypes;
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
	/// Interaction logic for DxLineSingleColorSample.xaml
	/// </summary>
	public partial class DxLineSingleColorSample : Page
	{
		public DxLineSingleColorSample()
		{
			var viewModel = new DxLineSingleColorViewModel();
			DataContext = viewModel;
			IsVisibleChanged += (s, e) =>
			{
				if (!IsVisible)
				{
					viewModel.Dispose();
				}
			};
			InitializeComponent();
		}
	}

	public class DxLineSingleColorViewModel : INotifyPropertyChanged, IDisposable
	{
		private int pointCount = 2_000;
		// Random isn't thread safe
		private ThreadLocal<Random> random = new ThreadLocal<Random>(()=>new Random());

		public volatile bool _hasBeenDisposed = false;

		public DxLineSingleColorViewModel()
		{
			StartCalculatingPoints1();
			StartCalculatingPoints2();
			StartCalculatingPoints3();
		}

		private DxPoint[] UpdateQueue(Queue<float> queue, float offset)
		{
			Thread.Sleep(10);
			while (queue.Count > pointCount-10)
			{
				queue.Dequeue();
			}
			while(queue.Count < pointCount)
			{
				queue.Enqueue((float)(random.Value.NextDouble() * 2.0) + offset);
			}
			return queue.Select((v, i) => new DxPoint((float)i, v)).ToArray();
		}

		private void StartCalculatingPoints1()
		{
			Task.Factory.StartNew(() =>
			{
				Queue<float> queue = new Queue<float>();
				while (!_hasBeenDisposed)
				{
					Points1 = UpdateQueue(queue, 1f);
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Points1)));
				}
			}, TaskCreationOptions.LongRunning);
		}

		private void StartCalculatingPoints2()
		{
			Task.Factory.StartNew(() =>
			{
				Queue<float> queue = new Queue<float>();
				while (!_hasBeenDisposed)
				{
					Points2 = UpdateQueue(queue, -1f);
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Points2)));
				}
			}, TaskCreationOptions.LongRunning);
		}
		private void StartCalculatingPoints3()
		{
			Task.Factory.StartNew(() =>
			{
				Queue<float> queue = new Queue<float>();
				while (!_hasBeenDisposed)
				{
					Points3 = UpdateQueue(queue, -3f);
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Points3)));
				}
			}, TaskCreationOptions.LongRunning);
		}

		public void Dispose()
		{
			_hasBeenDisposed = true;
		}

		public IEnumerable<DxPoint> Points1 { get; set; }
		public IEnumerable<DxPoint> Points2 { get; set; }
		public IEnumerable<DxPoint> Points3 { get; set; }

		public Color Color1 => Colors.Red;
		public Color Color2 => Colors.Green;
		public Color Color3 => Colors.Blue;

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
