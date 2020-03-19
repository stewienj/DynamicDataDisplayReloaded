using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Markers
{
	public sealed class BindMarkerEventArgs
	{
		public BindMarkerEventArgs() { }

		public FrameworkElement Marker { get; internal set; }
		public object Data { get; internal set; }
	}
}
