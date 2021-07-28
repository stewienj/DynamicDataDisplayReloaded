using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDataDisplay.SharpDX11.DataTypes
{
	public interface IDxPoint
	{
		InputElement[] GetInputElements();

		float X { get; }

		float Y { get; }
	}
}
