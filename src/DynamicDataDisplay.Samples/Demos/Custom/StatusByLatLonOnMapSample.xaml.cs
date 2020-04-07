using System.Windows.Controls;

namespace DynamicDataDisplay.Samples.Demos.Custom
{
	/// <summary>
	/// Interaction logic for StatusByLatLonOnMapSample.xaml
	/// </summary>
	public partial class StatusByLatLonOnMapSample : Page
	{
		public StatusByLatLonOnMapSample()
		{
			InitializeComponent();
			latLonStatus.CreateTestBitmap();
		}
	}
}
