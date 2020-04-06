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
	public struct DxColor
	{
		private Vector4 _color;

		private const float ByteToFloat = 1f / 255f;

		public DxColor(System.Windows.Media.Color color) : this(ToVector(color))
		{ 
		}

		public DxColor(float r, float g, float b, float a = 1f) : this(new Vector4(r, g, b, a))
		{
		}

		public DxColor(Vector4 color)
		{
			_color = color;
		}

		public static Vector4 ToVector(System.Windows.Media.Color color) => new Vector4(color.R * ByteToFloat, color.G * ByteToFloat, color.B * ByteToFloat, color.A * ByteToFloat);

		public Vector4 Float4 => _color;
	}
}
