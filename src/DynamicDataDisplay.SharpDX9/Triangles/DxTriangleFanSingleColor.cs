﻿using DynamicDataDisplay.SharpDX9.DataTypes;
using DynamicDataDisplay.SharpDX9.Shaders;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDataDisplay.SharpDX9.Triangles
{
	public class DxTriangleFanSingleColor : BaseDxPrimitive<DxPoint>
	{
		private DxPointGlobalColorShader _shader;

		protected override BaseDxTransformShader GetTransformEffect(Device device)
		{
			_shader = new DxPointGlobalColorShader(Device);
			_shader.DxColor = new DxColor(Color);
			return _shader;
		}

		protected override PrimitiveType GetPrimitiveType()
		{
			return PrimitiveType.TriangleFan;
		}
		protected override void SetColor(DxColor color)
		{
			_shader.DxColor = color;
		}
	}
}