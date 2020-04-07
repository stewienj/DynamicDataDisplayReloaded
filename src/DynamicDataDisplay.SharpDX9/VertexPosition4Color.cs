using SharpDX;

namespace DynamicDataDisplay.SharpDX9
{
	public struct VertexPosition4Color
	{
		public static int SizeInBytes
		{
			get { return 4 * sizeof(float) + sizeof(int); }
		}

		public Vector4 Position;
		public int Color;
	}
}
