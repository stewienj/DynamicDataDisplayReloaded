using System.Windows.Controls;

namespace Microsoft.Research.DynamicDataDisplay.Samples.Demos.Custom
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
