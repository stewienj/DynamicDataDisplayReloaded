using SlimDX;
using SlimDX.Direct3D9;
using System.Runtime.InteropServices;

namespace DynamicDataDisplay.Visualization3D.VertexStructures
{
	[StructLayout(LayoutKind.Sequential)]
	struct Vertex
	{
		[VertexStructures.VertexElement(DeclarationType.Float3, DeclarationUsage.Position)]
		public Vector3 Position;

		public static int SizeInBytes
		{
			get { return Marshal.SizeOf(typeof(Vertex)); }
		}

	}
}
