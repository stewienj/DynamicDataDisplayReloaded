using DynamicDataDisplay.ViewModelTypes;
using DynamicDataDisplay.SharpDX9.Shaders;
using SharpDX.Direct3D9;

namespace DynamicDataDisplay.SharpDX9.Markers
{
    /// <summary>
    /// This takes a collection of points and vertices and renders rectangles.
    /// </summary>
    public class DxMarkerRectangleTexturedByInstance : BaseDxInstancedTexturePrimitive<D3Vertex, D3InstancedPoint>
	{
		protected override BaseDxTransformShader GetTransformEffect(Device device)
		{
			return new DxRectangleTexturedInstanceShader(Device);
		}

		protected override PrimitiveType GetPrimitiveType()
		{
			return PrimitiveType.TriangleList;
		}
	}
}
