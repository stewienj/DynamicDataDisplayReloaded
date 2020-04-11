using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDataDisplay.SharpDX9.DataTypes
{
	// Probably don't need to set this explicitly but will just in case
	public struct DxPointAndColor : IDxPoint
	{
		private Vector2 _point;
		private Vector4 _color;

		private const float ByteToFloat = 1f / 255f;

		public DxPointAndColor(System.Windows.Point point, System.Windows.Media.Color color) : this(new Vector2((float)point.X, (float)point.Y), ToVector4(color))
		{ 
		}

		public DxPointAndColor(float x, float y, float r, float g, float b, float a = 1f) : this(new Vector2(x, y), new Vector4(r, g, b, a))
		{
		}

		public DxPointAndColor(Vector2 point, Vector4 color)
		{
			_point = point;
			_color = color;
		}

		public VertexElement[] GetVertexElements()
		{
			// Allocate Vertex Elements
			var vertexElems = new[] {
				new VertexElement(0, 0, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.Position, 0),
				new VertexElement(0, 8, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Color, 0),
				VertexElement.VertexDeclarationEnd
			};
			return vertexElems;
		}

		public static Vector4 ToVector4(System.Windows.Media.Color color) => new Vector4(color.R * ByteToFloat, color.G * ByteToFloat, color.B * ByteToFloat, color.A * ByteToFloat);

		public float X => _point.X;

		public float Y => _point.Y;

		public Vector4 Color => _color;
	}
}
