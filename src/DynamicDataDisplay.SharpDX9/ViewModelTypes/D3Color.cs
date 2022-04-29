namespace DynamicDataDisplay.ViewModelTypes
{
    /// <summary>
    /// Dx color, converts a color to 4 floats
    /// </summary>
    public struct D3Color
    {
        private System.Numerics.Vector4 _color;

        private const float ByteToFloat = 1f / 255f;

        public D3Color(System.Windows.Media.Color color) : this(ToVector(color))
        { 
        }

        public D3Color(float r, float g, float b, float a = 1f) : this(new System.Numerics.Vector4(r, g, b, a))
        {
        }

        public D3Color(System.Numerics.Vector4 color)
        {
            _color = color;
        }

        public static System.Numerics.Vector4 ToVector(System.Windows.Media.Color color) => new System.Numerics.Vector4(color.R * ByteToFloat, color.G * ByteToFloat, color.B * ByteToFloat, color.A * ByteToFloat);

        public System.Numerics.Vector4 Float4 => _color;
    }
}
