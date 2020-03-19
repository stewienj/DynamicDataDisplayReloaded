using System.Windows;
using System.Windows.Controls;

namespace NewMarkersSample.Pages
{
	/// <summary>
	/// Interaction logic for PieChartAPIPage.xaml
	/// </summary>
	public partial class PieChartAPIPage : Page
	{
		public PieChartAPIPage()
		{
			InitializeComponent();
		}

		private void addBtn_Click(object sender, RoutedEventArgs e)
		{
			var result = editor.GetValue();
			pieChart.AddPieItem(result.Caption, result.Value, result.Fill);
		}

		private void resetBtn_Click(object sender, RoutedEventArgs e)
		{
			editor.Reset();
		}
	}
}
