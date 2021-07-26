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
	/// This takes a collection of PointAndColor objects and renders them in seprate lines
	/// </summary>
	public class DxMultiLineMultiColor : BaseDxPrimitive<DxPointAndColor>
	{
		protected override BaseDxTransformShader GetTransformEffect(Device device)
		{
			return new DxPointAndColorShader(device);
		}

		protected override PrimitiveType GetPrimitiveType()
		{
			return PrimitiveType.LineList;
		}

	}
}
