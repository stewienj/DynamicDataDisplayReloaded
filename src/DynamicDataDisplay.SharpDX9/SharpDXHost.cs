using DynamicDataDisplay.Common.Auxiliary;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;

namespace DynamicDataDisplay.SharpDX9
{
    public class SharpDXHost : FrameworkElement, IPlotterElement
    {
        // ********************************************************************
        // Private Fields
        // ********************************************************************
        #region Private Fields

        /// <summary>
        /// Flag indicating an event handler has been added to the Composition Target for WPF
        /// </summary>
        private bool _compositionTargetRenderingHandlerAdded = false;

        /// <summary>
        /// A toggle so we only render on each alternate frame. If we don't do this it freezes.
        /// </summary>
        private bool _renderNextFrame = true;

        /// <summary>
        /// Amount of time we'll stall the renderer
        /// Note that if you exceed 2 seconds per frame render, then it will stop updating.
        /// </summary>
        private Duration _timeOutDuration = new Duration(TimeSpan.FromMilliseconds(2000));

        /// <summary>
        /// Intermediate image that DirectX9 renders to. This is then drawn on the WPF surface.
        /// </summary>
        private D3DImage _image = new D3DImage();

        /// <summary>
        /// Flag indicating that the back buffer needs to be resized to match the display surface size
        /// </summary>
        private volatile bool _sizeChanged;

        /// <summary>
        /// Presentation parameters for the DirectX9 device
        /// </summary>
        private PresentParameters _pp = new PresentParameters();

        /// <summary>
        /// Resize action that limits the number of times a resize can be acted upon to
        /// once per second.
        /// </summary>
        private ThrottledAction _resizeAction = new ThrottledAction(TimeSpan.FromMilliseconds(1000));

        /// <summary>
        /// Microsoft Direct3D 9Ex render environment
        /// </summary>
        private Direct3DEx _direct3D;

        #endregion Private Fields

        // ********************************************************************
        // Properties
        // ********************************************************************
        #region Properties

        /// <summary>
        /// The DirectX9 device
        /// </summary>
        public Device Device { get; private set; }

        #endregion Properties

        // ********************************************************************
        // Public Methods
        // ********************************************************************
        #region Public Methods

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


        #endregion Public Methods

        // ********************************************************************
        // Protected Overrides
        // ********************************************************************
        #region Protected Methods

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            _resizeAction.InvokeAction(() => _sizeChanged = true);
            base.OnRenderSizeChanged(sizeInfo);
        }


        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            drawingContext.DrawImage(_image, new Rect(RenderSize));
        }

        #endregion Protected Methods

        // ********************************************************************
        // Private Methods
        // ********************************************************************
        #region Private Methods

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
                _direct3D = new Direct3DEx();
                Device = new DeviceEx(_direct3D, 0, DeviceType.Hardware, hwnd.Handle, CreateFlags.HardwareVertexProcessing, _pp);
            }
            catch
            {
                // At this point we are pretty much screwed, require DeviceEx for the reset capability
                MessageBox.Show("Can't start DirectX, so that pretty much discounts doing anything else");
            }
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            _renderNextFrame = !_renderNextFrame;
            if (_renderNextFrame)
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
                    if (!_image.TryLock(_timeOutDuration))
                    {
                        return;
                    }
                    Result result = Device.TestCooperativeLevel();
                    if (result.Failure)
                    {
                        throw new SharpDXException();
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

        #endregion Private Methods

        // ********************************************************************
        // IPlotterElement Members
        // ********************************************************************
        #region IPlotterElement Members

        private Plotter2D _plotter;
        public void OnPlotterAttached(Plotter plotter)
        {
            _plotter = (Plotter2D)plotter;
            _plotter.CentralGrid.Children.Add(this);
            _plotter.Viewport.PropertyChanged += Viewport_PropertyChanged;

            Initialize3D();

            if (_plotter.IsVisible && !_compositionTargetRenderingHandlerAdded)
            {
                // Weak event handler for CompositionTarget_Rendering
                CompositionTargetRenderingEventManager.AddHandler(CompositionTarget_Rendering);
                _compositionTargetRenderingHandlerAdded = true;
            }
            _plotter.IsVisibleChanged += (s, e) =>
            {
                if (_plotter.IsVisible && !_compositionTargetRenderingHandlerAdded)
                {
                    // Weak event handler for CompositionTarget_Rendering
                    CompositionTargetRenderingEventManager.AddHandler(CompositionTarget_Rendering);
                    _compositionTargetRenderingHandlerAdded = true;
                }
                if (!_plotter.IsVisible && _compositionTargetRenderingHandlerAdded)
                {
                    CompositionTargetRenderingEventManager.RemoveHandler(CompositionTarget_Rendering);
                    _compositionTargetRenderingHandlerAdded = false;
                }
            };
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
            if (_compositionTargetRenderingHandlerAdded)
            {
                CompositionTargetRenderingEventManager.RemoveHandler(CompositionTarget_Rendering);
                _compositionTargetRenderingHandlerAdded = false;
            }

            _plotter.Viewport.PropertyChanged -= Viewport_PropertyChanged;
            _plotter.CentralGrid.Children.Remove(this);
            _plotter = null;
            Device.Dispose();
            _direct3D.Dispose();
        }

        public Plotter Plotter => _plotter;

        #endregion

        // ********************************************************************
        // IPlotterElement Members
        // ********************************************************************
        #region Events

        public event EventHandler<RenderEventArgs> Render;

        #endregion Events
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
