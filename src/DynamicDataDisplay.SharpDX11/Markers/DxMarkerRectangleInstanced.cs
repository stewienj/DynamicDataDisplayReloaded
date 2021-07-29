using DynamicDataDisplay.SharpDX11.DataTypes;
using DynamicDataDisplay.SharpDX11.Shaders;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace DynamicDataDisplay.SharpDX11.Markers
{
    /// <summary>
    /// This takes a collection of points and vertices and renders rectangles.
    /// </summary>
    public class DxMarkerRectangleInstanced : BaseDxInstancedTexturePrimitive<DxVertex, DxInstancePoint>
	{
		protected override BaseDxTransformShader GetTransformEffect(Device device)
		{
			return new DxRectangleInstancedTexturedShader(Device);
		}

		protected override PrimitiveTopology GetPrimitiveTopology()
		{
			return PrimitiveTopology.TriangleList;
		}
	}
}
