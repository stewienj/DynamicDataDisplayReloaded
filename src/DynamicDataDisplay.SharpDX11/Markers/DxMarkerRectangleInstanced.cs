﻿using DynamicDataDisplay.SharpDX9.DataTypes;
using DynamicDataDisplay.SharpDX9.Shaders;
using SharpDX.Direct3D9;

namespace DynamicDataDisplay.SharpDX9.Markers
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

		protected override PrimitiveType GetPrimitiveType()
		{
			return PrimitiveType.TriangleList;
		}
	}
}
