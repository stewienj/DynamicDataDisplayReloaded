using DynamicDataDisplay.Common.Auxiliary;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using Device = SharpDX.Direct3D11.Device;
using Resource = SharpDX.Direct3D11.Resource;

namespace DynamicDataDisplay.SharpDX11
{
	public class SharpDXHost : FrameworkElement, IPlotterElement
	{
		private Texture2D _backBuffer;
		private RenderTargetView _renderTargetView;
		private Device _device;
		private DeviceContext _deviceContext;
		private SwapChain _swapChain;

		private D3DImage _image = new D3DImage();
		private bool _sizeChanged;

		private ThrottledAction _resizeAction = new ThrottledAction(TimeSpan.FromMilliseconds(1000));

		public Device Device => _device;

		public DeviceContext DeviceContext => _deviceContext;

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			_resizeAction.InvokeAction(() => _sizeChanged = true);
			base.OnRenderSizeChanged(sizeInfo);
		}

		public bool LockImage()
		{
			return _image.TryLock(_timeOutDuration);
		}

		public void UnlockImage()
		{
			_image.Unlock();
		}

		public void AddChild(object child) => AddLogicalChild(child);

		public void RemoveChild(object child) => RemoveLogicalChild(child);

		private void Initialize3D()
		{
			HwndSource hwnd = new HwndSource(0, 0, 0, 0, 0, "D3", IntPtr.Zero);

			SwapChainDescription swapChainDescription = new SwapChainDescription
			{

				SwapEffect = SwapEffect.Discard,
				OutputHandle = hwnd.Handle,
				IsWindowed = true,
				Flags = SwapChainFlags.None,
				BufferCount = 1,
				Usage = Usage.RenderTargetOutput | Usage.Shared,
				ModeDescription = new ModeDescription(Math.Max(100, (int)ActualWidth), Math.Max(100, (int)ActualHeight), new Rational(60, 1), Format.B8G8R8A8_UNorm),
				SampleDescription = new SampleDescription(1, 0)
			};

			try
			{
				SharpDX.Direct3D11.Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.BgraSupport, swapChainDescription, out _device, out _swapChain);

				// Ignore all windows events
				using (var factory = _swapChain.GetParent<Factory>())
				{
					factory.MakeWindowAssociation(hwnd.Handle, WindowAssociationFlags.IgnoreAll);
				}

				// New RenderTargetView from the backbuffer
				_backBuffer = Resource.FromSwapChain<Texture2D>(_swapChain, 0);
				_renderTargetView = new RenderTargetView(_device, _backBuffer);
				_deviceContext = _device.ImmediateContext;
			}
			catch
			{
				// At this point we are pretty much screwed, require DeviceEx for the reset capability
				MessageBox.Show("Can't start DirectX, so that pretty much discounts doing anything else");
			}

			//System.Windows.Media.CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
			CompositionTargetRenderingEventManager.AddHandler(CompositionTarget_Rendering);
		}

		bool renderNext = true;
        // Note that if you exceed 2 seconds per frame render, then it will stop updating.
		Duration _timeOutDuration = new Duration(TimeSpan.FromMilliseconds(2000));
		private void CompositionTarget_Rendering(object sender, EventArgs e)
		{
			renderNext = !renderNext;
			if (renderNext)
			{
				return;
			}
			if (_image == null) return;

			try
			{
				// Lock the image while we are resetting as the rendering system freezes when we get a clash
				if (_sizeChanged && _image.TryLock(_timeOutDuration))
				{
					try
					{
						var modeDescription = new ModeDescription(Math.Max(100, (int)ActualWidth), Math.Max(100, (int)ActualHeight), new Rational(60, 1), Format.B8G8R8A8_UNorm);
						_swapChain.ResizeTarget(ref modeDescription);
						_sizeChanged = false;
					}
					finally
					{
						_image.Unlock();
					}
					return;
				}

				if (_image.IsFrontBufferAvailable)
				{
					if (!_image.TryLock(_timeOutDuration))
					{
						return;
					}
					Result result = _swapChain.Present(0, PresentFlags.Test);// Device.TestCooperativeLevel();
					if (result.Failure)
					{
						throw new SharpDXException();
					}
					try
					{
						/*
						Device.SetRenderState(global::SharpDX.Direct3D11.RenderState.CullMode, Cull.None);
						Device.SetRenderState(global::SharpDX.Direct3D9.RenderState.ZEnable, true);
						Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Transparent, float.MaxValue, 0);
						Device.BeginScene();
						Render.Raise(this, new RenderEventArgs(_swapChain.get, _pp.BackBufferHeight));
						Device.EndScene();
						Device.Present();
						_swapChain.Present()
						*/

						_image.SetBackBuffer(D3DResourceType.IDirect3DSurface9, _backBuffer.NativePointer);
						_image.AddDirtyRect(new Int32Rect(0, 0, _image.PixelWidth, _image.PixelHeight));
					}
					catch (Exception exc)
					{
						Debug.WriteLine("Error in rendering in DirectXHost: " + exc.Message);
					}
					finally
					{
						_image.Unlock();
					}
				}
			}
			catch (SharpDXException exc)
			{
				Debug.WriteLine("Exception in main render loop: " + exc.Message);
			}
		}

		public event EventHandler<RenderEventArgs> Render;

		protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
		{
			drawingContext.DrawImage(_image, new Rect(RenderSize));
		}

		#region IPlotterElement Members

		private Plotter2D _plotter;
		public void OnPlotterAttached(Plotter plotter)
		{
			_plotter = (Plotter2D)plotter;
			_plotter.CentralGrid.Children.Add(this);
			_plotter.Viewport.PropertyChanged += Viewport_PropertyChanged;
			Initialize3D();
		}

		private void Viewport_PropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Visible")
			{
				if (e.NewValue is DataRect newRect)
				{
					// Change the matrix
				}
			}
		}

		public void OnPlotterDetaching(Plotter plotter)
		{
			_plotter.Viewport.PropertyChanged -= Viewport_PropertyChanged;
			_plotter.CentralGrid.Children.Remove(this);
			_plotter = null;
			_deviceContext.Dispose();
			_device.Dispose();
		}

		public Plotter Plotter => _plotter;

		#endregion

	}

	public class RenderEventArgs : EventArgs
	{
		public RenderEventArgs(int width, int height)
		{
			Width = width;
			Height = height;
		}

		public int Width { get; }
		public int Height { get; }
	}
		

}
