using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace DynamicDataDisplay.Charts.Navigation
{
	/// <summary>
	/// Interaction logic for AboutWindow.xaml
	/// </summary>
	public partial class AboutWindow : Window
	{
		public AboutWindow()
		{
			InitializeComponent();
		}

		private void Hyperlink_Click(object sender, RoutedEventArgs e)
		{
			Hyperlink source = (Hyperlink)sender;
			Process.Start(source.NavigateUri.ToString());
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			// close on Esc or Enter pressed
			if (e.Key == Key.Escape || e.Key == Key.Enter)
			{
				Close();
			}
		}

		private void Hyperlink_Click_1(object sender, RoutedEventArgs e)
		{
			Hyperlink source = (Hyperlink)sender;
			Process.Start(source.NavigateUri.ToString());
		}
	}
}
