using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDataDisplay.SharpDX9.DataSources
{
	// Probably don't need to set this explicitly but will just in case
	public struct DxPointAndColor
	{
		private Vector4 _point;
		private Vector4 _color;

		private const float ByteToFloat = 1f / 255f;

		public DxPointAndColor(System.Windows.Point point, System.Windows.Media.Color color) : this(ToVector((float)point.X, (float)point.Y), ToVector(color))
		{ 
		}

		public DxPointAndColor(System.Windows.Point point, float depth, System.Windows.Media.Color color)  : this(ToVector((float)point.X, (float)point.Y, depth), ToVector(color))
		{
		}

		public DxPointAndColor(float x, float y, float depth, float r, float g, float b, float a = 1f) : this(ToVector(x, y, depth), ToVector(r, g, b, a))
		{
		}

		public DxPointAndColor(Vector4 point, Vector4 color)
		{
			_point = point;
			_color = color;
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

		public static Vector4 ToVector(System.Windows.Media.Color color) => new Vector4(color.R * ByteToFloat, color.G * ByteToFloat, color.B * ByteToFloat, color.A * ByteToFloat);

		public float X => _point.X;

		public float Y => _point.Y;

		public Vector4[] Float4s => new Vector4[]{_point, _color};

	}
}
