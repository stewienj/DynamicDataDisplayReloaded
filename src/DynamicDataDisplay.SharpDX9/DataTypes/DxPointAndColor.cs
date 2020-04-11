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
		private int _color;

		private const float ByteToFloat = 1f / 255f;

		public DxPointAndColor(System.Windows.Point point, System.Windows.Media.Color color) : this(new Vector2((float)point.X, (float)point.Y), color.ToArgb())
		{ 
		}

		public DxPointAndColor(float x, float y, System.Windows.Media.Color color) : this(new Vector2(x, y), color.ToArgb())
		{
		}

		public DxPointAndColor(Vector2 point, int color)
		{
			_point = point;
			_color = color;
		}

		public VertexElement[] GetVertexElements()
		{
			// Allocate Vertex Elements
			var vertexElems = new[] {
				new VertexElement(0, 0, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.Position, 0),
				new VertexElement(0, 8, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
				VertexElement.VertexDeclarationEnd
			};
			return vertexElems;
		}

		public float X => _point.X;

		public float Y => _point.Y;

		public int Color => _color;
	}
}
