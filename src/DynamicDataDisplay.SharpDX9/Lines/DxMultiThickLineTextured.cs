using DynamicDataDisplay.ViewModelTypes;
using DynamicDataDisplay.SharpDX9.Shaders;
using SharpDX.Direct3D9;
using System.Windows;
using DynamicDataDisplay.SharpDX9.Textures;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Threading;
using System.Windows.Media;

namespace DynamicDataDisplay.SharpDX9.Lines
{
    /*
    /// <summary>
    /// This takes a collection of vertices and renders a textured rectangle.
    /// Based on DxMarkerRectangleTextured
    /// </summary>
    public class DxMultiThickLineTextured : BaseDxTexturePrimitive<D3Vertex>
    {
        protected override BaseDxTransformShader GetTransformEffect(Device device)
        {
            // ToDo write dedicated shader
            return new DxRectangleTexturedShader(Device);
        }

        protected override PrimitiveType GetPrimitiveType()
        {
            return PrimitiveType.TriangleList;
        }
    }
    */

    /// <summary>
    /// Placeholder implementation, so this can be used before it is finished.
    /// </summary>

    public class DxMultiThickLineTextured : BaseDxPrimitive<D3Point>
    {
        public DxMultiThickLineTextured()
        {
            this.IsHitTestVisible = true;
        }

        public override void OnPlotterAttached(Plotter plotter)
        {
            base.OnPlotterAttached(plotter);
            if (_dxLeftMouseButtonDownHandlerCount > 0 && Plotter != null)
            {
                Plotter.MouseLeftButtonDown += Plotter_MouseLeftButtonDown;
            }
        }

        public override void OnPlotterDetaching(Plotter plotter)
        {
            if (_dxLeftMouseButtonDownHandlerCount > 0 && Plotter != null)
            {
                Plotter.MouseLeftButtonDown -= Plotter_MouseLeftButtonDown;
            }
            base.OnPlotterDetaching(plotter);
        }


        protected override BaseDxTransformShader GetTransformEffect(Device device)
        {
            return new DxPointSingleColorShader(Device);
        }

        protected override PrimitiveType GetPrimitiveType()
        {
            return PrimitiveType.LineList;
        }

        private void Plotter_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // TODO: Hit testing of the line
        }

        public double LineThickness
        {
            get { return (double)GetValue(LineThicknessProperty); }
            set { SetValue(LineThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LineThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineThicknessProperty =
            DependencyProperty.Register("LineThickness", typeof(double), typeof(DxMultiThickLineTextured), new PropertyMetadata(1.0));


        // TODO delete SharedTexture once we inherit from BaseDxTexturePrimitive

        /// <summary>
        /// A shared texture
        /// </summary>
        public DxSharedTexture SharedTexture
        {
            get { return (DxSharedTexture)GetValue(SharedTextureProperty); }
            set { SetValue(SharedTextureProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SharedTextureProperty =
            DependencyProperty.Register("SharedTexture", typeof(DxSharedTexture), typeof(DxMultiThickLineTextured), new PropertyMetadata(null, (s, e) =>
            {
                if (s is DxMultiThickLineTextured primitive)
                {
                   // primitive._texture = (e.NewValue as DxSharedTexture)?.GetTexture(primitive.Device);
                }
            }));

        public IEnumerable<double> DashPattern
        {
            get { return (IEnumerable<double>)GetValue(DashPatternProperty); }
            set { SetValue(DashPatternProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DashPattern.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DashPatternProperty =
            DependencyProperty.Register("DashPattern", typeof(IEnumerable<double>), typeof(DxMultiThickLineTextured), new PropertyMetadata(Enumerable.Empty<double>()));


        // We only hook the Plotter MouseLeftButtonDown and do hit testing if there is a listener
        // so this keep tabs on whether we have a listener or not

        private int _dxLeftMouseButtonDownHandlerCount = 0;
        private event MouseButtonEventHandler _dxMouseLeftButtonDown;
        public event MouseButtonEventHandler DxMouseLeftButtonDown
        {
            add
            {
                _dxMouseLeftButtonDown += value;
                if (Interlocked.Increment(ref _dxLeftMouseButtonDownHandlerCount) == 1)
                {
                    if (Plotter != null)
                    {
                        Plotter.MouseLeftButtonDown += Plotter_MouseLeftButtonDown;
                    }
                }
            }
            remove
            {
                _dxMouseLeftButtonDown -= value;

                if (Interlocked.Decrement(ref _dxLeftMouseButtonDownHandlerCount) == 0)
                {
                    if (Plotter != null)
                    {
                        Plotter.MouseLeftButtonDown -= Plotter_MouseLeftButtonDown;
                    }
                }
            }
        }
    }
}
