using DynamicDataDisplay.Common.Auxiliary;
using DynamicDataDisplay.Common.Palettes;
using DynamicDataDisplay.ViewModelTypes;
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

namespace DynamicDataDisplay.SamplesDX9.Demos.SharpDX
{
	/// <summary>
	/// Interaction logic for DxMarkerColoredByInstanceSample.xaml
	/// </summary>
	public partial class DxMarkerColoredByInstanceSample : Page
	{
		public DxMarkerColoredByInstanceSample()
		{
			var viewModel = new DxMarkerColoredByInstanceViewModel();
			DataContext = viewModel;
			IsVisibleChanged += (s, e) =>
			{
				if (!IsVisible)
				{
					viewModel.Dispose();
				}
			};
			Loaded += (s,e)=>
			{
				plotter.Visible = new DataRect(-.1, -.1, 1.2, 1.2);
				plotter.Viewport.PropertyChanged += (s2, e2) =>
				{
					var OnePixA = plotter.Viewport.Transform.ScreenToData(new Point(1, 1));
					var OnePixB = plotter.Viewport.Transform.ScreenToData(new Point(2, 2));
					viewModel.Width1Px = OnePixB.X - OnePixA.X;
					viewModel.Height1Px = OnePixB.Y - OnePixA.Y;
				};
			};
			InitializeComponent();
		}
	}


	public class DxMarkerColoredByInstanceViewModel : INotifyPropertyChanged, IDisposable
	{
		private static int _pointCount = 1_000_000;
		private D3InstancedPointAndColor[] _currentArray = new D3InstancedPointAndColor[_pointCount];
		private ThreadLocal<Random> random = new ThreadLocal<Random>(() => new Random());
		public volatile bool _hasBeenDisposed = false;
		public volatile bool _animationRunning = false;
		public volatile bool _doReset = true;

		public DxMarkerColoredByInstanceViewModel()
		{
			int sideCount = 10;
			// Start and end are the same, and need centre as this is a triangle fan
			var geometry = new List<D3Point>();
			geometry.Add(new D3Point(0, 0));
			var multiplier = Math.PI * 2.0 / sideCount;
			for(int i=0; i<=sideCount;++i)
			{
				var x = 20.0 * Math.Sin(i * multiplier);
				var y = 20.0 * Math.Cos(i * multiplier);
				geometry.Add(new D3Point(x, y));
			}
			Geometry = geometry;
			StartArrayUpdate();
		}

		private void DoReset()
		{
			var rnd = random.Value;
			HSBPalette palette = new HSBPalette();
			for (int i = 0; i < _pointCount; i++)
			{
				var point = new Point(rnd.NextDouble(), rnd.NextDouble());
				var length = Math.Sqrt(point.X * point.X + point.Y * point.Y) / Math.Sqrt(2);
				var color = palette.GetColor(length);
				_currentArray[i] = new D3InstancedPointAndColor(point, color);
			}
			var temp = (Positions as D3InstancedPointAndColor[]) ?? new D3InstancedPointAndColor[_pointCount];
			Positions = _currentArray;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Positions)));
			_currentArray = temp;
		}

		private void StartArrayUpdate()
		{
			Task.Factory.StartNew(() =>
			{
				// Create the random directions
				var rnd = random.Value;
				Vector[] directions = Enumerable.Range(0, _pointCount).Select(i =>
				{
					var angle = rnd.NextDouble() * Math.PI * 2;
					Vector dxdy = new Vector(Math.Sin(angle), Math.Cos(angle));
					return dxdy;
				}).ToArray();

				while (!_hasBeenDisposed)
				{
					if (_doReset)
					{
						DoReset();
						_doReset = false;
					}
					if (_animationRunning)
					{

						var positions = Positions as D3InstancedPointAndColor[];
						for (int i = 0; i < _pointCount; ++i)
						{
							var dxdy = new Vector(directions[i].X * Width1Px, directions[i].Y * Height1Px);
							var newX = dxdy.X + positions[i].X;
							var newY = dxdy.Y + positions[i].Y;
							if (newX < 0 || newX > 1)
							{
								newX = Math.Min(1, Math.Max(0, newX));
								directions[i] = new Vector(-directions[i].X, directions[i].Y);
							}
							if (newY < 0 || newY > 1)
							{
								newY = Math.Min(1, Math.Max(0, newY));
								directions[i] = new Vector(directions[i].X, -directions[i].Y);
							}

							_currentArray[i] = new D3InstancedPointAndColor(newX, newY, positions[i].Color);
						}
						var temp = (Positions as D3InstancedPointAndColor[]) ?? new D3InstancedPointAndColor[_pointCount];
						Positions = _currentArray;
						PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Positions)));
						_currentArray = temp;
					}
					Thread.Sleep(10);
				}
			});
		}

		public void Dispose()
		{
			_hasBeenDisposed = true;
		}

		// The width of 1 pixel in data space
		public double Width1Px { get; set; }
		// The height of 1 pixel in data space
		public double Height1Px { get; set; }

		public IEnumerable<D3Point> Geometry { get; set; }

		public IEnumerable<D3InstancedPointAndColor> Positions { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		private RelayCommandFactoryD3 _stopAnimaion = new RelayCommandFactoryD3();
		public ICommand StopAnimation => _stopAnimaion.GetCommand(() => _animationRunning = false);

		private RelayCommandFactoryD3 _startAnimation = new RelayCommandFactoryD3();
		public ICommand StartAnimation => _startAnimation.GetCommand(() => _animationRunning = true);

		private RelayCommandFactoryD3 _resetCircles = new RelayCommandFactoryD3();
		public ICommand ResetCircles => _resetCircles.GetCommand(() => _doReset = true);
	}
}
