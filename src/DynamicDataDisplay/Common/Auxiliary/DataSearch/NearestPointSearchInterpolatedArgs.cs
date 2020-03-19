using System;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary.DataSearch
{
	public class NearestPointSearchInterpolatedArgs : EventArgs
	{
		public NearestPointSearchInterpolatedArgs(Point mousePos, IEnumerable<double> positions)
		{
			MousePosInData = mousePos;
			PositionsInData = positions;
		}

		public Point MousePosInData { get; private set; }

		public IEnumerable<double> PositionsInData { get; private set; }
	}
}
