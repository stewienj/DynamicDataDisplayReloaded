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
	// Probably don't need to set this explicitly but will just in case
	public struct DxPointAndColor : IDxPoint
	{
		private Vector2 _point;
		private Vector4 _color;

		private const float ByteToFloat = 1f / 255f;

		public DxPointAndColor(System.Windows.Point point, System.Windows.Media.Color color) : this(new Vector2((float)point.X, (float)point.Y), color)
		{ 
		}

		public DxPointAndColor(float x, float y, System.Windows.Media.Color color) : this(new Vector2(x, y), color)
		{
		}

		public DxPointAndColor(Vector2 point, System.Windows.Media.Color color)
		{
			_point = point;
			_color = DxColor.ToVector(color);
		}

		public InputElement[] GetInputElements()
		{
			// Allocate Vertex Elements
			var inputElements = new[]
			{
				// TODO check names below correspond to names in the shader file
				new InputElement("POSITION", 0, Format.R32G32_Float, 0, 0),
				new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 8, 0)
			};
			return inputElements;
		}

		public float X => _point.X;

		public float Y => _point.Y;

		public Vector4 Color => _color;
	}
}
