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

namespace DynamicDataDisplay.SharpDX11.Markers
{
	/// <summary>
	/// This takes a collection of points and renders a continous line in a single color, then reproduces that line
	/// multiple times in the positions specified by the instance point locations.
	/// </summary>
	public class DxMarkerColoredByInstance : BaseDxInstancedPrimitive<DxPoint, DxInstancedPointAndColor>
	{
		public override void OnPlotterAttached(Plotter plotter)
		{
			base.OnPlotterAttached(plotter);
		}

		protected override BaseDxTransformShader GetTransformEffect(Device device)
		{
			return new DxMarkerColoredByInstanceShader(Device);
		}

		protected override PrimitiveTopology GetPrimitiveTopology()
		{
			return PrimitiveTopology.TriangleList;
		}
	}
}
