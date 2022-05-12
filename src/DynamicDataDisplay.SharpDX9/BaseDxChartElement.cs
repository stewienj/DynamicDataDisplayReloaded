using DynamicDataDisplay.ViewModelTypes;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Linq;
using System.Windows;

namespace DynamicDataDisplay.SharpDX9
{
    public abstract class BaseDxChartElement : FrameworkElement, IPlotterElement
    {
        private SharpDXHost _dxHost;

        protected SharpDXHost DxHost => _dxHost;

        protected Device Device => _dxHost != null ? _dxHost.Device : null;

        private void OnDirectXRender(object sender, RenderEventArgs e) => OnDirectXRender(e.Width, e.Height);

        protected virtual void OnDirectXRender(int width, int height) { }

        public DataRect VisibleRect => Plotter.Viewport.Visible;

        public Matrix DxDataTransform { get; private set; } = Matrix.Identity;

        public System.Numerics.Matrix4x4 DataTransform
        {
            get => (System.Numerics.Matrix4x4)GetValue(DataTransformProperty);
            set => SetValue(DataTransformProperty, value);
        }

        // Using a DependencyProperty as the backing store for DataTranform.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataTransformProperty =
            DependencyProperty.Register("DataTransform", typeof(System.Numerics.Matrix4x4), typeof(BaseDxChartElement), new PropertyMetadata(System.Numerics.Matrix4x4.Identity, (s, e) =>
            {
                if (s is BaseDxChartElement control && e.NewValue is System.Numerics.Matrix4x4 matrix)
                {
                    control.DxDataTransform = new Matrix(
                        matrix.M11, matrix.M12, matrix.M13, matrix.M14,
                        matrix.M21, matrix.M22, matrix.M23, matrix.M24,
                        matrix.M31, matrix.M32, matrix.M33, matrix.M34,
                        matrix.M41, matrix.M42, matrix.M43, matrix.M44);
                }
            }));

        public float DxDepth { get; private set; } = 1f;

        public float Depth
        {
            get => (float)GetValue(DepthProperty);
            set => SetValue(DepthProperty, value);
        }

        // Using a DependencyProperty as the backing store for Depth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DepthProperty =
            DependencyProperty.Register("Depth", typeof(float), typeof(BaseDxChartElement), new PropertyMetadata(1f, (s,e) =>
            {
                if (s is BaseDxChartElement control && e.NewValue is float newDepth)
                {
                    control.DxDepth = newDepth;
                }
            }));

        public D3Color DxColor { get; private set; }

        public System.Windows.Media.Color Color
        {
            get => (System.Windows.Media.Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        // Using a DependencyProperty as the backing store for LineColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(System.Windows.Media.Color), typeof(BaseDxChartElement), new PropertyMetadata(System.Windows.Media.Colors.Black, (s, e) =>
            {
                if (s is BaseDxChartElement control && e.NewValue is System.Windows.Media.Color newColor)
                {
                    control.DxColor = new D3Color(newColor);
                }
            }));

        #region IPlotterElement Members

        private Plotter2D plotter;
        protected Plotter2D Plotter => plotter;

        public virtual void OnPlotterAttached(Plotter plotter)
        {
            this.plotter = (Plotter2D)plotter;
            _dxHost = this.plotter.Children.OfType<SharpDXHost>().FirstOrDefault();
            if (_dxHost == null)
                throw new InvalidOperationException("First add DirectXHost to plotter.Children");

            _dxHost.AddChild(this);
            _dxHost.Render += OnDirectXRender;
        }

        public virtual void OnPlotterDetaching(Plotter plotter)
        {
            _dxHost.RemoveChild(this);
            _dxHost.Render -= OnDirectXRender;
            this.plotter = null;
        }

        Plotter IPlotterElement.Plotter => plotter;

        #endregion
    }
}
