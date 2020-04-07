using System;
using System.Windows;

namespace AxesApp
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
			var genericPlotter = plotter.GetGenericPlotter<DateTime, double>();
			genericPlotter.DataRect = new DynamicDataDisplay.GenericRect<DateTime, double>(DateTime.Now, 0, DateTime.Now.AddMinutes(60), 1);
		}
	}
}
