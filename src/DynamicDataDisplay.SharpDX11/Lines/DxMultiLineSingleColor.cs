using DynamicDataDisplay.Common.Auxiliary;
using DynamicDataDisplay.SharpDX11.DataTypes;
using DynamicDataDisplay.SharpDX11.Shaders;
using SharpDX;
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
	/// This takes a collection of points and renders a continous line in a single color
	/// </summary>
	public class DxMultiLineSingleColor : BaseDxPrimitive<DxPoint>
	{
		protected override BaseDxTransformShader GetTransformEffect(Device device)
		{
			return new DxPointSingleColorShader(Device);
		}

		protected override PrimitiveType GetPrimitiveType()
		{
			return PrimitiveType.LineList;
		}
	}
}
