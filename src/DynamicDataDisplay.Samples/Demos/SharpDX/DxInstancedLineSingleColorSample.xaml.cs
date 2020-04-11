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
	public partial class DxInstancedLineSingleColorSample : Page
	{
		public DxInstancedLineSingleColorSample()
		{
			var viewModel = new DxInstancedLineSingleColorViewModel();
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


	public class DxInstancedLineSingleColorViewModel : INotifyPropertyChanged, IDisposable
	{
		private int pointCount = 2_000;
		private int counter = 0;

		public volatile bool _hasBeenDisposed = false;

		public DxInstancedLineSingleColorViewModel()
		{
			Positions = new DxInstancePoint[]
			{
				new DxInstancePoint(0, 0.2f),
				new DxInstancePoint(0,    0),
				new DxInstancePoint(0,-0.2f)
			};
			StartCalculatingPoints1();
		}

		private DxPoint[] UpdateQueue(Queue<float> queue, float offset)
		{
			Thread.Sleep(10);
			while (queue.Count > pointCount - 1)
			{
				queue.Dequeue();
			}
			while (queue.Count < pointCount)
			{
				queue.Enqueue((float)(Math.Sin(Math.PI * 2 * counter / pointCount)));
				++counter;
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

		public void Dispose()
		{
			_hasBeenDisposed = true;
		}

		public IEnumerable<DxPoint> Points1 { get; set; }

		public IEnumerable<DxInstancePoint> Positions { get; set; }

		public Color Color1 => Colors.Red;

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
