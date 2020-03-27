using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Research.DynamicDataDisplay.SharpDX9
{
	public class TransformShader
	{
		private Effect _effect;
		private EffectHandle _technique;
		private EffectHandle _pass;

		public TransformShader(Device device, string effectName)
		{
			_effect = Effect.FromString(device, Global.GetResourceText($"{effectName}.fx"), ShaderFlags.None);
			_technique = _effect.GetTechnique(0);
			_pass = _effect.GetPass(_technique, 0);
		}

		public void BeginEffect(DataRect dataRect)
		{
			BeginEffect(dataRect, Matrix.Identity);
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
			_effect.Begin();
			_effect.BeginPass(0);
			_effect.SetValue("worldViewProj", worldViewProj);
		}

		public virtual void EndEffect()
		{
			_effect.EndPass();
			_effect.End();
		}
	}
}
