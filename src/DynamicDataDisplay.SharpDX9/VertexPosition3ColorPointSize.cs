using SharpDX;

namespace DynamicDataDisplay.SharpDX9
{
	public struct VertexPosition3ColorPointSize
	{
		public static int SizeInBytes
		{
			get { return 3 * 4 + 4 + 4; }
		}

		public Vector3 Position;
		public float PointSize;
		public int Color;
	}
}
