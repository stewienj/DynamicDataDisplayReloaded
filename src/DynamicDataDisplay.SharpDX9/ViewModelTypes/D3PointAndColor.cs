using System.Numerics;

namespace DynamicDataDisplay.ViewModelTypes
{
    // Probably don't need to set this explicitly but will just in case
    public struct D3PointAndColor : ID3Point
	{
		private Vector2 _point;
		private int _color;

		private const float ByteToFloat = 1f / 255f;

		public D3PointAndColor(System.Windows.Point point, System.Windows.Media.Color color) : this(new Vector2((float)point.X, (float)point.Y), color.ToArgb())
		{ 
		}

		public D3PointAndColor(float x, float y, System.Windows.Media.Color color) : this(new Vector2(x, y), color.ToArgb())
		{
		}

		public D3PointAndColor(Vector2 point, int color)
		{
			_point = point;
			_color = color;
		}

		public float X => _point.X;

		public float Y => _point.Y;

		public int Color => _color;
	}
}
