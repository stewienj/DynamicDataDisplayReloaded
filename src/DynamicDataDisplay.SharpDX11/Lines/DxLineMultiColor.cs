﻿using DynamicDataDisplay.Common.Auxiliary;
using DynamicDataDisplay.SharpDX11.DataTypes;
using DynamicDataDisplay.SharpDX11.Shaders;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DynamicDataDisplay.SharpDX11.Lines
{
	/// <summary>
	/// This takes a collection of PointAndColor objects and renders them in a continuous line
	/// </summary>
	public class DxLineMultiColor : BaseDxPrimitive<DxPointAndColor>
	{
		protected override BaseDxTransformShader GetTransformEffect(Device device)
		{
			return new DxPointAndColorShader(device);
		}

		protected override PrimitiveTopology GetPrimitiveTopology()
		{
			return PrimitiveTopology.LineStrip;
		}

	}
}
