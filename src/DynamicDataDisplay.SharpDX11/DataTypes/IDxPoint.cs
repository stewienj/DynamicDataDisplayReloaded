using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDataDisplay.SharpDX9.DataTypes
{
	public interface IDxPoint
	{
		VertexElement[] GetVertexElements();

		float X { get; }

		float Y { get; }
	}
}
