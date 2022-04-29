using DynamicDataDisplay.Common.Auxiliary;
using DynamicDataDisplay.ViewModelTypes;
using DynamicDataDisplay.SharpDX9.Shaders;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DynamicDataDisplay.SharpDX9.Lines
{
	/// <summary>
	/// This takes a collection of points and renders a continous line in a single color
	/// </summary>
	public class DxLineSingleColor : BaseDxPrimitive<D3Point>
	{
		protected override BaseDxTransformShader GetTransformEffect(Device device)
		{
			return new DxPointSingleColorShader(Device);
		}

		protected override PrimitiveType GetPrimitiveType()
		{
			return PrimitiveType.LineStrip;
		}
	}
}
