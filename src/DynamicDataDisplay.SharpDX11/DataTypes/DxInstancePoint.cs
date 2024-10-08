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
	/// This is same as DxPoint, except it specifies stream 1, and has position usage index 1, vs 0 in DxPoint for each of those
	/// </summary>
	public struct DxInstancePoint : IDxPoint
	{
		private Vector2 _point;

		public DxInstancePoint(System.Windows.Point point) : this(new Vector2((float)point.X, (float)point.Y))
		{
		}

		public DxInstancePoint(float x, float y) : this(new Vector2(x, y))
		{
		}

		public DxInstancePoint(Vector2 point)
		{
			_point = point;
		}

		public VertexElement[] GetVertexElements()
		{
			// Allocate Vertex Elements
			var vertexElems = new[] {
				new VertexElement(1, 0, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.Position, 1),
				VertexElement.VertexDeclarationEnd
			};
			return vertexElems;
		}

		public float X => _point.X;

		public float Y => _point.Y;
	}
}
