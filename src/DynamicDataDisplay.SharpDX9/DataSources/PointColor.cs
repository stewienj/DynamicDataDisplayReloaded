using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Research.DynamicDataDisplay.SharpDX9.DataSources
{
	// Probably don't need to set this explicitly but will just in case
	public struct PointColor
	{
		private Vector4 _point;
		private Vector4 _color;

		static readonly float ByteToFloat = 1f / 255f;

		public PointColor(System.Windows.Point point, System.Windows.Media.Color color) : this(point, 1, color)
		{
		}

		public PointColor(System.Windows.Point point, float depth, System.Windows.Media.Color color)
		{
			_point = new Vector4((float)point.X, (float)point.Y, depth, 1);
			_color = new Vector4(color.R * ByteToFloat, color.G * ByteToFloat, color.B * ByteToFloat, color.A * ByteToFloat);
		}

		public PointColor(float x, float y, float depth, System.Windows.Media.Color color)
		{
			_point = new Vector4(x, y, depth, 1);
			_color = new Vector4(color.R * ByteToFloat, color.G * ByteToFloat, color.B * ByteToFloat, color.A * ByteToFloat);
		}

		public PointColor(Vector4 point, Vector4 color)
		{
			_point = point;
			_color = color;
		}

		public float X => _point.X;

		public float Y => _point.Y;

		public Vector4[] Float4s => new Vector4[]{_point, _color};

	}
}
