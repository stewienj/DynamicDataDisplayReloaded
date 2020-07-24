using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DynamicDataDisplay.BitmapGraphs
{
	public class StatusByLatLonChartView : BitmapBasedGraph
	{
		/// <summary>Currently running request</summary>
		private RenderRequest _activeRequest = null;
		private bool _shutdownStarted = false;
		private IList<uint> _source = null;
		private int _renderMinX = -180;
		private int _renderMinY = -90;
		private int _renderWidth = 360;
		private int _renderHeight = 180;
		private int _sourceWidth = 720;
		private int _sourceHeight = 360;
		private ReaderWriterLockSlim _sourceLock = new ReaderWriterLockSlim();

		public StatusByLatLonChartView()
		{
			SourceSize = new Size(_sourceWidth, _sourceHeight);
			Dispatcher.ShutdownStarted += (s, e) =>
			{
				_shutdownStarted = true;
				_activeRequest?.Cancel();
			};
		}

		public void CreateTestBitmap()
		{
			var pixels = new uint[_sourceWidth * _sourceHeight];
			uint alpha = 0x77000000;
			int colorIndex = 0;
			int pixelsIndex = 0;
			for (int y = 0; y < _sourceHeight; ++y)
			{
				colorIndex = y;
				for (int x = 0; x < _sourceWidth; ++x)
				{
					colorIndex = (colorIndex + 1) % 3;
					pixels[pixelsIndex] = alpha | (uint)(255 << (colorIndex * 8));
					pixelsIndex++;
				}
			}
			_source = pixels;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data">The data rectangle to be rendered</param>
		/// <param name="output">The display rectangle relative to the whole desktop area, so only width and height are relevant</param>
		/// <param name="renderRequest">Book keeping for thread tracking</param>
		/// <returns></returns>
		protected override BitmapSource RenderFrame(DataRect data, Rect output, RenderRequest renderRequest)
		{
			var transform = Plotter2D?.Viewport?.Transform?.DataTransform;

			// Return null if no update required
			if (_shutdownStarted || data.Width == 0 || data.Height == 0 || _source == null)
			{
				return null;
			}

			_activeRequest = renderRequest;
			try
			{
				_sourceLock.EnterReadLock();

				// Check the nominated source size doesn't exceed the source data size
				if (_sourceWidth * _sourceHeight > _source.Count)
				{
					return null;
				}

				double dataToSourceWidth = (double)_sourceWidth / (double)_renderWidth;
				double dataToSourceHeight = (double)_sourceHeight / (double)_renderHeight;

				int outputWidth = (int)Math.Round(output.Width);
				int outputHeight = (int)Math.Round(output.Height);
				var newPixels = new uint[outputWidth * outputHeight];

				// Multithread the rendering
				var yRange = Enumerable.Range(0, outputHeight);

				Batch(yRange, 64).AsParallel().ForAll(yBatch =>
				{
					foreach (int y in yBatch)
					{
						double fractionOfRectY = (double)y / outputHeight;
						double positionY = fractionOfRectY * data.Height + data.YMin;
						if (transform != null)
						{
							if (positionY < transform.MaxLatitude && (-positionY) < transform.MaxLatitude)
							{
								positionY = transform.ViewportToData(new Point(0, positionY)).Y;
							}
							else
							{
								continue;
							}
						}
						// The Y source coordinate will be ...
						int sourceIndexY = (int)Math.Floor((positionY - _renderMinY) * dataToSourceHeight);
						double pixelWidth = 1.0 / outputWidth;
						if (sourceIndexY >= 0 && sourceIndexY < _sourceHeight)
						{
							int sourceIndexYOffset = _sourceWidth * sourceIndexY;
							int destIndex = (outputHeight - 1 - y) * outputWidth;
							for (int x = 0; x < outputWidth; ++x)
							{
								// Optimise the divide by precalculating the reciprocal
								double fractionOfRectX = x * pixelWidth;
								double positionX = fractionOfRectX * data.Width + data.XMin;
								// The X source coordinate will be ...
								int sourceIndexX = (int)Math.Floor((positionX - _renderMinX) * dataToSourceWidth);
								if (sourceIndexX >= 0 && sourceIndexX < _sourceWidth)
								{
									newPixels[destIndex] = _source[sourceIndexX + sourceIndexYOffset];
								}
								++destIndex;
							}
						}
					}
				});
				var retVal = BitmapSource.Create(outputWidth, outputHeight, 96.0, 96.0, PixelFormats.Bgra32, null, newPixels, outputWidth * 4);
				retVal.Freeze();
				return retVal;
			}
			catch (Exception)
			{
				return null;
			}
			finally
			{
				_sourceLock.ExitReadLock();
				_activeRequest = null;
			}
		}
		public static IEnumerable<List<T>> Batch<T>(IEnumerable<T> source, int batchSize)
		{
			if (batchSize == 0)
			{
				yield break;
			}
			List<T> retVal = new List<T>();
			int count = 0;
			foreach (var item in source)
			{
				if (count >= batchSize)
				{
					yield return retVal;
					count = 0;
					retVal = new List<T>();
				}
				retVal.Add(item);
				count++;
			}
			yield return retVal;
		}



		public Size SourceSize
		{
			get { return (Size)GetValue(SourceSizeProperty); }
			set { SetValue(SourceSizeProperty, value); }
		}

		// Using a DependencyProperty as the backing store for SourceSize.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SourceSizeProperty =
			DependencyProperty.Register("SourceSize", typeof(Size), typeof(StatusByLatLonChartView), new PropertyMetadata(new Size(360, 180), (s, e) =>
			 {
				if (s is StatusByLatLonChartView view)
				{
					if (e.NewValue is Size newSize)
					{
						view._sourceLock.EnterWriteLock();
						view._sourceWidth = (int)newSize.Width;
						view._sourceHeight = (int)newSize.Height;
						view._sourceLock.ExitWriteLock();
					}
					view.UpdateVisualization();
				}
			}));


		public IList<uint> PixelSource
		{
			get { return (IList<uint>)GetValue(PixelSourceProperty); }
			set { SetValue(PixelSourceProperty, value); }
		}


		// Using a DependencyProperty as the backing store for PixelSource.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty PixelSourceProperty =
			DependencyProperty.Register("PixelSource", typeof(IList<uint>), typeof(StatusByLatLonChartView), new PropertyMetadata(null, (s, e) =>
			{
				if (s is StatusByLatLonChartView view)
				{
					if (e.OldValue is INotifyPropertyChanged oldNotifyProperty)
					{
						oldNotifyProperty.PropertyChanged -= view.NotifyProperty_PropertyChanged;
					}
					if (e.OldValue is INotifyCollectionChanged oldCollectionChanged)
					{
						oldCollectionChanged.CollectionChanged -= view.CollectionChanged_CollectionChanged;
					}
					if (e.NewValue is IList<uint> newList)
					{
						if (e.NewValue is INotifyPropertyChanged newNotifyProperty)
						{
							newNotifyProperty.PropertyChanged += view.NotifyProperty_PropertyChanged;
						}
						if (e.NewValue is INotifyCollectionChanged newCollectionChanged)
						{
							newCollectionChanged.CollectionChanged += view.CollectionChanged_CollectionChanged;
						}
						view._sourceLock.EnterWriteLock();
						view._source = newList;
						view._sourceLock.ExitWriteLock();
					}
					view.UpdateVisualization();
				}
			}));

		private void CollectionChanged_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			UpdateVisualization();
		}

		private void NotifyProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			UpdateVisualization();
		}

	}
}
