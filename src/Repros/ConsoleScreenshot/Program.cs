using DynamicDataDisplay;
using System;
using System.Windows.Media;

namespace ConsoleScreenshot
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			ChartPlotter plotter = new ChartPlotter();
			plotter.PerformLoad();
			plotter.Background = Brushes.Transparent;
			plotter.SaveScreenshot("1.png");
		}
	}
}
