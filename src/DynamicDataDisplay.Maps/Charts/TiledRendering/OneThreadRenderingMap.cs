using System.Windows;
using System.Windows.Markup;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.TiledRendering
{
	[ContentProperty("VisualToRender")]
	public class OneThreadRenderingMap : RenderingMap
	{
		public OneThreadRenderingMap()
		{
		}

		public OneThreadRenderingMap(FrameworkElement visualToRender)
			: this()
		{
			VisualToRender = visualToRender;
		}

		public FrameworkElement VisualToRender
		{
			get { return RenderTileServer.VisualToRender; }
			set { RenderTileServer.VisualToRender = value; }
		}
	}
}