using System;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary.DataSearch
{
	public class NearestPointSearchArgs : EventArgs
	{

		public NearestPointSearchArgs(Point mousePos, Point nearestPoint, bool hasLock, string nearestLine)
		{
			MousePos = mousePos;
			NearestPoint = nearestPoint;
			HasLock = hasLock;
			NearestLine = nearestLine;
		}

		public Point MousePos
		{
			get; private set;
		}

		public Point NearestPoint
		{
			get; private set;
		}
		public bool HasLock
		{
			get; private set;
		}

		public string NearestLine
		{
			get; private set;
		}

	}
}
