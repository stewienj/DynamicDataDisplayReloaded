using SharpDX;
using SharpDX.Direct3D11;
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

		public VertexElement[] GetVertexElements()
		{
			// Allocate Vertex Elements
			var vertexElems = new[] {
				new VertexElement(0, 0, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.Position, 0),
				VertexElement.VertexDeclarationEnd
			};
			return vertexElems;
		}

		public float X => _point.X;

		public float Y => _point.Y;
	}
}
