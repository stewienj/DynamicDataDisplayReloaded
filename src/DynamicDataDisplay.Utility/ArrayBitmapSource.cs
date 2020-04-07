using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DynamicDataDisplay.Utility
{
	/// <summary>
	/// Wraps a 2 dimensional array as a bitmap source
	/// </summary>  public class ArrayBitmapSource : BitmapSource
	public class ArrayBitmapSource<T> : BitmapSource
	{
		private T[,] _buffer;

		protected override Freezable CreateInstanceCore()
		{
			return new ArrayBitmapSource<uint>();
		}

		public ArrayBitmapSource() : this(new T[100, 100])
		{
		}

		public ArrayBitmapSource(T[,] buffer)
		{
			_buffer = buffer;
		}

		public T[,] Buffer => _buffer;

		public override PixelFormat Format => PixelFormats.Bgra32;
		public override int PixelHeight => _buffer.GetLength(0);
		public override int PixelWidth => _buffer.GetLength(1);

		public override double DpiX => 96;
		public override double DpiY => 96;

		public override BitmapPalette Palette => null;

		public override event EventHandler<ExceptionEventArgs> DecodeFailed;
		public void OnDecodeFailed()
		{
			DecodeFailed?.Invoke(this, null);
		}


		public override void CopyPixels(Int32Rect sourceRect, Array pixels, int stride, int offset)
		{
			if (sourceRect.Y >= PixelHeight)
				return;
			// Not applicatble because..we store just an int array, below would use width in bytes
			//int stride = width * 3 + (width * 3) % 4;
			int start = sourceRect.Y * PixelWidth;
			System.Buffer.BlockCopy(_buffer, start, pixels, 0, (_buffer.Length - start) << 2);
		}

		/// <summary>
		/// Last resort, use this if still getting an exception
		/// </summary>
		public BitmapSource BitmapSource
		{
			get
			{
				var buffer = new int[PixelWidth * PixelHeight];
				System.Buffer.BlockCopy(_buffer, 0, buffer, 0, (_buffer.Length) << 2);

				var bitmapSource = BitmapSource.Create(PixelWidth, PixelHeight, DpiX, DpiY, Format, null, buffer, PixelWidth * 4);
				bitmapSource.Freeze();
				return bitmapSource;
			}
		}

	}
}
