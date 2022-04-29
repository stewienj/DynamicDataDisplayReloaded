using System.Numerics;

namespace DynamicDataDisplay.ViewModelTypes
{
    /// <summary>
	/// This is same as DxPoint, except it specifies stream 1, and has position usage index 1, vs 0 in DxPoint for each of those
	/// </summary>
	public struct D3InstancePoint : ID3Point
	{
		private Vector2 _point;

		public D3InstancePoint(System.Windows.Point point) : this(new Vector2((float)point.X, (float)point.Y))
		{
		}

		public D3InstancePoint(float x, float y) : this(new Vector2(x, y))
		{
		}

		public D3InstancePoint(Vector2 point)
		{
			_point = point;
		}

		public float X => _point.X;

		public float Y => _point.Y;
	}
}
