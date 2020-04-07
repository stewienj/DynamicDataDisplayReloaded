using SharpDX.Direct3D9;

namespace DynamicDataDisplay.SharpDX9.Lines
{
	public class DxLineMultiColorShader : TransformShader
	{
		public DxLineMultiColorShader(Device device) : base(device, nameof(DxLineMultiColorShader))
		{
		}
	}
}
