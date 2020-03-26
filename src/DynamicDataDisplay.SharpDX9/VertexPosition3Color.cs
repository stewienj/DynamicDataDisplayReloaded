using SharpDX;

namespace Microsoft.Research.DynamicDataDisplay.SharpDX9
{
	public struct VertexPosition3Color
	{
		public static int SizeInBytes
		{
			get { return 3 * sizeof(float) + sizeof(int); }
		}

		public Vector3 Position;
		public int Color;
	}
}
