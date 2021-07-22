using DynamicDataDisplay.SharpDX9.DataTypes;
using DynamicDataDisplay.SharpDX9.Shaders;
using SharpDX.Direct3D9;

namespace DynamicDataDisplay.SharpDX9.Markers
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

		protected override PrimitiveType GetPrimitiveType()
		{
			return PrimitiveType.TriangleList;
		}
	}
}
