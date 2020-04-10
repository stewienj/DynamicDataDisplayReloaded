using DynamicDataDisplay.Common.Auxiliary;
using DynamicDataDisplay.SharpDX9.DataTypes;
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
	public class DxInstancedLineSingleColor : BaseDxInstancedPrimitive<DxPoint, DxInstancePoint>
	{
		private DxPointGlobalColorShader _shader;

		public override void OnPlotterAttached(Plotter plotter)
		{
			base.OnPlotterAttached(plotter);
		}

		protected override BaseDxTransformShader GetTransformEffect(Device device)
		{
			_shader = new DxPointInstancedGlobalColorShader(Device);
			_shader.DxColor = new DxColor(Color);
			return _shader;
		}

		protected override PrimitiveType GetPrimitiveType()
		{
			return PrimitiveType.LineStrip;
		}

		protected override void SetColor(DxColor color)
		{
			if (_shader != null)
			{
				_shader.DxColor = color;
			}
		}
	}
}
