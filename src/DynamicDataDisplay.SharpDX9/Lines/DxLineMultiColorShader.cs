using SharpDX.Direct3D9;

namespace Microsoft.Research.DynamicDataDisplay.SharpDX9.Lines
{
	public class DxLineMultiColorShader : TransformShader
	{
		public DxLineMultiColorShader(Device device) : base(device, nameof(DxLineMultiColorShader))
		{
		}
	}
}
