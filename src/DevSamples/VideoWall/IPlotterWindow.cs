using Microsoft.Research.DynamicDataDisplay;
using System;

namespace VideoWall
{
	public interface IPlotterWindow
	{
		int X { get; }
		int Y { get; }

		ChartPlotter Plotter { get; }
		event EventHandler VisibleChanged;
	}
}
