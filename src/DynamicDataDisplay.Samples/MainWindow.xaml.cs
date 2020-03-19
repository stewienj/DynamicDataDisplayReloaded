using System.Globalization;
using System.Threading;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
			InitializeComponent();
		}
	}
}
