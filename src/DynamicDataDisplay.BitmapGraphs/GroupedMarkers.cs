using Microsoft.Research.DynamicDataDisplay.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Microsoft.Research.DynamicDataDisplay.BitmapGraphs
{
	public class GroupedMarkers
	{
		/// <summary>
		/// Caches renderings of the grouped marker with the text inside.
		/// </summary>
		private GraphicsFont _graphicsFont = new GraphicsFont();

		/// <summary>
		/// Graphics font for the selection box, note that the background is transparent because the selction
		/// box is rendered independently of the text inside, unlike the markers where the whole box is cached
		/// inside the GraphicsFont object.
		/// </summary>
		private GraphicsFont _graphicsFontSelection = new GraphicsFont { BackgroundColor = 0, DrawBorder = false };

		public BitmapSource DrawImage(int imageWidth, int imageHeight, IEnumerable<(Point Point, int Count, Rect Bin)> points, RenderRequest renderRequest)
		{
			Stopwatch sw = new Stopwatch();
			try
			{
				sw.Start();
				_graphicsFont.StartBulkOperations();
				GroupedMarkersCalculationProgress?.Invoke(null, new GroupedMarkersCalculationProgressArgs { Progress = 0 });

				if (imageWidth < 1 || imageHeight < 1)
				{
					return null;
				}

				if (renderRequest.IsCancellingRequested)
					return null;

				uint[,] finalImage = CalculateParallel(imageWidth, imageHeight, points, renderRequest);

				if (renderRequest.IsCancellingRequested)
					return null;

				var groupedMarkersImage = new ArrayBitmapSource<uint>(finalImage);
				groupedMarkersImage.Freeze();

				return groupedMarkersImage;
			}
			finally
			{
				sw.Stop();
				GroupedMarkersCalculationProgress?.Invoke(null, new GroupedMarkersCalculationProgressArgs { Progress = null, CalculationTime = sw.Elapsed });
			}
		}

		private uint[,] CalculateParallel(int imageWidth, int imageHeight, IEnumerable<(System.Windows.Point Point, int Count, Rect Bin)> points, RenderRequest renderRequest)
		{
			// Note this needs to be y,x not x,y
			uint[,] pixels = new uint[imageHeight, imageWidth];
			var pointsList = points.ToList();
			int batchCount = pointsList.Count / (Environment.ProcessorCount - 1) + 1;

			var batches = pointsList.AsParallel().OrderBy(p => p.Point.X).ThenBy(p => p.Point.Y).Batch(batchCount);

			int pointsProcessedCount = 0;
			int progressIncrement = Math.Max(1, pointsList.Count / 1000);

			Func<int, int, int, int, (int, int, int, int, int, int)> GetBoundsFromSize = (x, y, spreadX, spreadY) =>
			{
		  // There's a spread size border around the image for speed as we eliminate boundary testing and branching

		  int yCoreStart = y - spreadY / 2;
				int yCoreEnd = yCoreStart + spreadY;
				int xCoreStart = x - spreadX / 2;
				int xCoreEnd = xCoreStart + spreadX;

		  // Work out the source start indicies, may be non zero if we have to modify
		  // the start indicies to be within the dest image, see further down.
		  int fontStartY = Math.Max(0, yCoreStart) - yCoreStart;
				int fontStartX = Math.Max(0, xCoreStart) - xCoreStart;

		  // Limit the x,y to within the destination image
		  yCoreStart = Math.Max(0, yCoreStart);
				yCoreEnd = Math.Min(imageHeight, yCoreEnd);
				xCoreStart = Math.Max(0, xCoreStart);
				xCoreEnd = Math.Min(imageWidth, xCoreEnd);

				return (xCoreStart, xCoreEnd, yCoreStart, yCoreEnd, fontStartX, fontStartY);
			};

			Func<Point, uint[,], (int, int, int, int, int, int)> GetBoundsFromLabel = (point, label) => GetBoundsFromSize((int)(point.X * imageWidth), (int)(point.Y * imageHeight), label.GetLength(1), label.GetLength(0));

			Parallel.ForEach(batches, (batch, state, index) =>
			{
		  // Note the access to the pixels and alphaMap below isn't thread safe. There's
		  // minor artifacting but it's not visible to the user, so I left the code like this.
		  // If thread safe is required the separately compute to 4 or 8 different arrays, and
		  // then aggregate them into one.
		  foreach (var point in batch)
				{
					if (renderRequest.IsCancellingRequested)
						break;

					var label = _graphicsFont.GetBlock(point.Count.ToString());

					(int xCoreStart, int xCoreEnd, int yCoreStart, int yCoreEnd, int fontStartX, int fontStartY) = GetBoundsFromLabel(point.Point, label);

					int fontY = fontStartY;
					for (int yCore = yCoreStart; yCore < yCoreEnd; ++yCore)
					{
						int fontX = fontStartX;
						for (int xCore = xCoreStart; xCore < xCoreEnd; ++xCore)
						{
							pixels[yCore, xCore] = label[fontY, fontX];
							fontX++;
						}
						fontY++;
					}

					var progress = ++pointsProcessedCount;
					if ((progress % progressIncrement) == 0)
					{
						GroupedMarkersCalculationProgress?.Invoke(null, new GroupedMarkersCalculationProgressArgs { Progress = (double)progress / pointsList.Count });
					}
				}
			});

			// Now render the selection box over the top of the markers, transparently, and render the text again over the top
			if (LastSelection?.SelectedPoints != null)
			{
				uint selectionAlpha = 0xC0000000;

				// This code draws a yellow box that is a least big enough to hold the text that shows the count
				// of selected items, but it also covers all of the selected points, so if you zoom in, the box
				// gets bigger. It might also be bigger than the selected box because some of the selected items
				// may lie outside of the box that shows how many there are.

				// Note the last selection is in data coordinates, Y,X
				var label = _graphicsFontSelection.GetBlock(LastSelection.SelectedPoints.Count.ToString());
				var labelWidth = label.GetLength(1);
				var labelHeight = label.GetLength(0);

				// Cache the conversion of all the points
				var convertedPoints = LastSelection.ConvertedSelectedPoints.AsParallel().Select(p => Transform.DataToScreen(p)).ToList();

				// Selection Location is is lat lon
				var location = LastSelection.SelectionLocation;
				var screenPos = Transform.DataToScreen(location);

				// Get the data bounds box and then merge it with the text bounds box
				var minDataX = convertedPoints.AsParallel().Min(p => p.X);
				var maxDataX = convertedPoints.AsParallel().Max(p => p.X);
				var minDataY = convertedPoints.AsParallel().Min(p => p.Y);
				var maxDataY = convertedPoints.AsParallel().Max(p => p.Y);

				// Merging with the text bounds box, add a slight border to selection if it is label size
				minDataX = Math.Min(minDataX, screenPos.X - labelWidth * 0.5 - 0.5);
				maxDataX = Math.Max(maxDataX, screenPos.X + labelWidth * 0.5);
				minDataY = Math.Min(minDataY, screenPos.Y - labelHeight * 0.5 - 0.5);
				maxDataY = Math.Max(maxDataY, screenPos.Y + labelHeight * 0.5);

				// Limit the x,y to within the destination image
				int xCoreStart = (int)Math.Round(Math.Max(0, minDataX));
				int xCoreEnd = (int)Math.Round(Math.Min(imageWidth, maxDataX));
				int yCoreStart = (int)Math.Round(Math.Max(0, minDataY));
				int yCoreEnd = (int)Math.Round(Math.Min(imageHeight, maxDataY));

				// Render the box
				for (int yCore = yCoreStart; yCore < yCoreEnd; ++yCore)
				{
					for (int xCore = xCoreStart; xCore < xCoreEnd; ++xCore)
					{
						uint color = pixels[yCore, xCore];
						uint red = (((color & 0xFF0000) + 0xFF0000 * 3) >> 2) & 0xFF0000;
						uint green = (((color & 0xFF00) + 0xFF00 * 3) >> 2) & 0xFF00;
						uint blue = (color & 0xFF) >> 2;

						pixels[yCore, xCore] = selectionAlpha | red | green | blue;
					}
				}

				// Render the text over the top of the markers with the selection box over the top
				int xCoreWidth = xCoreEnd - xCoreStart;
				int yCoreHeight = yCoreEnd - yCoreStart;
				int labelStartX = labelWidth - Math.Min(xCoreWidth, labelWidth);
				int labelStartY = labelHeight - Math.Min(yCoreHeight, labelHeight);

				int xOffset = (int)((xCoreWidth - (labelWidth - labelStartX)) * 0.5) + xCoreStart;
				int yOffset = (int)((yCoreHeight - (labelHeight - labelStartY)) * 0.5) + yCoreStart;

				for (int labelY = labelStartY; labelY < labelHeight; ++labelY)
				{
					for (int labelX = labelStartX; labelX < labelWidth; ++labelX)
					{
						uint color = pixels[labelY + yOffset, labelX + xOffset];
						uint labelColor = label[labelY, labelX];

						double alpha = (labelColor >> 24) / 255.0;
						if (alpha > 0)
						{
							uint red = (uint)((color & 0xFF0000) * (1.0 - alpha) + (labelColor & 0xFF0000) * alpha) & 0xFF0000;
							uint green = (uint)((color & 0xFF00) * (1.0 - alpha) + (labelColor & 0xFF00) * alpha) & 0xFF00;
							uint blue = (uint)((color & 0xFF) * (1.0 - alpha) + (labelColor & 0xFF) * alpha) & 0xFF;

							pixels[labelY + yOffset, labelX + xOffset] = selectionAlpha | red | green | blue;
						}
					}
				}
			}

			return pixels;
		}

		public static EventHandler<GroupedMarkersCalculationProgressArgs> GroupedMarkersCalculationProgress;

		public SelectedPointsChangedArgs LastSelection { get; internal set; } = null;

		public CoordinateTransform Transform { get; internal set; }
	}
}
