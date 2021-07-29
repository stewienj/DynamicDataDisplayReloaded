using DynamicDataDisplay.SharpDX11.DataTypes;
using DynamicDataDisplay.SharpDX11.Shaders;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace DynamicDataDisplay.SharpDX11.Markers
{
    /// <summary>
    /// This takes a collection of vertices and renders a textured rectangle.
    /// </summary>
    public class DxMarkerRectangle : BaseDxTexturePrimitive<DxVertex>
	{
		protected override BaseDxTransformShader GetTransformEffect(Device device)
		{
			return new DxRectangleTexturedShader(Device);
		}

		protected override PrimitiveTopology GetPrimitiveTopology()
		{
			return PrimitiveTopology.TriangleList;
		}
	}
}
