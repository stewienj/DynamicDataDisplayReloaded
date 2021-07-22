using DynamicDataDisplay.SharpDX9.DataTypes;
using DynamicDataDisplay.SharpDX9.Shaders;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDataDisplay.SharpDX9.Triangles
{
	public class DxTriangleStripTextured : BaseDxPrimitive<DxVertex>
	{
		protected override BaseDxTransformShader GetTransformEffect(Device device)
		{
			return new DxRectangleTexturedShader(device);
		}

		protected override PrimitiveType GetPrimitiveType()
		{
			return PrimitiveType.TriangleStrip;
		}
	}
}
