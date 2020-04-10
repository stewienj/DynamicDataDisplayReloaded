using DynamicDataDisplay.SharpDX9.DataTypes;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Runtime.CompilerServices;

namespace DynamicDataDisplay.SharpDX9.Shaders
{
	public class DxPointGlobalColorShader : BaseDxTransformShader
	{

		public DxPointGlobalColorShader(Device device, [CallerFilePath] string callerFileName = "") : base(device, callerFileName)
		{
		}

		public override void BeginEffect(DataRect dataRect, Matrix dataTransform)
		{
			_effect.SetValue("pointColor", DxColor.Float4);
			base.BeginEffect(dataRect, dataTransform);
		}

		public override void DoMultipassEffect(DataRect dataRect, Action<int> processPass, Matrix dataTransform)
		{
			_effect.SetValue("pointColor", DxColor.Float4);
			base.DoMultipassEffect(dataRect, processPass, dataTransform);
		}

		public DxColor DxColor { get; set; } = new DxColor(System.Windows.Media.Colors.Black);

	}
}
