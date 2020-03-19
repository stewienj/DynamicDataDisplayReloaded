using System;
using System.Windows;

namespace AxisControlSample
{
	/// <summary>
	/// Interaction logic for Window3.xaml
	/// </summary>
	public partial class Window3 : Window
	{
		public Window3()
		{
			InitializeComponent();
			plotter.SetHorizontalAxisMapping(0, DateTime.Now, 1, DateTime.Now.AddYears(1));
		}
	}
}
