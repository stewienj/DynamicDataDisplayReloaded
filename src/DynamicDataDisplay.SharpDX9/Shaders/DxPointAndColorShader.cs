using SharpDX.Direct3D9;

namespace DynamicDataDisplay.SharpDX9.Shaders
{
	public class DxPointAndColorShader : TransformShader
	{
		public DxPointAndColorShader(Device device) : base(device, nameof(DxPointAndColorShader))
		{
		}
	}
}
