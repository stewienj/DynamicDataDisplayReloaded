using DynamicDataDisplay.ViewModelTypes;
using DynamicDataDisplay.SharpDX9.Shaders;
using SharpDX.Direct3D9;
using System.Windows;
using DynamicDataDisplay.SharpDX9.Textures;

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

    public class DxMultiThickLineTextured : BaseDxPrimitive<D3Point>
    {
        protected override BaseDxTransformShader GetTransformEffect(Device device)
        {
            return new DxPointSingleColorShader(Device);
        }

        protected override PrimitiveType GetPrimitiveType()
        {
            return PrimitiveType.LineList;
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
    }
}
