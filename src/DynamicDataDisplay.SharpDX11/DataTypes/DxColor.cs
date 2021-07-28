using SharpDX;

namespace DynamicDataDisplay.SharpDX11.DataTypes
{
	/// <summary>
	/// Dx color, converts a color to 4 floats
	/// </summary>
	public struct DxColor
	{
		private const float ByteToFloat = 1f / 255f;

		private Vector4 _color;

		public DxColor(System.Windows.Media.Color color) : this(ToVector(color)) { }

		public DxColor(float r, float g, float b, float a = 1f) : this(new Vector4(r, g, b, a)) { }

        public DxColor(Vector4 color) => _color = color;

        public static Vector4 ToVector(System.Windows.Media.Color color) => new Vector4(color.R * ByteToFloat, color.G * ByteToFloat, color.B * ByteToFloat, color.A * ByteToFloat);

		public Vector4 Float4 => _color;
	}
}
