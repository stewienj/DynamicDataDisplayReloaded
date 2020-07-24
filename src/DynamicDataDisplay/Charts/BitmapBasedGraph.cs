using DynamicDataDisplay.Charts;
using DynamicDataDisplay.Common.Auxiliary;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DynamicDataDisplay
{
	public abstract class BitmapBasedGraph : ViewportElement2D
	{
		static BitmapBasedGraph()
		{
			Type thisType = typeof(BitmapBasedGraph);
			BackgroundRenderer.UsesBackgroundRenderingProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(true));
		}

		private int _nextRequestId = 0;

		/// <summary>Latest complete request</summary>
		private RenderRequest _completedRequest = null;

		/// <summary>Currently running request</summary>
		private RenderRequest _activeRequest = null;

		/// <summary>Result of latest complete request</summary>
		private BitmapSource _completedBitmap = null;

		/// <summary>Pending render request</summary>
		private RenderRequest _pendingRequest = null;

		/// <summary>True means that current bitmap is invalidated and is to be re-rendered.</summary>
		private bool _bitmapInvalidated = true;

		/// <summary>Shows tooltips.</summary>
		private PopupTip _popup;

		// Throttle to 60 requests per second
		private ThrottledAction _renderAction = new ThrottledAction(TimeSpan.FromMilliseconds(1000.0 / 60.0));

		/// <summary>
		/// Initializes a new instance of the <see cref="MarkerPointsGraph"/> class.
		/// </summary>
		public BitmapBasedGraph()
		{
			ManualTranslate = true;
		}

		protected virtual UIElement GetTooltipForPoint(Point point, DataRect visible, Rect output)
		{
			return null;
		}

		protected PopupTip GetPopupTipWindow()
		{
			if (_popup != null)
				return _popup;

			foreach (var item in Plotter.Children)
			{
				if (item is ViewportUIContainer)
				{
					ViewportUIContainer container = (ViewportUIContainer)item;
					if (container.Content is PopupTip)
						return _popup = (PopupTip)container.Content;
				}
			}

			_popup = new PopupTip();
			_popup.Placement = PlacementMode.Relative;
			_popup.PlacementTarget = this;
			Plotter.Children.Add(_popup);
			return _popup;
		}

		protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
		{
			base.OnMouseMove(e);

			var popup = GetPopupTipWindow();
			if (popup.IsOpen)
				popup.Hide();

			if (_bitmapInvalidated) return;

			Point p = e.GetPosition(this);
			Point dp = p.ScreenToData(Plotter2D.Transform);

			var tooltip = GetTooltipForPoint(p, _completedRequest.Visible, _completedRequest.Output);
			if (tooltip == null) return;

			popup.VerticalOffset = p.Y + 20;
			popup.HorizontalOffset = p.X;

			popup.ShowDelayed(new TimeSpan(0, 0, 1));

			Grid grid = new Grid();

			Rectangle rect = new Rectangle
			{
				Stroke = Brushes.Black,
				Fill = SystemColors.InfoBrush
			};

			StackPanel sp = new StackPanel();
			sp.Orientation = Orientation.Vertical;
			sp.Children.Add(tooltip);
			sp.Margin = new Thickness(4, 2, 4, 2);

			var tb = new TextBlock();
			tb.Text = string.Format("Location: {0:F2}, {1:F2}", dp.X, dp.Y);
			tb.Foreground = SystemColors.GrayTextBrush;
			sp.Children.Add(tb);

			grid.Children.Add(rect);
			grid.Children.Add(sp);
			grid.Measure(SizeHelper.CreateInfiniteSize());
			popup.Child = grid;
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);
			GetPopupTipWindow().Hide();
		}

		/// <summary>
		/// Adds a render task and invalidates visual.
		/// </summary>
		public void UpdateVisualization()
		{
			if (Viewport == null) return;

			Rect output = new Rect(RenderSize);
			CreateRenderTask(Viewport.Visible, output);
			InvalidateVisual();
		}

		protected override void OnVisibleChanged(DataRect newRect, DataRect oldRect)
		{
			base.OnVisibleChanged(newRect, oldRect);
			CreateRenderTask(newRect, Viewport.Output);
			InvalidateVisual();
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);
			CreateRenderTask(Viewport?.Visible ?? new DataRect(0,0,1,1), new Rect(sizeInfo.NewSize));
			InvalidateVisual();
		}


		protected override void OnPlotterDetaching(Plotter plotter)
		{
			base.OnPlotterDetaching(plotter);
			_activeRequest?.Cancel();
		}

		protected abstract BitmapSource RenderFrame(DataRect visible, Rect output, RenderRequest renderRequest);

		private void RenderThreadFunc()
		{
			lock (this)
			{
				_activeRequest = null;
				if (_pendingRequest != null)
				{
					_activeRequest = _pendingRequest;
					_pendingRequest = null;
				}
			}
			if (_activeRequest != null)
			{
				try
				{
					// Return null if no update required
					BitmapSource result = Dispatcher.HasShutdownStarted ? null : (BitmapSource)RenderFrame(_activeRequest.Visible, _activeRequest.Output, _activeRequest);
					if (result != null)
						Dispatcher.BeginInvoke(
							new RenderCompletionHandler(OnRenderCompleted),
							new RenderResult(_activeRequest, result));
				}
				catch (Exception exc)
				{
					Trace.WriteLine(string.Format("RenderRequest {0} failed: {1}", _activeRequest.RequestID, exc.Message));
				}
			}
		}

		private void CreateRenderTask(DataRect visible, Rect output)
		{
			lock (this)
			{
				_bitmapInvalidated = true;

				if (_activeRequest != null)
					_activeRequest.Cancel();
				_pendingRequest = new RenderRequest(_nextRequestId++, visible, output);
				_renderAction.InvokeAction(RenderThreadFunc);
			}
		}

		private delegate void RenderCompletionHandler(RenderResult result);

		protected virtual void OnRenderCompleted(RenderResult result)
		{
			if (result.IsSuccess)
			{
				_completedRequest = result.Request;
				_completedBitmap = result.Bitmap;
				_bitmapInvalidated = false;

				InvalidateVisual();
				BackgroundRenderer.RaiseRenderingFinished(this);
			}
		}

		protected override void OnRenderCore(DrawingContext dc, RenderState state)
		{
			if (_completedRequest != null && _completedBitmap != null)
				dc.DrawImage(_completedBitmap, _completedRequest.Visible.ViewportToScreen(Viewport.Transform));
		}
	}

	public class RenderRequest
	{
		private int _requestId;
		private DataRect _visible;
		private Rect _output;
		private int _cancelling;

		public RenderRequest(int requestId, DataRect visible, Rect output)
		{
			_requestId = requestId;
			_visible = visible;
			_output = output;
		}

		public int RequestID => _requestId;

		public DataRect Visible => _visible;

		public Rect Output => _output;

		public bool IsCancellingRequested => _cancelling > 0;

		public void Cancel() => Interlocked.Increment(ref _cancelling);
	}

	public class RenderResult
	{
		private RenderRequest _request;
		private BitmapSource _source;

		/// <summary>Constructs successul rendering result</summary>
		/// <param name="request">Source request</param>
		/// <param name="result">Rendered bitmap</param>
		public RenderResult(RenderRequest request, BitmapSource result)
		{
			_request = request;
			_source = result;
		}

		public RenderRequest Request => _request;

		public bool IsSuccess => _source != null;

		public BitmapSource Bitmap => _source;
	}
}
