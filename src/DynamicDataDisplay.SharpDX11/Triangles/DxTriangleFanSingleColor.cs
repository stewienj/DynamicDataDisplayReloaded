﻿using DynamicDataDisplay.SharpDX11.DataTypes;
using DynamicDataDisplay.SharpDX11.Shaders;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDataDisplay.SharpDX11.Triangles
{
	public class DxTriangleFanSingleColor : BaseDxPrimitive<DxPoint>
	{
		protected override BaseDxTransformShader GetTransformEffect(Device device)
		{
			return new DxPointSingleColorShader(Device);
		}

		protected override PrimitiveType GetPrimitiveType()
		{
			return PrimitiveType.TriangleFan;
		}
	}
}
