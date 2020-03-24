using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D9;
using System.Windows;
using System.Windows.Interop;
using SlimDX;
using System.Diagnostics;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay.SlimDX
{
	public class SlimDXHost : FrameworkElement, IPlotterElement
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
			Initialize3D();
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
			_pp.BackBufferWidth = Math.Max(1, (int)ActualWidth);
			_pp.BackBufferHeight = Math.Max(1, (int)ActualHeight);
			_pp.BackBufferFormat = Format.A8R8G8B8;
			_pp.AutoDepthStencilFormat = Format.D32SingleLockable;

			try
			{
				var direct3DEx = new Direct3DEx();
				Direct3D = direct3DEx;
				Device = new DeviceEx(direct3DEx, 0, DeviceType.Hardware, hwnd.Handle, CreateFlags.HardwareVertexProcessing, _pp);
			}
			catch
			{
				Direct3D = new Direct3D();
				Device = new Device(Direct3D, 0, DeviceType.Hardware, hwnd.Handle, CreateFlags.HardwareVertexProcessing, _pp);
			}
			System.Windows.Media.CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
		}

		Duration _timeOutDuration = new Duration(TimeSpan.FromMilliseconds(200));
		int _counter = 0;
		private void CompositionTarget_Rendering(object sender, EventArgs e)
		{
			if (_counter++ % 2 == 0) return;
			if (_image == null) return;

			try
			{
				// Lock the image while we are resetting as the rendering system freezes when we get a clash
				if (_sizeChanged && _image.TryLock(_timeOutDuration))
				{
					try
					{
						_pp.BackBufferWidth = Math.Max(1, (int)ActualWidth);
						_pp.BackBufferHeight = Math.Max(1, (int)ActualHeight);
						Device.Reset(_pp);
						_counter = 0;
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
					if (result.IsFailure)
					{
						throw new Direct3D9Exception();
					}
					if (!_image.TryLock(_timeOutDuration))
					{
						return;
					}

					try
					{
						Device.SetRenderState(global::SlimDX.Direct3D9.RenderState.CullMode, Cull.None);
						Device.SetRenderState(global::SlimDX.Direct3D9.RenderState.ZEnable, true);
						Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, new Color4(0, 0, 0, 0), 1.0f, 0);
						Device.BeginScene();

						Render.Raise(this);

						Device.EndScene();
						Device.Present();

						_image.SetBackBuffer(D3DResourceType.IDirect3DSurface9, Device.GetBackBuffer(0, 0).ComPointer);
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
			catch (Direct3D9Exception exc)
			{
				Device.Reset(_pp);
				Debug.WriteLine("Exception in main render loop: " + exc.Message);
			}
		}

		public event EventHandler Render;

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
		}

		public Plotter Plotter => _plotter;

		#endregion

	}
}
