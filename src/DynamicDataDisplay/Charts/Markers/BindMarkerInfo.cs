using System.Windows;

namespace DynamicDataDisplay.Charts.Markers
{
	public sealed class BindMarkerEventArgs
	{
		public BindMarkerEventArgs() { }

		public FrameworkElement Marker { get; internal set; }
		public object Data { get; internal set; }
	}
}
