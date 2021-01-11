using DynamicDataDisplay.Common.Auxiliary;
using DynamicDataDisplay.Common.Palettes;
using DynamicDataDisplay.SharpDX9.DataTypes;
using DynamicDataDisplay.SharpDX9.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DynamicDataDisplay.Samples.Demos.SharpDX
{
    /// <summary>
    /// Interaction logic for DxRectangleTexturedSample.xaml
    /// </summary>
    public partial class DxRectangleTexturedSample : Page
	{
		public DxRectangleTexturedSample()
		{
			var viewModel = new DxRectangleTexturedSampleViewModel();
			DataContext = viewModel;
			IsVisibleChanged += (s, e) =>
			{
				if (!IsVisible)
				{
					viewModel.Dispose();
				}
			};
			Loaded += (s, e) =>
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


	public class DxRectangleTexturedSampleViewModel : INotifyPropertyChanged, IDisposable
	{
		private static int _pointCount = 10000;
		private Point[] _points = new Point[_pointCount];
		private DxVertex[] _currentArray = new DxVertex[_pointCount*6];
		private ThreadLocal<Random> random = new ThreadLocal<Random>(() => new Random());
		public volatile bool _hasBeenDisposed = false;
		public volatile bool _animationRunning = false;
		public volatile bool _doReset = true;

		public DxRectangleTexturedSampleViewModel()
		{
			StartArrayUpdate();
		}

		private void DoReset()
		{
			var rnd = random.Value;
			HSBPalette palette = new HSBPalette();
			// Create a new array of points
			for (int i = 0; i < _pointCount; i++)
			{
				var point = new Point(rnd.NextDouble(), rnd.NextDouble());
				_points[i] = point;

				var vertices = SharpDXHelper.MakeRectangle((float)point.X, (float)point.Y, 0.05f, 0.05f);
				for (var j=0;j<vertices.Count;j++)
                {
					_currentArray[i*6+j] = vertices[j];
				}
			}
			// Replace the old array of points with the new array
			Points = _points;
			var temp = (Positions as DxVertex[]) ?? new DxVertex[_pointCount*6];
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
						// Update the position of every point according to its direction
						for (int i = 0; i < Points.Count(); i++)
						{
							var dxdy = new Vector(directions[i].X * Width1Px, directions[i].Y * Height1Px);
							var newX = dxdy.X + _points[i].X;
							var newY = dxdy.Y + _points[i].Y;

							// Point has hit an east or west wall. Reverse direction vector X component
							if (newX < 0 || newX > 1)
							{
								newX = Math.Min(1, Math.Max(0, newX));
								directions[i] = new Vector(-directions[i].X, directions[i].Y);
							}
							// Point has hit a north or south wall. Reverse direction vector Y component
							if (newY < 0 || newY > 1)
							{
								newY = Math.Min(1, Math.Max(0, newY));
								directions[i] = new Vector(directions[i].X, -directions[i].Y);
							}

							_points[i] = new Point(newX, newY);
							var rectangle = SharpDXHelper.MakeRectangle((float)newX, (float)newY, 0.05f, 0.05f);
							for (var j=0;j<6;j++)
                            {
								_currentArray[i*6 + j] = rectangle[j];
							}
						}

						Points = _points;

						var temp = (Positions as DxVertex[]) ?? new DxVertex[_pointCount*6];
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

		public int PointCount { get; set; }

		public IEnumerable<DxVertex> Positions { get; set; }

		public IEnumerable<Point> Points { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		private RelayCommandFactoryD3 _stopAnimaion = new RelayCommandFactoryD3();
		public ICommand StopAnimation => _stopAnimaion.GetCommand(() => _animationRunning = false);

		private RelayCommandFactoryD3 _startAnimation = new RelayCommandFactoryD3();
		public ICommand StartAnimation => _startAnimation.GetCommand(() => _animationRunning = true);

		private RelayCommandFactoryD3 _resetCircles = new RelayCommandFactoryD3();
		public ICommand ResetCircles => _resetCircles.GetCommand(() => _doReset = true);
	}
}
