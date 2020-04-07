using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace DynamicDataDisplay.Common.Auxiliary
{
	public static class ScreenshotHelper
	{
		public class Parameters : INotifyPropertyChanged, IXmlSerializable
		{

			public Size GetRenderSize(UIElement uiElement)
			{
				var elementSize = uiElement.RenderSize;

				double width = this._width ?? elementSize.Width;
				double height = this._height ?? elementSize.Height;

				return new Size(width, height);

			}

			public XmlSchema GetSchema()
			{
				return null;
			}

			public void ReadXml(XmlReader reader)
			{
				int count = reader.AttributeCount;
				for (int i = 0; i < count; ++i)
				{
					reader.MoveToAttribute(i);
					switch (reader.Name)
					{
						case "DPI":
							Dpi = int.Parse(reader.GetAttribute(i));
							break;
						case "Width":
							Width = int.Parse(reader.GetAttribute(i));
							break;
						case "Height":
							Height = int.Parse(reader.GetAttribute(i));
							break;
						case "OutputDirectory":
							OutputDirectory = reader.GetAttribute(i);
							break;
					}
				}
			}

			public void WriteXml(XmlWriter writer)
			{
				writer.WriteAttributeString("DPI", Dpi.ToString());
				if (Width.HasValue)
				{
					writer.WriteAttributeString("Width", Width.Value.ToString());
				}
				if (Height.HasValue)
				{
					writer.WriteAttributeString("Height", Height.Value.ToString());
				}
				if (!string.IsNullOrWhiteSpace(OutputDirectory))
				{
					writer.WriteAttributeString("OutputDirectory", OutputDirectory);
				}
			}

			private int _dpi = 96;
			public int Dpi
			{
				get
				{
					return _dpi;
				}
				set
				{
					_dpi = value;
					PropertyChanged.Raise(this);
				}
			}

			public int ScreenDpi
			{
				get
				{
					return (int)((32 * 96) / SystemParameters.CursorWidth);
				}
			}

			private int? _width = null;
			public int? Width
			{
				get
				{
					return _width;
				}
				set
				{
					_width = value;
					PropertyChanged.Raise(this);
				}
			}

			private int? _height = null;
			public int? Height
			{
				get
				{
					return _height;
				}
				set
				{
					_height = value;
					PropertyChanged.Raise(this);
				}
			}

			private string _outputDirectory = null;
			public string OutputDirectory
			{
				get
				{
					return _outputDirectory;
				}
				set
				{
					_outputDirectory = value;
					PropertyChanged.Raise(this);
				}
			}

			public event PropertyChangedEventHandler PropertyChanged;

		}
		/// <summary>Gets the encoder by extension</summary>
		/// <param name="extension">The extension</param>
		/// <returns>BitmapEncoder object</returns>
		public static BitmapEncoder GetEncoderByExtension(string extension)
		{
			switch (extension)
			{
				case "bmp":
					return new BmpBitmapEncoder();
				case "jpg":
					return new JpegBitmapEncoder();
				case "gif":
					return new GifBitmapEncoder();
				case "png":
					return new PngBitmapEncoder();
				case "tiff":
					return new TiffBitmapEncoder();
				case "wmp":
					return new WmpBitmapEncoder();
				default:
					throw new ArgumentException(Strings.Exceptions.CannotDetermineImageTypeByExtension, "extension");
			}
		}

		/// <summary>Creates the screenshot of entire plotter element</summary>
		/// <returns></returns>
		public static async Task<BitmapSource> CreateScreenshot(UIElement uiElement, ScreenshotHelper.Parameters parameters)
		{
			Window window = Window.GetWindow(uiElement);
			if (window == null)
			{
				return CreateElementScreenshot(uiElement);
			}

			var bmp = await CreateScreenshot(uiElement, parameters.GetRenderSize(uiElement), parameters.Dpi);
			return bmp;
		}

		/// <summary>
		/// If the bitmap passed in has transparent edges, then this reduces the thickness of those edges to 1 pixel
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static async Task<BitmapSource> CropBitmapsTransparentEdges(BitmapSource source, double dpi)
		{

			int width = source.PixelWidth;
			int height = source.PixelHeight;
			int edgeThickness = (int)Math.Ceiling(dpi / 96.0);
			uint[] array = new uint[width * height];
			source.CopyPixels(array, width * 4, 0);

			// Find the min/max pixels that are not transparent and these will be the boundaries of our visible area

			int minXNonTransparent = width - 1;
			int maxXNonTransparent = 0;
			int minYNonTransparent = height - 1;
			int maxYNonTransparent = 0;

			await Task.Run(() =>
			{
				int index = 0;
				for (int y = 0; y < height; ++y)
				{
					for (int x = 0; x < width; ++x)
					{
						if (((array[index] >> 24) & 0xFF) != 0)
						{
							minXNonTransparent = Math.Min(minXNonTransparent, x);
							maxXNonTransparent = Math.Max(maxXNonTransparent, x);

							minYNonTransparent = Math.Min(minYNonTransparent, y);
							maxYNonTransparent = Math.Max(maxYNonTransparent, y);
						}
						++index;
					}
				}
			});

			// Add Edge thickness
			minXNonTransparent = Math.Max(0, minXNonTransparent - edgeThickness);
			maxXNonTransparent = Math.Min(width - 1, maxXNonTransparent + edgeThickness);
			minYNonTransparent = Math.Max(0, minYNonTransparent - edgeThickness);
			maxYNonTransparent = Math.Min(height - 1, maxYNonTransparent + edgeThickness);

			int newWidth = maxXNonTransparent - minXNonTransparent + 1;
			int newHeight = maxYNonTransparent - minYNonTransparent + 1;
			if (newWidth > 0 && newHeight > 0)
			{
				return new CroppedBitmap(source, new Int32Rect(minXNonTransparent, minYNonTransparent, newWidth, newHeight));
			}
			else
			{
				return source;
			}
		}

		/// <summary>Creates the screenshot of entire plotter element</summary>
		/// <returns></returns>
		public static async Task<BitmapSource> CreateScreenshot(UIElement uiElement, Size size, int dpi)
		{
			double dpiCoeff = dpi / 96.0;

			var elementSize = uiElement.RenderSize;
			double dpiX = size.Width / elementSize.Width * dpi;
			double dpiY = size.Height / elementSize.Height * dpi;
			RenderTargetBitmap bmp = new RenderTargetBitmap(
				(int)(size.Width * dpiCoeff), (int)(size.Height * dpiCoeff),
				dpiX, dpiY, PixelFormats.Default);

			// transparent background
			Rectangle whiteRect = new Rectangle { Width = elementSize.Width, Height = elementSize.Height, Fill = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255)) };
			whiteRect.Measure(elementSize);
			whiteRect.Arrange(new Rect(elementSize));
			bmp.Render(whiteRect);

			FrameworkElement frElement = uiElement as FrameworkElement;
			Transform layoutTransform = null;
			// So by manipulating the layout tranform and the dpi we can get this plot any size we want
			// if we want to half the size, we multiply the layout transform by 2 and set the dpi to half the current dpi
			if (frElement != null)
			{
				layoutTransform = frElement.LayoutTransform;
				frElement.LayoutTransform = new ScaleTransform(elementSize.Width / size.Width, elementSize.Height / size.Height);
			}

			await Task.Delay(3000);

			// Visual objects can have a VisualOffset value set, this appears to be related to how the uiElement would be rendered inside its
			// parent element, for some reason RenderTargetBitmap includes this offset when rendering and it creates an offset image that
			// hides part of the uiElement.  VisualOffset is also a protected member of the Visual object, so we need to get/set it via reflection...
			var offset = (Vector)uiElement.GetType().GetProperty("VisualOffset", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(uiElement);
			if (offset.LengthSquared > 0)
			{
				uiElement.GetType().GetProperty("VisualOffset", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(uiElement, new Vector());
			}

			// Give the UI Thread a chance to update the chart visuals by waiting 3 seconds and 1 update cycle of the Dispatcher
			bmp.Render(uiElement);

			// Restore proper values to the uiElement
			if (offset.LengthSquared > 0)
			{
				uiElement.GetType().GetProperty("VisualOffset", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(uiElement, offset);
			}

			if (frElement != null)
			{
				frElement.LayoutTransform = layoutTransform;
			}

			return bmp;
		}

		public static BitmapSource CreateElementScreenshot(UIElement uiElement)
		{
			bool measureValid = uiElement.IsMeasureValid;

			if (!measureValid)
			{
				double width = 300;
				double height = 300;

				FrameworkElement frElement = uiElement as FrameworkElement;
				if (frElement != null)
				{
					if (!double.IsNaN(frElement.Width))
						width = frElement.Width;
					if (!double.IsNaN(frElement.Height))
						height = frElement.Height;
				}

				Size size = new Size(width, height);
				uiElement.Measure(size);
				uiElement.Arrange(new Rect(size));
			}

			RenderTargetBitmap bmp = new RenderTargetBitmap(
				(int)uiElement.RenderSize.Width, (int)uiElement.RenderSize.Height,
				96, 96, PixelFormats.Default);

			// this is waiting for dispatcher to perform measure, arrange and render passes
			uiElement.Dispatcher.Invoke(((Action)(() => { })), DispatcherPriority.Background);

			Size elementSize = uiElement.DesiredSize;
			// white background
			Rectangle whiteRect = new Rectangle { Width = elementSize.Width, Height = elementSize.Height, Fill = Brushes.White };
			whiteRect.Measure(elementSize);
			whiteRect.Arrange(new Rect(elementSize));
			bmp.Render(whiteRect);

			bmp.Render(uiElement);

			return bmp;
		}

		private static Dictionary<BitmapSource, string> pendingBitmaps = new Dictionary<BitmapSource, string>();

		public static void SaveBitmapToStream(BitmapSource bitmap, Stream stream, string fileExtension, int dpi)
		{
			if (bitmap == null)
				throw new ArgumentNullException("bitmap");
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (string.IsNullOrEmpty(fileExtension))
				throw new ArgumentException(Strings.Exceptions.ExtensionCannotBeNullOrEmpty, fileExtension);

			// Need to reset the DPI because otherwise this renders improperly in apps that read the DPI
			// Can't change it on the source bitmap as it will upset the scaling of the source visual
			// so need to copy the rendered pixels to a new bitmap.
			byte[] pixels = new byte[bitmap.PixelWidth * bitmap.Format.BitsPerPixel / 8 * bitmap.PixelHeight];
			bitmap.CopyPixels(pixels, bitmap.PixelWidth * bitmap.Format.BitsPerPixel / 8, 0);
			bitmap = BitmapSource.Create(bitmap.PixelWidth, bitmap.PixelHeight, dpi, dpi, bitmap.Format, bitmap.Palette, pixels, bitmap.PixelWidth * bitmap.Format.BitsPerPixel / 8);

			BitmapEncoder encoder = ScreenshotHelper.GetEncoderByExtension(fileExtension);
			encoder.Frames.Add(BitmapFrame.Create(bitmap, null, new BitmapMetadata(fileExtension.Trim('.')), null));
			encoder.Save(stream);
		}

		public static void CopyBitmapToClipboard(BitmapSource bitmap, int dpi = 96)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				ScreenshotHelper.SaveBitmapToStream(bitmap, stream, "png", dpi);
				var data = new DataObject("PNG", stream);
				Clipboard.Clear();
				Clipboard.SetDataObject(data, true);
			}
		}

		public static void SaveBitmapToFile(BitmapSource bitmap, string filePath, int dpi = 96)
		{
			if (string.IsNullOrEmpty(filePath))
				throw new ArgumentException(Strings.Exceptions.FilePathCannotbeNullOrEmpty, "filePath");

			if (bitmap.IsDownloading)
			{
				pendingBitmaps[bitmap] = filePath;
				bitmap.DownloadCompleted += OnBitmapDownloadCompleted;
				return;
			}

			string dirPath = System.IO.Path.GetDirectoryName(filePath);
			if (!string.IsNullOrEmpty(dirPath) && !Directory.Exists(dirPath))
			{
				Directory.CreateDirectory(dirPath);
			}

			bool fileExistedBefore = File.Exists(filePath);
			try
			{
				using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
				{
					string extension = System.IO.Path.GetExtension(filePath).TrimStart('.');
					SaveBitmapToStream(bitmap, fs, extension, dpi);
				}
			}
			catch (ArgumentException)
			{
				if (!fileExistedBefore && File.Exists(filePath))
				{
					try
					{
						File.Delete(filePath);
					}
					catch { }
				}
			}
			catch (IOException exc)
			{
				Debug.WriteLine("Exception while saving bitmap to file: " + exc.Message);
			}
		}

		public static void SaveStreamToFile(Stream stream, string filePath)
		{
			string dirPath = System.IO.Path.GetDirectoryName(filePath);
			if (!string.IsNullOrEmpty(dirPath) && !Directory.Exists(dirPath))
			{
				Directory.CreateDirectory(dirPath);
			}

			using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
			{
				string extension = System.IO.Path.GetExtension(filePath).TrimStart('.');
				if (stream.CanSeek)
					stream.Seek(0, SeekOrigin.Begin);

				stream.CopyTo(fs);
			}

			stream.Dispose();
		}

		private static void OnBitmapDownloadCompleted(object sender, EventArgs e)
		{
			BitmapSource bmp = (BitmapSource)sender;
			bmp.DownloadCompleted -= OnBitmapDownloadCompleted;
			string filePath = pendingBitmaps[bmp];
			pendingBitmaps.Remove(bmp);

			SaveBitmapToFile(bmp, filePath, 96);
		}
	}
}