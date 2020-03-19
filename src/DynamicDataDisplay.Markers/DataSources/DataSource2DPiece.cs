using System.Windows;

namespace DynamicDataDisplay.Markers.DataSources
{
	internal sealed class DataSource2DPiece<T>
	{
		public Point Position { get; set; }
		public T Data { get; set; }
	}
}
