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
	public struct DxInstancedPointAndColor : IDxPoint
	{
		private Vector2 _point;
		private int _color;

		private const float ByteToFloat = 1f / 255f;

		public DxInstancedPointAndColor(System.Windows.Point point, System.Windows.Media.Color color) : this(new Vector2((float)point.X, (float)point.Y), color.ToArgb())
		{ 
		}

		public DxInstancedPointAndColor(System.Windows.Point point, int color) : this(new Vector2((float)point.X, (float)point.Y),color)
		{
		}

		public DxInstancedPointAndColor(float x, float y, System.Windows.Media.Color color) : this(new Vector2(x, y), color.ToArgb())
		{
		}

		public DxInstancedPointAndColor(double x, double y, int color) : this(new Vector2((float)x, (float)y), color)
		{
		}

		public DxInstancedPointAndColor(Vector2 point, int color)
		{
			_point = point;
			_color = color;
		}

		public VertexElement[] GetVertexElements()
		{
			// Allocate Vertex Elements
			var vertexElems = new[] {
				new VertexElement(1, 0, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.Position, 1),
				new VertexElement(1, 8, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 1),
				VertexElement.VertexDeclarationEnd
			};
			return vertexElems;
		}

		public float X => _point.X;

		public float Y => _point.Y;

		public int Color => _color;
	}
}
