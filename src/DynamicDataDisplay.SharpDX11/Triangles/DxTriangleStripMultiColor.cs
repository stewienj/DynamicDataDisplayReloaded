using DynamicDataDisplay.SharpDX11.DataTypes;
using DynamicDataDisplay.SharpDX11.Shaders;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDataDisplay.SharpDX11.Triangles
{
	public class DxTriangleStripMultiColor : BaseDxPrimitive<DxPointAndColor>
	{
		protected override BaseDxTransformShader GetTransformEffect(Device device)
		{
			return new DxPointAndColorShader(device);
		}

		protected override PrimitiveType GetPrimitiveType()
		{
			return PrimitiveType.TriangleStrip;
		}
	}
}
