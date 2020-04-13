using DynamicDataDisplay.Common.Auxiliary;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Interop;

namespace DynamicDataDisplay.SharpDX9
{
	public class SharpDXHost : FrameworkElement, IPlotterElement
	{
		private D3DImage _image = new D3DImage();
		private bool _sizeChanged;
		private PresentParameters _pp = new PresentParameters();
		private ThrottledAction _resizeAction = new ThrottledAction(TimeSpan.FromMilliseconds(1000));

		public Device Device { get; private set; }

		public Direct3D Direct3D { get; private set; }

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			_resizeAction.InvokeAction(() => _sizeChanged = true);
			base.OnRenderSizeChanged(sizeInfo);
		}

		public void AddChild(object child) => AddLogicalChild(child);

		public void RemoveChild(object child) => RemoveLogicalChild(child);

		private void Initialize3D()
		{
			HwndSource hwnd = new HwndSource(0, 0, 0, 0, 0, "D3", IntPtr.Zero);

			_pp.SwapEffect = SwapEffect.Discard;
			_pp.DeviceWindowHandle = hwnd.Handle;
			_pp.Windowed = true;
			_pp.EnableAutoDepthStencil = true;
			_pp.BackBufferWidth = Math.Max(100, (int)ActualWidth);
			_pp.BackBufferHeight = Math.Max(100, (int)ActualHeight);
			_pp.BackBufferFormat = Format.A8R8G8B8;
			_pp.AutoDepthStencilFormat = Format.D32SingleLockable;
			_pp.BackBufferCount = 1;
			try
			{
				var direct3DEx = new Direct3DEx();
				Direct3D = direct3DEx;
				Device = new DeviceEx(direct3DEx, 0, DeviceType.Hardware, hwnd.Handle, CreateFlags.HardwareVertexProcessing, _pp);
			}
			catch
			{
				// At this point we are pretty much screwed, require DeviceEx for the reset capability
				MessageBox.Show("Can't start DirectX, so that pretty much discounts doing anything else");
			}
			System.Windows.Media.CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
		}

		bool renderNext = true;
		Duration _timeOutDuration = new Duration(TimeSpan.FromMilliseconds(200));
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
						HwndSource hwnd = new HwndSource(0, 0, 0, 0, 0, "D3", IntPtr.Zero);
						_pp.DeviceWindowHandle = hwnd.Handle;
						_pp.BackBufferWidth = Math.Max(100, (int)ActualWidth);
						_pp.BackBufferHeight = Math.Max(100, (int)ActualHeight);
						Device.Reset(_pp);
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
					Result result = Device.TestCooperativeLevel();
					if (result.Failure)
					{
						throw new SharpDXException();
					}
					if (!_image.TryLock(_timeOutDuration))
					{
						return;
					}
					try
					{
						Device.SetRenderState(global::SharpDX.Direct3D9.RenderState.CullMode, Cull.None);
						Device.SetRenderState(global::SharpDX.Direct3D9.RenderState.ZEnable, true);
						Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Transparent, float.MaxValue, 0);
						Device.BeginScene();
						Render.Raise(this, new RenderEventArgs(_pp.BackBufferWidth, _pp.BackBufferHeight));
						Device.EndScene();
						Device.Present();

						_image.SetBackBuffer(D3DResourceType.IDirect3DSurface9, Device.GetBackBuffer(0, 0).NativePointer);
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
				Device.Reset(_pp);
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
			Device.Dispose();
			Direct3D.Dispose();
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
