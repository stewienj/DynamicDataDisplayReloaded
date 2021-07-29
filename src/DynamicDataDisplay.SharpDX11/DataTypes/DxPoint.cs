using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDataDisplay.SharpDX11.DataTypes
{
	/// <summary>
	/// Stores a point as 2 floats
	/// </summary>
	public struct DxPoint : IDxPoint
	{
		private Vector2 _point;

		public DxPoint(System.Windows.Point point) : this(new Vector2((float)point.X, (float)point.Y))
		{
		}

		public DxPoint(double x, double y) : this(new Vector2((float)x, (float)y))
		{
		}
		public DxPoint(float x, float y) : this(new Vector2(x, y))
		{
		}

		public DxPoint(Vector2 point)
		{
			_point = point;
		}

		public InputElement[] GetInputElements()
		{
			// Allocate Vertex Elements
			var inputElements = new[]
			{
				// TODO check names below correspond to names in the shader file
				new InputElement("POSITION", 0, Format.R32G32_Float, 0, 0)
			};
			return inputElements;
		}

		public float X => _point.X;

		public float Y => _point.Y;
	}
}
