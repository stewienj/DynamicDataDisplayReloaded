using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDataDisplay.SharpDX9
{
	public class BaseDxTransformShader
	{
		protected Effect _effect;
		private EffectHandle _technique;
		private EffectHandle _pass;

		public BaseDxTransformShader(Device device, [CallerFilePath] string callerFileName="")
		{
			string effectName = Path.GetFileNameWithoutExtension(callerFileName);
			_effect = Effect.FromString(device, GetResourceText($"{effectName}.fx"), ShaderFlags.None);
			_technique = _effect.GetTechnique(0);
			_pass = _effect.GetPass(_technique, 0);
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

		public void BeginEffect(DataRect dataRect)
		{
			BeginEffect(dataRect, Matrix.Identity);
		}

		public void DoMultipassEffect(DataRect dataRect, Action<int> processPass)
		{
			DoMultipassEffect(dataRect, processPass, Matrix.Identity);
		}

		public virtual void BeginEffect(DataRect dataRect, Matrix dataTransform)
		{
			// Todo move the centre point
			var view = Matrix.LookAtLH(new Vector3((float)dataRect.CenterX, (float)dataRect.CenterY, -1f), new Vector3((float)dataRect.CenterX, (float)dataRect.CenterY, 0), Vector3.UnitY);
			var proj = Matrix.OrthoLH((float)dataRect.Width, (float)dataRect.Height, 0.1f, 100.0f);
			var viewProj = Matrix.Multiply(view, proj);

			// Can do any rotations etc here
			var worldViewProj = dataTransform * viewProj;

			_effect.Technique = _technique;
			_effect.SetValue("worldViewProj", worldViewProj);
			_effect.Begin();
			_effect.BeginPass(0);
		}

		public virtual void DoMultipassEffect(DataRect dataRect, Action<int> processPass, Matrix dataTransform)
		{
			// Todo move the centre point
			var view = Matrix.LookAtLH(new Vector3((float)dataRect.CenterX, (float)dataRect.CenterY, -1f), new Vector3((float)dataRect.CenterX, (float)dataRect.CenterY, 0), Vector3.UnitY);
			var proj = Matrix.OrthoLH((float)dataRect.Width, (float)dataRect.Height, 0.1f, 100.0f);
			var viewProj = Matrix.Multiply(view, proj);

			// Can do any rotations etc here
			var worldViewProj = dataTransform * viewProj;

			_effect.Technique = _technique;

			_effect.SetValue("worldViewProj", worldViewProj);
			var passCount = _effect.Begin();
			for (int passNo = 0; passNo < passCount; ++passNo)
			{
				_effect.BeginPass(passNo);
				processPass(passNo);
				_effect.EndPass();
			}
			_effect.End();
		}

		public virtual void EndEffect()
		{
			_effect.EndPass();
			_effect.End();
		}
	}
}
