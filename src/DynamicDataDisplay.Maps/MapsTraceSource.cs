using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Maps
{
	public sealed class MapsTraceSource
	{
		private MapsTraceSource()
		{
		}

		private static readonly MapsTraceSource instance = new MapsTraceSource();
		public static MapsTraceSource Instance
		{
			get { return instance; }
		}

		private readonly TraceSource serverInformationTraceSource = new TraceSource("D3.Maps.Server", SourceLevels.Information);
		public TraceSource ServerInformationTraceSource
		{
			get { return serverInformationTraceSource; }
		}
	}
}
