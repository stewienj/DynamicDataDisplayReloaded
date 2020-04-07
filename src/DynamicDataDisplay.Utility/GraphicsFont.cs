using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;

namespace DynamicDataDisplay.Utility
{
	/// <summary>
	/// Creates a big texture square with all the usable characters rendered to it in a grid.
	/// That was the original intention. That code path isn't used at the moment due to an issue
	/// with character spacing.
	/// </summary>
	public class GraphicsFont
	{
		private const int MIN_TEXTURE_SIZE = 0x80;

		private string _glyphs = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~⌂\x00a9";

		private Font _font = new Font("Calibri", 14, GraphicsUnit.Pixel);
		private StringFormat _stringFormat = StringFormat.GenericDefault;
		private int _glowSize = 0;
		private int[] _texture;
		private int _textureSize;

		private ConcurrentDictionary<string, uint[,]> _textToBlockPrevious = new ConcurrentDictionary<string, uint[,]>();
		private ConcurrentDictionary<string, uint[,]> _textToBlockCurrent = new ConcurrentDictionary<string, uint[,]>();

		public GraphicsFont()
		{
			//Initialize();
		}

		/*
		public GraphicsFont(int fontColor, int backColor)
		{
		  _fontColor = fontColor;
		  BackgroundColor = backColor;
		  Initialize();
		}


		public GraphicsFont(System.Drawing.Font font)
		{
		  _font = font;
		  Initialize();
		}

		public GraphicsFont(System.Drawing.Font font, int fontColor)
		{
		  _font = font;
		  _fontColor = fontColor;
		  Initialize();
		}

		public GraphicsFont(System.Drawing.Font font, int fontColor, int backColor)
		{
		  _font = font;
		  _fontColor = fontColor;
		  BackgroundColor = backColor;
		  Initialize();
		}

		public GraphicsFont(System.Drawing.Font font, int fontColor, int backColor, int glowSize)
		{
		  _font = font;
		  _fontColor = fontColor;
		  BackgroundColor = backColor;
		  _glowSize = glowSize;
		  Initialize();
		}

		public GraphicsFont(System.Drawing.Font font, string glyphs)
		{
		  _font = font;
		  _glyphs = glyphs;
		  Initialize();
		}

		public GraphicsFont(System.Drawing.Font font, int fontColor, int? backColor, string glyphs, int glowSize)
		{
		  _font = font;
		  _fontColor = fontColor;
		  BackgroundColor = backColor ?? 0;
		  _glowSize = glowSize;
		  Initialize();
		}
		*/

		public void StartBulkOperations()
		{
			_textToBlockPrevious = _textToBlockCurrent;
			_textToBlockCurrent = new ConcurrentDictionary<string, uint[,]>();
		}

		/// <summary>
		/// Gets a block containing the text, note the block is in coordinates Y,X
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public uint[,] GetBlock(string text)
		{
			if (!_textToBlockCurrent.TryGetValue(text, out var block))
			{
				if (!_textToBlockPrevious.TryGetValue(text, out block))
				{
					block = CreateNewBlockFullRedraw(text);
					_textToBlockCurrent.TryAdd(text, block);
				}
			}
			return block;
		}

		public uint[,] CreateNewBlockFullRedraw(string text)
		{
			// Note you can calculate the height required
			// There are approximately 72.272 font points per inch
			// So height is 96.0/72.272 * point size

			// Lets say width is approximately 1.5 * char count * height

			float heightF = _font.Size * 1.5f;
			float widthF = heightF * text.Length;
			int width = (int)Math.Ceiling(widthF) + 1;
			int height = (int)Math.Ceiling(heightF) + 1;

			Font font = (Font)_font.Clone();
			Brush fontBrush = new SolidBrush(Color.FromArgb(FontColor));
			Brush backgroundBrush = new SolidBrush(Color.FromArgb(unchecked((int)BackgroundColor)));
			Pen backgroundPen = new Pen(Color.DarkGreen);

			using (Bitmap image = new Bitmap(width, height, PixelFormat.Format32bppArgb))
			{
				using (Graphics graphics = Graphics.FromImage(image))
				{
					graphics.SmoothingMode = SmoothingMode.HighQuality;
					graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
					graphics.PageUnit = GraphicsUnit.Pixel;
					try
					{
						var textSize = graphics.MeasureString(text, font, width, _stringFormat);
						int textWidth = (int)Math.Ceiling(textSize.Width);
						int textHeight = (int)Math.Ceiling(textSize.Height);
						graphics.FillRectangle(backgroundBrush, 0, 0, textWidth, textHeight);
						if (DrawBorder)
						{
							graphics.DrawRectangle(backgroundPen, 0, 0, textWidth - 1, textHeight - 1);
						}
						graphics.DrawString(text, font, fontBrush, 0f, 0f, _stringFormat);
						return Array2FromBitmap(image, textWidth, textHeight);
					}
					catch (ArgumentException)
					{
						return null;
					}
					finally
					{
						font.Dispose();
						fontBrush.Dispose();
						backgroundBrush.Dispose();
						backgroundPen.Dispose();
					}
				}
			}
		}

		public System.Windows.Media.Imaging.BitmapSource CreateBitmapSource()
		{
			if (_texture == null)
			{
				Initialize();
			}

			var bitmapSource = System.Windows.Media.Imaging.BitmapSource.Create(
			  _textureSize, _textureSize,
			  96, 96,
			  System.Windows.Media.PixelFormats.Bgra32,
			  null,
			  _texture,
			  _textureSize * 4);

			bitmapSource.Freeze();
			return bitmapSource;
		}

		private int[,] CreateNewBlockFromGrid(string text)
		{
			var blockSpecs = text.Select(c => GetCharInfo(c)).Where(c => c != null).ToList();
			if (blockSpecs.Count == 0)
			{
				return new int[0, 0];
			}

			int height = blockSpecs.Select(bs => bs.Height).Max();
			int width = blockSpecs.Select(bs => bs.Width).Sum();
			var block = new int[height, width];

			// xOffset is the offset of the character in the output block
			int xOffset = 0;
			foreach (var blockSpec in blockSpecs)
			{
				for (int y = 0; y < height; ++y)
				{
					int textureOffset = (blockSpec.Y + y) * _textureSize + blockSpec.X;
					for (int x = 0; x < blockSpec.Width; ++x)
					{
						block[y, x + xOffset] = _texture[textureOffset];
						textureOffset++;
					}
				}
				// Move the offset along for the next character
				xOffset += blockSpec.Width;
			}
			return block;
		}

		/// <summary>
		/// Draws the glow as the background for the letter on top
		/// </summary>
		/// <param name="g"></param>
		/// <param name="glyph"></param>
		/// <param name="fontToUse"></param>
		/// <param name="brush"></param>
		/// <param name="posX"></param>
		/// <param name="posY"></param>
		/// <param name="glowSize"></param>
		private void DrawGlow(Graphics g, string glyph, System.Drawing.Font fontToUse, Brush brush, int posX, int posY, int glowSize)
		{
			for (double i = glowSize; i > 0.10000000149011612; i--)
			{
				int num2 = (int)((i * 2.0) * 3.1415926535897931);
				double num3 = 6.2831853071795862 / ((double)num2);
				for (int j = 0; j < num2; j++)
				{
					float num5 = (float)(Math.Sin(j * num3) * i);
					float num6 = (float)(Math.Cos(j * num3) * i);
					g.DrawString(glyph, fontToUse, brush, (float)(posX + num6), (float)(posY + num5), _stringFormat);
				}
			}
		}

		/// <summary>
		/// This measures the rendered size of each character in the glyphs passed in. Note that they need to be
		/// measured before they are rendered so as we know the size of the overall texture square to create.
		/// </summary>
		/// <param name="font"></param>
		/// <param name="glyphs"></param>
		/// <param name="glowSize"></param>
		/// <returns></returns>
		private SizeF[] MeasureEachCharSize(System.Drawing.Font font, string glyphs, int glowSize)
		{
			SizeF[] charSizeArray = new SizeF[glyphs.Length];

			using (Bitmap image = new Bitmap(MIN_TEXTURE_SIZE, MIN_TEXTURE_SIZE, PixelFormat.Format32bppArgb))
			{
				using (Graphics graphics = Graphics.FromImage(image))
				{
					graphics.SmoothingMode = SmoothingMode.HighQuality;
					graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
					graphics.PageUnit = GraphicsUnit.Pixel;
					try
					{
						for (int i = 0; i < glyphs.Length; i++)
						{
							charSizeArray[i] = graphics.MeasureString(glyphs[i].ToString(), font, 0x400, _stringFormat);
							charSizeArray[i] = new SizeF(charSizeArray[i].Width + (glowSize * 2), charSizeArray[i].Height + (glowSize * 2));
						}
					}
					catch (ArgumentException)
					{
						return null;
					}
				}

			}
			return charSizeArray;
		}

		private RectangleF[] MeasureEachCharRect(System.Drawing.Font font, string glyphs, int glowSize)
		{
			RectangleF[] charSizeArray = new RectangleF[glyphs.Length];
			try
			{
				var xlate = new Matrix();
				xlate.Translate(0, 0);
				var point = new PointF(0, 0);
				Pen pen = new Pen(Color.Black, 1);

				for (int i = 0; i < glyphs.Length; i++)
				{
					GraphicsPath path = new GraphicsPath();
					path.AddString(glyphs[i].ToString(), font.FontFamily, (int)font.Style, font.SizeInPoints, point, _stringFormat);
					charSizeArray[i] = path.GetBounds(xlate, pen);
					// TODO add glow size
					// charSizeArray[i] = new SizeF(charSizeArray[i].Width + (glowSize * 2), charSizeArray[i].Height + (glowSize * 2));

					// Should pair the above with this method of drawing:

					//graphics.DrawPath(Pens.Black, path);
					//bounds = path.GetBounds(); // is this different to what we already calculated?
				}
			}
			catch (ArgumentException)
			{
				return null;
			}
			return charSizeArray;
		}


		internal CharacterSlot GetCharInfo(char c)
		{
			if (Characters.ContainsKey(c))
			{
				return Characters[c];
			}
			return null;
		}

		private int[] ArrayFromBitmap(Bitmap image)
		{
			BitmapData bitmapdata = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
			int[] array = new int[image.Width * image.Height];
			Marshal.Copy(bitmapdata.Scan0, array, 0, array.Length);
			image.UnlockBits(bitmapdata);
			return array;
		}

		private uint[,] Array2FromBitmap(Bitmap image, int destWidth, int destHeight)
		{
			int[] temp = ArrayFromBitmap(image);
			uint[,] array = new uint[destHeight, destWidth];
			unchecked
			{
				for (int y = 0; y < destHeight; ++y)
				{
					int srcIndex = y * image.Width;
					for (int x = 0; x < destWidth; ++x)
					{
						array[y, x] = (uint)temp[srcIndex];
						srcIndex++;
					}
				}
			}

			return array;
		}

		private void Initialize()
		{
			var fontBrush = new SolidBrush(Color.FromArgb(FontColor));
			var backgroundBrush = new SolidBrush(Color.FromArgb(unchecked((int)BackgroundColor)));

			int gridSize = (int)Math.Sqrt((double)_glyphs.Length);
			while ((gridSize * gridSize) < _glyphs.Length)
			{
				gridSize++;
			}

			SizeF[] charBigArray = MeasureEachCharSize(_font, _glyphs, _glowSize);
			RectangleF[] charSizeArray = MeasureEachCharRect(_font, _glyphs, _glowSize);
			if (charSizeArray == null || charSizeArray.Length == 0)
			{
				return;
			}

			MaxWidth = charBigArray.Max(s => s.Width);
			MaxHeight = charBigArray.Max(s => s.Height);
			var maxCellSize = Math.Max(MaxWidth, MaxHeight);
			var textureSize = MIN_TEXTURE_SIZE;
			int gridCellSize = textureSize / gridSize;
			while ((maxCellSize > gridCellSize) && (textureSize < 0x400))
			{
				textureSize *= 2;
				gridCellSize = textureSize / gridSize;
			}
			textureSize = gridCellSize * gridSize;
			Brush brush = null;
			if (_glowSize > 0)
			{
				brush = new SolidBrush(System.Drawing.Color.FromArgb((int)(128.0 / ((double)_glowSize)), System.Drawing.Color.White));
			}
			_textureSize = textureSize;
			using (Bitmap image = new Bitmap(textureSize, textureSize, PixelFormat.Format32bppArgb))
			{
				using (Graphics graphics2 = Graphics.FromImage(image))
				{
					if (backgroundBrush != null)
					{
						graphics2.FillRectangle(backgroundBrush, 0, 0, textureSize, textureSize);
					}

					graphics2.SmoothingMode = SmoothingMode.HighQuality;
					graphics2.TextRenderingHint = TextRenderingHint.AntiAlias;
					int index = 0;
					for (int barePosY = 0; barePosY < textureSize; barePosY += gridCellSize)
					{
						for (int barePosX = 0; barePosX < textureSize; barePosX += gridCellSize)
						{
							if (index >= _glyphs.Length)
							{
								break;
							}
							int posX = barePosX + _glowSize;
							int posY = barePosY + _glowSize;
							string s = _glyphs[index].ToString();
							if (_glowSize > 0)
							{
								if (_glowSize <= 2)
								{
									for (float n = _glowSize; n > 0.1f; n--)
									{
										graphics2.DrawString(s, _font, brush, (float)(posX + n), (float)(posY + n), _stringFormat);
										graphics2.DrawString(s, _font, brush, posX + n, (float)posY, _stringFormat);
										graphics2.DrawString(s, _font, brush, (float)(posX + n), (float)(posY - n), _stringFormat);
										graphics2.DrawString(s, _font, brush, (float)(posX - n), (float)(posY + n), _stringFormat);
										graphics2.DrawString(s, _font, brush, posX - n, (float)posY, _stringFormat);
										graphics2.DrawString(s, _font, brush, (float)(posX - n), (float)(posY - n), _stringFormat);
										graphics2.DrawString(s, _font, brush, (float)posX, posY + n, _stringFormat);
										graphics2.DrawString(s, _font, brush, (float)posX, posY - n, _stringFormat);
									}
								}
								else
								{
									DrawGlow(graphics2, s, _font, brush, posX, posY, _glowSize);
								}
							}
							else
							{
								graphics2.DrawString(s, _font, fontBrush, (float)posX, (float)posY, _stringFormat);
							}
							CharacterSlot slot = new CharacterSlot(barePosX, barePosY, (int)Math.Ceiling(MaxHeight), charSizeArray[index]);
							Characters.Add(_glyphs[index], slot);
							index++;
						}
					}
				}
				_texture = ArrayFromBitmap(image);
			}
			brush?.Dispose();
		}

		public bool DrawBorder { get; set; } = true;

		public float MaxHeight { get; private set; }

		public float MaxWidth { get; private set; }

		public uint BackgroundColor { get; set; } = unchecked((uint)Color.LightGreen.ToArgb());

		public Dictionary<char, CharacterSlot> Characters { get; } = new Dictionary<char, CharacterSlot>();

		public int FontColor { get; set; } = Color.Black.ToArgb();

		public class CharacterSlot
		{
			// Keep this for future use in case we want to use this in DirectX
			//public readonly Vertex.TransformedTextured[] CachedVertices;
			public readonly RectangleF Rectangle;
			public readonly int X;
			public readonly int Y;
			public readonly int Width;
			public readonly int Height;

			public CharacterSlot(int x, int y, int height, RectangleF rectangle)
			{
				if (rectangle.Width == 0 || rectangle.Height == 0)
				{
					rectangle = new RectangleF(0, 0, 1, 1);
				}
				Rectangle = rectangle;
				Width = (int)Math.Ceiling(rectangle.Right) + 1;
				Height = height;
				X = x + (int)Math.Floor(rectangle.Left) + 1;
				Y = y;
				// For future use if want to use this as a texture in DirectX or something
				/*
				float num = ((float)x) / ((float)textureSize);
				float num2 = ((float)y) / ((float)textureSize);
				float num3 = size.Width / ((float)textureSize);
				float num4 = size.Height / ((float)textureSize);
				this.CachedVertices = new Vertex.TransformedTextured[4];
				this.CachedVertices[0].X = 0f;
				this.CachedVertices[0].Y = size.Height;
				this.CachedVertices[0].U = num;
				this.CachedVertices[0].V = num2 + num4;
				this.CachedVertices[0].Z = 0f;
				this.CachedVertices[0].Rhw = 1f;
				this.CachedVertices[1].X = size.Width;
				this.CachedVertices[1].Y = size.Height;
				this.CachedVertices[1].U = num + num3;
				this.CachedVertices[1].V = num2 + num4;
				this.CachedVertices[1].Z = 0f;
				this.CachedVertices[1].Rhw = 1f;
				this.CachedVertices[2].X = 0f;
				this.CachedVertices[2].Y = 0f;
				this.CachedVertices[2].U = num;
				this.CachedVertices[2].V = num2;
				this.CachedVertices[2].Z = 0f;
				this.CachedVertices[2].Rhw = 1f;
				this.CachedVertices[3].X = size.Width;
				this.CachedVertices[3].Y = 0f;
				this.CachedVertices[3].U = num + num3;
				this.CachedVertices[3].V = num2;
				this.CachedVertices[3].Z = 0f;
				this.CachedVertices[3].Rhw = 1f;
				*/
			}
		}
	}
}
