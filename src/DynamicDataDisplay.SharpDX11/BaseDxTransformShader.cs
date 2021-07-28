using DynamicDataDisplay.SharpDX11.DataTypes;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDataDisplay.SharpDX11
{
	public class BaseDxTransformShader : IDisposable
	{
		protected Effect _effect;
		private EffectTechnique _technique;
		private EffectPass _pass;
		private CompilationResult _effectByteCode;
		private DeviceContext _deviceContext;

		public BaseDxTransformShader(Device device, [CallerFilePath] string callerFileName = "")
		{
			string effectName = Path.GetFileNameWithoutExtension(callerFileName);

			_effectByteCode = ShaderBytecode.Compile(GetResourceText($"{effectName}.fx"), "fx_5_0");

			_effect = new Effect(device, _effectByteCode);
			_technique = _effect.GetTechniqueByIndex(0);
			_pass = _technique.GetPassByIndex(0);
			_deviceContext = device.ImmediateContext;
		}

		public void Dispose()
		{
			_deviceContext.Dispose();
		}

		public string GetResourceText(string name)
		{
			Assembly a = this.GetType().Assembly;
			var resourceNames = a.GetManifestResourceNames();
			var matching = resourceNames.First(n => n.EndsWith(name));
			using (var stream = a.GetManifestResourceStream(matching))
			{
				using (var reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			}
		}

		public void DoMultipassEffect(int width, int height, BaseDxChartElement chart, Action<int> processPass)
		{
			DoMultipassEffect(width, height, chart.VisibleRect, chart.DxDepth, chart.DxColor, chart.DxDataTransform, processPass);
		}

		internal byte[] GetShaderByteCode() => _effectByteCode;

		protected virtual void DoMultipassEffect(int width, int height, DataRect dataRect, float depth, DxColor color, Matrix dataTransform, Action<int> processPass)
		{
			// Todo move the centre point
			var view = Matrix.LookAtLH(new Vector3((float)dataRect.CenterX, (float)dataRect.CenterY, -1f), new Vector3((float)dataRect.CenterX, (float)dataRect.CenterY, 0), Vector3.UnitY);
			var proj = Matrix.OrthoLH((float)dataRect.Width, (float)dataRect.Height, 0.1f, 100.0f);
			var viewProj = Matrix.Multiply(view, proj);

			// Can do any rotations etc here
			var worldViewProj = dataTransform * viewProj;

			_effect.GetConstantBufferByName("bufferWidth").AsScalar().Set((float)width);
			_effect.GetConstantBufferByName("bufferHeight").AsScalar().Set((float)height);
			_effect.GetConstantBufferByName("depth").AsScalar().Set(depth);
			_effect.GetConstantBufferByName("pointColor").AsVector().Set(color.Float4);
			_effect.GetConstantBufferByName("worldViewProj").AsMatrix().SetMatrix(worldViewProj);
			for (int passNo = 0; passNo < _technique.Description.PassCount; ++passNo)
			{
				_pass.Apply(_deviceContext);
				processPass(passNo);
			}
		}
	}
}
