using DynamicDataDisplay.SharpDX9.DataTypes;
using SharpDX;
using SharpDX.Direct3D9;

namespace DynamicDataDisplay.SharpDX9.Shaders
{
	public class DxPointGlobalColorShader : TransformShader
	{

		public DxPointGlobalColorShader(Device device) : base(device, nameof(DxPointGlobalColorShader))
		{
		}

		public override void BeginEffect(DataRect dataRect, Matrix dataTransform)
		{
			base.BeginEffect(dataRect, dataTransform);
			_effect.SetValue("pointColor", DxColor.Float4);
		}

		public DxColor DxColor { get; set; } = new DxColor(System.Windows.Media.Colors.Black);

	}
}
