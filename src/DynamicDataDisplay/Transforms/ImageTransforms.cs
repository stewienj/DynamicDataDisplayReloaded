using System;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DynamicDataDisplay.Transforms
{
	//-------------------------------------------------------------------------
	public static class ImageTransforms
	{

		//-------------------------------------------------------------------------
		public static BitmapSource ApplyMercatorTransform(this BitmapSource src, double northDeg, double southDeg)
		{
			var tgt = new WriteableBitmap(src);

			int ySize = src.PixelHeight;
			int xSize = src.PixelWidth;
			byte[] pixels = new byte[ySize * xSize * 4];
			byte[] srcPixels = new byte[ySize * xSize * 4];
			src.CopyPixels(srcPixels, 4 * xSize, 0);

			var tForm = new MercatorTransform();
			var vpNorth = tForm.DataToViewport(new System.Windows.Point(0.0, northDeg));
			var vpSouth = tForm.DataToViewport(new System.Windows.Point(0.0, southDeg));

			Parallel.For(0, ySize, (y) =>
			{
				// calculate target pixel latitude
				var tgtLat = northDeg - (y * (northDeg - southDeg) / ySize);

				// transform the target pixel lat to a source latitude via Mercator transform
				var vp = tForm.DataToViewport(new System.Windows.Point(0.0, tgtLat));

				// translate src viewport coord to a y value
				int srcY = (int)Math.Floor(ySize * (Math.Abs(vp.Y - vpNorth.Y) / Math.Abs(vpNorth.Y - vpSouth.Y)));

				for (int x = 0; x < xSize; ++x)
				{
					int tgtOffset = (y * src.PixelWidth + x) * 4;
					int srcOffset = (srcY * src.PixelWidth + x) * 4;

					pixels[tgtOffset] = srcPixels[srcOffset];
					pixels[tgtOffset + 1] = srcPixels[srcOffset + 1];
					pixels[tgtOffset + 2] = srcPixels[srcOffset + 2];
					pixels[tgtOffset + 3] = srcPixels[srcOffset + 3];
				}
			});

			WriteableBitmap bmp = new WriteableBitmap(xSize, ySize, 96, 96, System.Windows.Media.PixelFormats.Bgra32, BitmapPalettes.WebPaletteTransparent);
			bmp.WritePixels(new System.Windows.Int32Rect(0, 0, xSize, ySize), pixels, 4 * xSize, 0);
			bmp.Freeze();

			return bmp;
		}


	}
}
