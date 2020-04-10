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
	public struct DxInstancePoint : IDxPoint
	{
		private Vector4 _point;

		public DxInstancePoint(System.Windows.Point point) : this(ToVector((float)point.X, (float)point.Y))
		{
		}

		public DxInstancePoint(System.Windows.Point point, float depth) : this(ToVector((float)point.X, (float)point.Y, depth))
		{
		}

		public DxInstancePoint(float x, float y, float depth) : this(ToVector(x, y, depth))
		{
		}
		public DxInstancePoint(float x, float y, float z, float w) : this(ToVector(x, y, z, w))
		{
		}

		public DxInstancePoint(Vector4 point)
		{
			_point = point;
		}

		public VertexElement[] GetVertexElements()
		{
			// Allocate Vertex Elements
			var vertexElems = new[] {
				new VertexElement(1, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Position, 1),
				VertexElement.VertexDeclarationEnd
			};
			return vertexElems;
		}

		/// <summary>
		/// Convert x,y or x,y,z to Vector4. Note that w will be set for 1 as this is meant to be a point. When w is 1 then tranlations
		/// will be applied. When w is 0 then translations won't be applied as it represents a vector.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <param name="w"></param>
		/// <returns></returns>
		public static Vector4 ToVector(float x, float y, float z = 1, float w = 1) => new Vector4(x, y, z, w);

		public float X => _point.X;

		public float Y => _point.Y;

		public Vector4 Float4 => _point;
	}
}
