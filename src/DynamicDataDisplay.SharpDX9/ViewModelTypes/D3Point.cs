using System.Numerics;

namespace DynamicDataDisplay.ViewModelTypes
{
    /// <summary>
    /// Stores a point as 2 floats
    /// </summary>
    public struct D3Point : ID3Point
    {
        private Vector2 _point;

        public D3Point(System.Windows.Point point) : this(new Vector2((float)point.X, (float)point.Y))
        {
        }

        public D3Point(double x, double y) : this(new Vector2((float)x, (float)y))
        {
        }
        public D3Point(float x, float y) : this(new Vector2(x, y))
        {
        }

        public D3Point(Vector2 point)
        {
            _point = point;
        }

        public void Offset(double offsetX, double offsetY)
        {
            _point += new Vector2((float)offsetX, (float)offsetY);
        }

        public void Offset(float offsetX, float offsetY)
        {
            _point += new Vector2(offsetX, offsetY);
        }

        public float X
        {
            get => _point.X;
            set => _point.X = value;
        }

        public float Y
        {
            get => _point.Y;
            set => _point.Y = value;
        }
    }
}
