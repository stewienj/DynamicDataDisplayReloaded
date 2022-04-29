using System.Numerics;

namespace DynamicDataDisplay.ViewModelTypes
{
    // Probably don't need to set this explicitly but will just in case
    public struct D3InstancedPointAndColor : ID3Point
    {
        private Vector2 _point;
        private int _color;

        private const float ByteToFloat = 1f / 255f;

        public D3InstancedPointAndColor(System.Windows.Point point, System.Windows.Media.Color color) : this(new Vector2((float)point.X, (float)point.Y), color.ToArgb())
        { 
        }

        public D3InstancedPointAndColor(System.Windows.Point point, int color) : this(new Vector2((float)point.X, (float)point.Y),color)
        {
        }

        public D3InstancedPointAndColor(float x, float y, System.Windows.Media.Color color) : this(new Vector2(x, y), color.ToArgb())
        {
        }

        public D3InstancedPointAndColor(double x, double y, int color) : this(new Vector2((float)x, (float)y), color)
        {
        }

        public D3InstancedPointAndColor(Vector2 point, int color)
        {
            _point = point;
            _color = color;
        }

        public float X => _point.X;

        public float Y => _point.Y;

        public int Color => _color;
    }
}
