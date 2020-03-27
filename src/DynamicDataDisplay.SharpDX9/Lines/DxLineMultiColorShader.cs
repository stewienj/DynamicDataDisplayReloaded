using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Research.DynamicDataDisplay.SharpDX9.Lines
{
	public class DxLineMultiColorShader : TransformShader
	{
		public DxLineMultiColorShader(Device device) : base(device, nameof(DxLineMultiColorShader))
		{

		}
	}
}
