using DynamicDataDisplay.BitmapGraphs;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DynamicDataDisplay.Samples.Demos.Custom
{
	/// <summary>
	/// Interaction logic for GraphicsFontTest.xaml
	/// </summary>
	public partial class GraphicsFontTest : UserControl
	{
		private GraphicsFont _graphicsFont = new GraphicsFont();

		public GraphicsFontTest()
		{
			FontGrid = _graphicsFont.CreateBitmapSource();
			CreateCharactersList();
			InitializeComponent();
		}

		private void CreateCharactersList()
		{
			Characters = _graphicsFont
			  .Characters
			  .Select(kvp => new
			  {
				  Char = kvp.Key,
				  X = kvp.Value.X,
				  Y = kvp.Value.Y,
				  Width = kvp.Value.Width,
				  Height = kvp.Value.Height,
				  Bitmap = GetBitmap(kvp)
			  });
		}

		private BitmapSource GetBitmap(KeyValuePair<char, GraphicsFont.CharacterSlot> kvp)
		{
			var block = _graphicsFont.GetBlock(kvp.Key.ToString());
			return new ArrayBitmapSource<uint>(block);
		}

		public BitmapSource FontGrid { get; private set; }

		public object Characters { get; set; }
	}
}
