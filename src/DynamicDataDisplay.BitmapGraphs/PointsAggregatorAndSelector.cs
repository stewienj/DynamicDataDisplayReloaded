using DynamicDataDisplay.Common.Auxiliary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DynamicDataDisplay.BitmapGraphs
{
	public class PointsAggregatorAndSelector
	{
		private ThrottledAction _pointsUpdate = new ThrottledAction(TimeSpan.FromMilliseconds(10));
		private IEnumerable _lastPointsSource;
		private Func<object, Point> _pointObjectToPointConverter;
		private CancellationTokenSource _mouseClickCancellationSource = null;
		private ReaderWriterLockSlim _mouseClickLock = new ReaderWriterLockSlim();
		private UIElementSingleClickDoubleClickRouter _mouseHandler;
		private UIElement _positionElement;

		public PointsAggregatorAndSelector(UIElement positionElement)
		{
			_positionElement = positionElement;
		}

		private Plotter2D _plotter = null;
		public Plotter2D Plotter
		{
			get => _plotter;
			set
			{
				if (_plotter?.Viewport != null)
				{
					_plotter.Viewport.PropertyChanged -= Viewport_PropertyChanged;
				}
				_plotter = value;
				HookMouse(_plotter);
				if (_plotter?.Viewport != null)
				{
					_plotter.Viewport.PropertyChanged += Viewport_PropertyChanged;
				}
			}
		}

		private void HookMouse(UIElement element)
		{
			if (_mouseHandler != null)
			{
				UnhookMouse();
			}
			if (element != null)
			{
				// Handle Mouse Clicks
				_mouseHandler = new UIElementSingleClickDoubleClickRouter(element, _positionElement);
				_mouseHandler.MouseLeftSingleClick += Mouse_LeftSingleClick;
				_mouseHandler.MouseCancelLeftSingleClick += Mouse_CancelLeftSingleClick;
			}
		}

		private void UnhookMouse()
		{
			if (_mouseHandler != null)
			{
				_mouseHandler.MouseLeftSingleClick -= Mouse_LeftSingleClick;
				_mouseHandler.MouseCancelLeftSingleClick -= Mouse_CancelLeftSingleClick;
				_mouseHandler.Dispose();
				_mouseHandler = null;
			}
		}

		private void Viewport_PropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Viewport2D.Transform) || e.PropertyName == nameof(Viewport2D.Visible))
			{
				Recalculate();
			}
		}

		public void Recalculate()
		{
			SetPointsSource(_lastPointsSource);
		}

		public void SetPointObjectToPointConverter(Func<object, Point> pointObjectToPointConverter)
		{
			_pointObjectToPointConverter = pointObjectToPointConverter;
			SetPointsSource(_lastPointsSource);
		}

		public void SetPointsSource(IEnumerable pointsSource)
		{
			_lastPointsSource = pointsSource;
			var transform = Plotter?.Transform;
			var dataTransform = transform?.DataTransform;

			_pointsUpdate.InvokeAction(() =>
			{
				// Do a sanity check and retun a nothing result if anything is invalid
				if (dataTransform == null || pointsSource == null || NewPointsReady == null || _pointObjectToPointConverter == null)
				{
					// Invoke the data update event
					NewPointsReady?.Invoke(this,
					  new AggregatedPointsChangedArgs
					  {
						  ViewportAggregatedPoints = new List<(Point, int, Rect)>()
					  });
					return;
				}

				// Transforms points to Binned Buckets
				var ToBinnedBucketAndPoint = CalculateToBinnedBucketAndPoint(transform);

				var BinnedBucketToLocation = CalculateBinnedBucketToLocation(transform);
				var BinnedBucketToRect = CalculateBinnedBucketToRect(transform);

				// Transform points to DataRect
				// Fire event with all the new data

				var pointsViewport = pointsSource
				  .Cast<object>()
				  .AsParallel()
				  .Select(ll => dataTransform.DataToViewport(_pointObjectToPointConverter(ll)));

				// Scale the points to integers, group, the rescale back to near original value
				var scaledPoints = pointsViewport
				  // Create scaled/original pairs
				  .Select(ToBinnedBucketAndPoint)
				  // Convert to dictionary indexed by scaled pairs
				  .ToLookup(xyp => xyp.BinnedBucket, xy => xy.Point.ToVector())
				  // Aggregate the original pairs into averages
				  .Select(g => (Point: (g.Aggregate((xy1, xy2) => xy1 + xy2) / g.Count()).ToPoint(), Count: g.Count(), Bin: BinnedBucketToRect(g.Key)))
				  // This just uses the center of the rectangle
				  //.Select(g => (Point: BinnedBucketToLocation(g.Key), Count: g.Count()))
				  // Convert to list
				  .ToList();

				// Invoke the data update event
				NewPointsReady?.Invoke(this,
				  new AggregatedPointsChangedArgs
				  {
					  ViewportAggregatedPoints = scaledPoints
				  });
			});
		}

		public virtual async void Mouse_LeftSingleClick(object sender, Point clickPoint)
		{
			// Here's a whole bunch of syncronisation stuff to cancel any previous
			// calculations, get local copies of class variables in case they change
			CancelMouseClick(true);

			var pointsLatLonDeg = _lastPointsSource;
			var transform = Plotter?.Transform;
			var dataTransform = transform?.DataTransform;

			var screenRect = transform.ScreenRect;

			if (pointsLatLonDeg == null || dataTransform == null || _pointObjectToPointConverter == null)
			{
				return;
			}

			try
			{
				var token = _mouseClickCancellationSource.Token;
				(var selectedPoints, var selectedLocation) = await Task.Run(() =>
				{
					try
					{
						// Create a selection rectangle
						var offset = new Vector(5, 5);
						var minClickPoint = clickPoint - offset;
						var maxClickPoint = clickPoint + offset;
						var minViewportClickLocation = transform.ScreenToViewport(minClickPoint);
						var maxViewportClickLocation = transform.ScreenToViewport(maxClickPoint);
						var clickRect = new Rect(minViewportClickLocation, maxViewportClickLocation);

						// Find all the points in the above rectangle
						var points =
							pointsLatLonDeg
							.Cast<object>()
							.AsParallel()
							.Where(ll => clickRect.Contains(dataTransform.DataToViewport(_pointObjectToPointConverter(ll))))
							.ToList();

						_mouseClickLock.EnterWriteLock();
						if (!token.IsCancellationRequested && points.Any())
						{
							var pointsLocation =
							points
							.Select(p => _pointObjectToPointConverter(p).ToVector())
							.Aggregate((a, b) => a + b)
							/ points.Count;

							// Type cast a null because type inference failed if I don't
							return (points, pointsLocation.ToPoint());
						}
						else
						{
							// Type cast a null because type inference failed if I don't
							return (null as List<object>, new Point(0, 0));
						}
					}
					finally
					{
						_mouseClickLock.ExitWriteLock();
					}
				}, token);

				SelectedPointsChanged?.Invoke(this, new SelectedPointsChangedArgs { SelectedPoints = selectedPoints, SelectionLocation = selectedLocation, Converter = _pointObjectToPointConverter });
				// The task cancellation exception seems to get rethrown on the dispatcher
			}
			catch (Exception) { }
		}

		private void CancelMouseClick(bool renewToken)
		{
			_mouseClickLock.EnterWriteLock();
			_mouseClickCancellationSource?.Cancel();
			_mouseClickCancellationSource?.Dispose();
			if (!renewToken)
			{
				SelectedPointsChanged?.Invoke(this, new SelectedPointsChangedArgs { SelectedPoints = null });
			}
			_mouseClickLock.ExitWriteLock();
			_mouseClickCancellationSource = renewToken ? new CancellationTokenSource() : null;

		}

		public void Mouse_CancelLeftSingleClick(object sender, Point clickPoint)
		{
			CancelMouseClick(false);
		}

		public BitmapSource RenderAggregationGrid(DataRect data, Rect output, BitmapDebugType debugType)
		{
			double offsetMultiplier = 0.0;
			switch (debugType)
			{
				case BitmapDebugType.GridCentrePoints:
					offsetMultiplier = 0.0;
					break;
				case BitmapDebugType.GridBoxes:
					offsetMultiplier = -0.5;
					break;
			}

			// Get the scalers from the plotter transform

			var transform = Plotter?.Transform;
			if (transform == null)
				return null;
			(var scalerX, var scalerY) = CalculateScalers(transform);

			int height = (int)output.Height;
			int width = (int)output.Width;

			uint[,] buffer = new uint[height, width];

			uint red = 0xFFFF0000;

			// Get the edge of the datarect, divide by the scaler, this will give us the centre point of the aggregation box
			// then add multiples of the scaler to get each box centre.

			var startX = (Math.Round(data.XMin / scalerX) + offsetMultiplier) * scalerX;

			for (double x = startX; x < data.XMax; x += scalerX)
			{
				int xCoord = (int)((x - data.XMin) / data.Width * output.Width);
				if (xCoord >= 0 && xCoord < width)
				{
					for (int yCoord = 0; yCoord < height; ++yCoord)
					{
						buffer[yCoord, xCoord] = red;
					}
				}
			}

			var startY = (Math.Round(data.YMin / scalerY) + offsetMultiplier) * scalerY;

			for (double y = startY; y <= data.YMax; y += scalerY)
			{
				int yCoord = (int)((data.YMax - y) / data.Height * output.Height);
				if (yCoord >= 0 && yCoord < height)
				{
					for (int xCoord = 0; xCoord < width; ++xCoord)
					{
						buffer[yCoord, xCoord] = red;
					}
				}
			}

			var gridImage = new ArrayBitmapSource<uint>(buffer);
			gridImage.Freeze();

			return gridImage;
		}


		private Func<Point, ((int, int) BinnedBucket, Point Point)> CalculateToBinnedBucketAndPoint(CoordinateTransform transform)
		{
			(var scalerX, var scalerY) = CalculateScalers(transform);
			var scalerDivX = 1 / scalerX;
			var scalerDivY = 1 / scalerY;

			// Transform points to DataRect
			Func<Point, ((int, int), Point)> ToBucketAndPoint = p => (((int)Math.Round(p.X * scalerDivX), (int)Math.Round(p.Y * scalerDivY)), p);

			return ToBucketAndPoint;
		}

		private Func<Point, (int, int)> CalculateToBinnedBucket(CoordinateTransform transform)
		{
			(var scalerX, var scalerY) = CalculateScalers(transform);
			var scalerDivX = 1 / scalerX;
			var scalerDivY = 1 / scalerY;

			// Transform points to DataRect
			Func<Point, (int, int)> ToBucket = p => ((int)Math.Round(p.X * scalerDivX), (int)Math.Round(p.Y * scalerDivY));

			return ToBucket;
		}

		private Func<(int, int), Point> CalculateBinnedBucketToLocation(CoordinateTransform transform)
		{
			(var scalerX, var scalerY) = CalculateScalers(transform);
			Func<(int X, int Y), Point> ToLocation = p => new Point(p.X * scalerX, p.Y * scalerY);

			return ToLocation;
		}

		private Func<(int, int), Rect> CalculateBinnedBucketToRect(CoordinateTransform transform)
		{
			(var scalerX, var scalerY) = CalculateScalers(transform);
			Func<(int X, int Y), Rect> ToRect = p => new Rect((p.X - 0.5) * scalerX, (p.Y - 0.5) * scalerY, scalerX, scalerY);
			return ToRect;
		}

		private (double ScalerX, double ScalerY) CalculateScalers(CoordinateTransform transform)
		{
			var screenRect = transform.ScreenRect;
			var viewportRect = transform.ViewportRect;

			// Need to adjust lats and longs separately.

			// So lets pick:
			// 1 adjusted lat = 25 pixels
			// 1 adjusted lon = 75 pixels
			// Work out how many parts required by dividing screenRect by 25 = p
			// divide lat/p = divisor for lat. DONT - Round down to power of 2 - log base 2, round, if round is higher than original value subtract 1

			// Do the horizontal/Width/Longitude first

			//double powerNumber = 2; Commented out as it seems to work better witout this

			var minPixelWidth = Math.Max(AggregatedBlockPixelWidth, 1.0);// 40.0 by default in GroupesMarkersChartView;
			var maxHorizontalDivides = screenRect.Width / minPixelWidth;
			var minLongitudesPerDivide = viewportRect.Width / maxHorizontalDivides;
			var scalerX = minLongitudesPerDivide;
			//scalerX = Math.Pow(powerNumber, Math.Ceiling(Math.Log(scalerX, powerNumber))); Commented out as it seems to work better witout this

			var minPixelHeight = Math.Max(AggregatedBlockPixelHeight, 1.0);// 25.0 by default in GroupesMarkersChartView;
			var maxVerticalDivides = screenRect.Height / minPixelHeight;
			var minLatitudesPerDivide = viewportRect.Height / maxVerticalDivides;
			var scalerY = minLatitudesPerDivide;
			//scalerY = Math.Pow(powerNumber, Math.Ceiling(Math.Log(scalerY, powerNumber))); Commented out as it seems to work better witout this

			return (scalerX, scalerY);
		}

		public double AggregatedBlockPixelWidth { get; set; } = 1.0;
		public double AggregatedBlockPixelHeight { get; set; } = 1.0;

		public event EventHandler<AggregatedPointsChangedArgs> NewPointsReady;
		public event EventHandler<SelectedPointsChangedArgs> SelectedPointsChanged;
	}

	public class AggregatedPointsChangedArgs : EventArgs
	{
		/// <summary>
		/// A list of points that have been transformed
		/// </summary>
		public List<(Point Point, int Count, Rect Bin)> ViewportAggregatedPoints { get; set; }
	}

	public class SelectedPointsChangedArgs : EventArgs
	{
		/// <summary>
		/// A list of points that have been transformed
		/// </summary>
		public List<object> SelectedPoints { get; set; }

		public Point SelectionLocation { get; set; }
		public Func<object, Point> Converter { get; internal set; }

		public IEnumerable<Point> ConvertedSelectedPoints =>
		  (
			Converter != null
			  ? SelectedPoints?.Select(p => Converter(p))
			  : null
		  )
		  ?? Enumerable.Empty<Point>();
	}
}
