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
	/// This takes a collection of points and renders a continous line in a single color, then reproduces that line
	/// multiple times in the positions specified by the instance point locations.
	/// </summary>
	public class DxInstancedLineSingleColor : BaseDxInstancedPrimitive<DxPoint, DxInstancePoint>
	{
		public override void OnPlotterAttached(Plotter plotter)
		{
			base.OnPlotterAttached(plotter);
		}

		protected override BaseDxTransformShader GetTransformEffect(Device device)
		{
			return new DxPointInstancedSingleColorShader(Device);
		}

		protected override PrimitiveType GetPrimitiveType()
		{
			return PrimitiveType.LineStrip;
		}
	}
}
