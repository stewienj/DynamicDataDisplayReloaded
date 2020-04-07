using DynamicDataDisplay.Markers;
using DynamicDataDisplay.Markers.MarkerGenerators.Rendering;
using System;
using System.Collections.Generic;

namespace DynamicDataDisplay.Charts.Markers
{
	public class RenderingChart : DevMarkerChart
	{
		private List<object> data = new List<object>();
		private RenderingChartCanvas renderingCanvas = new RenderingChartCanvas();

		public RenderingChart()
		{
			//renderingCanvas.SetBinding(Viewport2D.ContentBoundsProperty, new Binding("(Viewport2D.ContentBounds)") { Source = this });
			ViewportPanel.SetViewportBounds(renderingCanvas, new DataRect(0, 0, 1, 1));
			CurrentItemsPanel.Children.Add(renderingCanvas);
		}

		protected override void OnMarkerGeneratorChanged(MarkerGenerator prevGenerator, MarkerGenerator currGenerator)
		{
			base.OnMarkerGeneratorChanged(prevGenerator, currGenerator);

			CurrentItemsPanel.Children.Add(renderingCanvas);
		}

		protected override void MarkerBuilder_OnChanged(object sender, EventArgs e)
		{
			base.MarkerBuilder_OnChanged(sender, e);

			CurrentItemsPanel.Children.Add(renderingCanvas);
			renderingCanvas.Data = data;
			renderingCanvas.Renderer = (MarkerRenderer)MarkerBuilder.CreateMarker(null);
		}

		protected override void DrawAllMarkers(bool reuseExisting, bool continueAfterDataPrepaired)
		{
			base.DrawAllMarkers(reuseExisting, continueAfterDataPrepaired);

			renderingCanvas.InvalidateVisual();
		}

		protected override void OnItemsPanelChanged()
		{
			base.OnItemsPanelChanged();

			CurrentItemsPanel.Children.Add(renderingCanvas);
		}

		protected override int CreateAndAddMarker(object dataItem, int actualChildIndex)
		{
			data.Add(dataItem);
			return 1;
		}
	}
}
