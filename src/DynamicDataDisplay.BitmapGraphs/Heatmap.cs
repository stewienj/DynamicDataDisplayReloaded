using DynamicDataDisplay.Common.Auxiliary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DynamicDataDisplay.BitmapGraphs
{
	public class Heatmap
	{
		public class HeatmapCalculationProgressArgs : EventArgs
		{
			public double? Progress { get; set; }

			public TimeSpan CalculationTime { get; set; }
		}

		public static EventHandler<HeatmapCalculationProgressArgs> HeatmapCalculationProgress;

		public static ArrayBitmapSource<uint> DrawImage(int imageWidth, int imageHeight, int spread, IEnumerable<(System.Windows.Point Point, int Count)> points, uint[] colorMap, RenderRequest renderRequest)
		{
			Stopwatch sw = new Stopwatch();
			try
			{
				sw.Start();
				HeatmapCalculationProgress?.Invoke(null, new HeatmapCalculationProgressArgs { Progress = 0 });

				if (imageWidth < 1 || imageHeight < 1)
				{
					return null;
				}

				int kernelBreadth = spread * 2 + 1;
				float[] colorkernel = new float[kernelBreadth * kernelBreadth];
				uint[] alphaKernel = new uint[kernelBreadth * kernelBreadth];

				float centreMultiplier = -100000 * spread;

				float alphaBias = 65536.0f / (spread + 1);
				for (int y = -spread; y <= spread; ++y)
				{
					// Add the width * y + the x offset
					int offset = (spread + y) * kernelBreadth + spread;
					for (int x = -spread; x <= spread; ++x)
					{
						float length = (float)Math.Sqrt(x * x + y * y);
						float multiplier = Math.Max(0, spread + 1 - length);
						if (length > 1)
						{
							alphaKernel[offset + x] = (uint)(multiplier * alphaBias);
						}
						else
						{
							alphaKernel[offset + x] = 128000U;
						}
						colorkernel[offset + x] = multiplier;
					}
				}

				if (renderRequest.IsCancellingRequested)
					return null;


				(uint[] alphaMap, float[] pixels) = CalculateParallelVector(imageWidth, imageHeight, spread, points, renderRequest, colorkernel, alphaKernel);

				if (renderRequest.IsCancellingRequested)
					return null;

				uint[,] finalImage = NormalizeImage(imageWidth, imageHeight, spread, colorMap, alphaMap, pixels);
				var HeatmapImage = new ArrayBitmapSource<uint>(finalImage);
				HeatmapImage.Freeze();

				return HeatmapImage;
			}
			finally
			{
				sw.Stop();
				HeatmapCalculationProgress?.Invoke(null, new HeatmapCalculationProgressArgs { Progress = null, CalculationTime = sw.Elapsed });
			}
		}

		private static uint[,] NormalizeImage(int imageWidth, int imageHeight, int spread, uint[] colorMap, uint[] alphaMap, float[] pixels)
		{
			int sourceWidth = (int)imageWidth + spread * 2;
			float max = Math.Max(1, pixels.AsParallel().Max());

			float alphaMultiplier = (float)(127.0 / (Math.Pow(2, 32) - 1));
			uint[,] finalImage = new uint[imageHeight, imageWidth];
			Parallel.For(0, imageHeight, y =>
			{
				int offset = (y + spread) * sourceWidth + spread;
				for (int x = 0; x < imageWidth; ++x)
				{
					float pixel = pixels[offset + x];
					if (pixel != 0)
					{
						uint rawAlpha = alphaMap[offset + x];
						uint colorMapIndex = (uint)(((colorMap.Length - 1) * pixel) / max);
						if (rawAlpha < 65537)
						{
							uint alpha = ((alphaMap[offset + x] * 224U) / 65536U + 31U);
							finalImage[y, x] = alpha << 24 | (colorMap[colorMapIndex] & 0xFFFFFF);
						}
						else
						{
							var color = colorMap[colorMapIndex];
							float bias = 0.7f;
							var r = (uint)(((color & 0xFF0000) + 0xFF0000 * bias) / (bias + 1)) & 0xFF0000;
							var g = (uint)(((color & 0xFF00) + 0xFF00 * bias) / (bias + 1)) & 0xFF00;
							var b = (uint)(((color & 0xFF) + 0xFF * bias) / (bias + 1)) & 0xFF;
							finalImage[y, x] = 0xFF000000 | r | g | b;
						}
					}
				}
			});

			return finalImage;
		}

		private static (uint[] alphaMap, float[] pixels) CalculateParallel(int imageWidth, int imageHeight, int spread, IEnumerable<System.Windows.Point> points, RenderRequest renderRequest, float[] colorKernel, uint[] alphaKernel)
		{
			// Put in some borders the size of the spread so we don't cross over outside the bounds
			int bufferWidth = imageWidth + spread * 2;
			int bufferHeight = imageHeight + spread * 2;

			uint[] alphaMap = new uint[bufferHeight * bufferWidth];
			float[] pixels = new float[bufferHeight * bufferWidth];


			var pointsList = points.ToList();
			int batchCount = pointsList.Count / (Environment.ProcessorCount - 1) + 1;

			double minDistX = (spread * 2 + 1) / imageWidth;
			double minDistY = (spread * 2 + 1) / imageHeight;

			Func<System.Windows.Point, List<System.Windows.Point>, bool> goToNextBatch = (p, lp) =>
			{
				int lastEntry = lp.Count - 1;
				var first = lp[0];
				var last = lp[lastEntry];
				return
			((p.X - first.X > minDistX) || (p.Y - first.Y > minDistY)) &&
			((p.X - last.X > minDistX) || (p.Y - last.Y > minDistY));
			};

			var batches = pointsList.AsParallel().OrderBy(p => p.X).ThenBy(p => p.Y).BatchConditionalSplit(batchCount, goToNextBatch);

			int pointsProcessedCount = 0;
			int progressIncrement = Math.Max(1, pointsList.Count / 1000);

			int kernelBreadth = spread * 2 + 1;

			Parallel.ForEach(batches, batch =>
			{
		  // Note the access to the pixels and alphaMap below isn't thread safe. There's
		  // minor artifacting but it's not visible to the user, so I left the code like this.
		  // If thread safe is required the separately compute to 4 or 8 different arrays, and
		  // then aggregate them into one.
		  foreach (var point in batch)
				{
					if (renderRequest.IsCancellingRequested)
						break;

			  // There's a spread size border around the image for speed as we eliminate boundary testing and branching
			  int x = (int)(point.X * imageWidth);
					int y = (int)(point.Y * imageHeight);
					int imageOffset = y * bufferWidth + x;
					int kernelOffset = 0;
					for (int dy = 0; dy < kernelBreadth; ++dy)
					{
						for (int dx = 0; dx < kernelBreadth; ++dx)
						{
							alphaMap[imageOffset] = Math.Max(alphaMap[imageOffset], alphaKernel[kernelOffset]);
							pixels[imageOffset] += colorKernel[kernelOffset];
							imageOffset++;
							kernelOffset++;
						}
						imageOffset += bufferWidth - kernelBreadth;
					}

					var progress = ++pointsProcessedCount;
					if ((progress % progressIncrement) == 0)
					{
						HeatmapCalculationProgress?.Invoke(null, new HeatmapCalculationProgressArgs { Progress = (double)progress / pointsList.Count });
					}
				}
			});

			return (alphaMap, pixels);
		}

		private static (uint[] alphaMap, float[] pixels) CalculateParallelVector(int imageWidth, int imageHeight, int spread, IEnumerable<(System.Windows.Point Point, int Count)> points, RenderRequest renderRequest, float[] colorKernel, uint[] alphaKernel)
		{
			int kernelBreadth = spread * 2 + 1;

			// This is how many copies of the kernel we have to make, each one shifted by 1 float
			int vectorLength = Vector<float>.Count;

			// Need the extra padding to accomodate the shifting of the kernel to align with vector boundaries
			int extraPadding = vectorLength - 1;

			// Need to pad out each row with blanks at the beginning and end so we can copy it into vectors

			float[][] tempColorKernel = new float[kernelBreadth][];
			uint[][] tempAlphaKernel = new uint[kernelBreadth][];
			for (int k = 0; k < kernelBreadth; ++k)
			{
				// Add some padding each side so we can break it into a vectorized array
				tempColorKernel[k] = new float[kernelBreadth + vectorLength * 2 - 2 + extraPadding];
				tempAlphaKernel[k] = new uint[kernelBreadth + vectorLength * 2 - 2 + extraPadding];
				Array.Copy(colorKernel, k * kernelBreadth, tempColorKernel[k], vectorLength - 1, kernelBreadth);
				Array.Copy(alphaKernel, k * kernelBreadth, tempAlphaKernel[k], vectorLength - 1, kernelBreadth);
			}

			int vectorKernelCount = vectorLength;
			// Divide kernelBreadth by vector length, but round up
			int vectorKernelWidth = (kernelBreadth + vectorLength - 1 + extraPadding) / vectorLength;
			int vectorKernelHeight = kernelBreadth;

			var vectorColorKernels = new List<Vector<float>[]>();
			var vectorAlphaKernels = new List<Vector<uint>[]>();
			for (int vectorOffset = 0; vectorOffset < vectorKernelCount; ++vectorOffset)
			{
				var vectorColorKernel = new Vector<float>[vectorKernelWidth * vectorKernelHeight];
				var vectorAlphaKernel = new Vector<uint>[vectorKernelWidth * vectorKernelHeight];
				for (int y = 0; y < vectorKernelHeight; ++y)
				{
					int vectorIndex = y * vectorKernelWidth;
					int kernelIndex = vectorLength - vectorOffset - 1;
					for (int x = 0; x < vectorKernelWidth; ++x)
					{
						vectorColorKernel[vectorIndex] = new Vector<float>(tempColorKernel[y], kernelIndex);
						vectorAlphaKernel[vectorIndex] = new Vector<uint>(tempAlphaKernel[y], kernelIndex);
						vectorIndex++;
						kernelIndex += vectorLength;
					}
				}
				vectorColorKernels.Add(vectorColorKernel);
				vectorAlphaKernels.Add(vectorAlphaKernel);
			}


			// Put in some borders the size of the spread so we don't cross over outside the bounds
			int bufferWidth = (imageWidth + spread * 2 + vectorLength - 1) / vectorLength;
			int bufferHeight = imageHeight + spread * 2;

			// TODO: Work out why we are overflowing these arrays, started when we merged to common point aggregator for heatmap and groupedmarkers
			// Sometimes a rounding error causes overflow, so make the arrays slightly bigger
			var alphaMap = new Vector<uint>[(bufferHeight + 1) * (bufferWidth + 1)];
			var pixels = new Vector<float>[(bufferHeight + 1) * (bufferWidth + 1)];

			var pointsList = points.ToList();
			int batchCount = pointsList.Count / (Environment.ProcessorCount - 1) + 1;

			double minDistX = (double)kernelBreadth / (double)imageWidth;
			double minDistY = (double)kernelBreadth / (double)imageHeight;

			Func<(System.Windows.Point Point, int Count), List<(System.Windows.Point Point, int Count)>, bool> goToNextBatch = (p, lp) =>
			{
				int lastEntry = lp.Count - 1;
				var first = lp[0];
				var last = lp[lastEntry];
				return
			((p.Point.X - first.Point.X > minDistX) || (p.Point.Y - first.Point.Y > minDistY)) &&
			((p.Point.X - last.Point.X > minDistX) || (p.Point.Y - last.Point.Y > minDistY));
			};

			var batches = pointsList.AsParallel().OrderBy(p => p.Point.X).ThenBy(p => p.Point.Y).BatchConditionalSplit(batchCount, goToNextBatch);

			int pointsProcessedCount = 0;
			int progressIncrement = Math.Max(1, pointsList.Count / 1000);

			Parallel.ForEach(batches, batch =>
			{
		  // Note the access to the pixels and alphaMap below isn't thread safe. There's
		  // minor artifacting but it's not visible to the user, so I left the code like this.
		  // If thread safe is required the separately compute to 4 or 8 different arrays, and
		  // then aggregate them into one.
		  foreach (var point in batch)
				{
					if (renderRequest.IsCancellingRequested)
						break;

			  // There's a spread size border around the image for speed
			  int xBase = (int)(point.Point.X * imageWidth);
					int y = (int)(point.Point.Y * imageHeight);

					int kernelNo = xBase % vectorLength;
					var vectorAlphaKernel = vectorAlphaKernels[kernelNo];
					var vectorColorKernel = vectorColorKernels[kernelNo];

					int imageOffset = y * bufferWidth + xBase / vectorLength;
					int kernelOffset = 0;
					for (int dy = 0; dy < vectorKernelHeight; ++dy)
					{
						for (int dx = 0; dx < vectorKernelWidth; ++dx)
						{
							alphaMap[imageOffset] = Vector.Max(alphaMap[imageOffset], vectorAlphaKernel[kernelOffset]);
							pixels[imageOffset] += vectorColorKernel[kernelOffset] * point.Count;
							imageOffset++;
							kernelOffset++;
						}
						imageOffset += bufferWidth - vectorKernelWidth;
					}

					var progress = ++pointsProcessedCount;
					if ((progress % progressIncrement) == 0)
					{
						HeatmapCalculationProgress?.Invoke(null, new HeatmapCalculationProgressArgs { Progress = (double)progress / pointsList.Count });
					}
				}
			});

			int outBufferWidth = bufferWidth * vectorLength + 1;
			var alphaMapOut = new uint[bufferHeight * outBufferWidth];
			var pixelsOut = new float[bufferHeight * outBufferWidth];

			for (int y = 0; y < bufferHeight; ++y)
			{
				for (int x = 0; x < bufferWidth; ++x)
				{
					var index = y * bufferWidth + x;
					var indexOut = y * (imageWidth + spread * 2) + x * vectorLength;
					pixels[index].CopyTo(pixelsOut, indexOut);
					alphaMap[index].CopyTo(alphaMapOut, indexOut);
				}
			}


			return (alphaMapOut, pixelsOut);
		}
	}
}
