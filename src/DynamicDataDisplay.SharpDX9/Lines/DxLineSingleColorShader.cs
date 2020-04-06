using Microsoft.Research.DynamicDataDisplay.SharpDX9.DataSources;
using SharpDX;
using SharpDX.Direct3D9;

namespace Microsoft.Research.DynamicDataDisplay.SharpDX9.Lines
{
	public class DxLineSingleColorShader : TransformShader
	{
		public DxLineSingleColorShader(Device device) : base(device, nameof(DxLineSingleColorShader))
		{
		}
		public void BeginEffect(DataRect dataRect, DxColor color)
		{
			BeginEffect(dataRect, color, Matrix.Identity);
		}

		public void BeginEffect(DataRect dataRect, DxColor color, Matrix dataTransform)
		{
			base.BeginEffect(dataRect, dataTransform);
			_effect.SetValue("pointColor", color.Float4);
		}

	}
}
