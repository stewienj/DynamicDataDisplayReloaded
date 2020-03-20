using System.Windows.Controls;

namespace Microsoft.Research.DynamicDataDisplay.Samples.Demos.Custom
{
	/// <summary>
	/// Interaction logic for StatusByLatLonOnMapSample.xaml
	/// </summary>
	public partial class StatusByLatLonOnCoastLineSample : Page
	{
		public StatusByLatLonOnCoastLineSample()
		{
			InitializeComponent();
			latLonStatus.CreateTestBitmap();
		}
	}
}
