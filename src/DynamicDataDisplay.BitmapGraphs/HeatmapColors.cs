using System;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.Research.DynamicDataDisplay.BitmapGraphs
{
	public class HeatmapColors
	{
		public class ColorStop
		{
			public uint Color { get; set; }
			public double Offset { get; set; }
		}

		public HeatmapColors()
		{
			ApplyColorScale(ColorSpectrum);
		}

		public void SetColorScale(string colorScaleDescription)
		{
			var propertyInfo = typeof(HeatmapColors)
			  .GetProperties()
			  .Where(pi => pi.GetCustomAttributes(typeof(DisplayNameAttribute), false)
				.OfType<DisplayNameAttribute>()
				.Select(displayName => displayName.DisplayName)
				.Contains(colorScaleDescription)
				)
			  .FirstOrDefault();

			var colorScale = propertyInfo?.GetValue(this) as ColorStop[];
			ApplyColorScale(colorScale);
		}

		private void ApplyColorScale(ColorStop[] colorScale)
		{
			if (colorScale == null || colorScale.Length == 0)
				return;

			int lastOffset = 0;
			uint lastColor = colorScale.First().Color;

			foreach (var colorStop in colorScale)
			{
				int offsetEnd = (int)Math.Round(colorStop.Offset * (ColorMap.Length - 1));
				int segLength = offsetEnd - lastOffset;
				if (segLength > 0)
				{
					for (int i = lastOffset; i <= offsetEnd; ++i)
					{
						double multiplier = (i - lastOffset) / (double)segLength;
						double a1 = (1.0 - multiplier) * (lastColor & 0xFF000000);
						double a2 = multiplier * (colorStop.Color & 0xFF000000);
						double r1 = (1.0 - multiplier) * (lastColor & 0xFF0000);
						double r2 = multiplier * (colorStop.Color & 0xFF0000);
						double g1 = (1.0 - multiplier) * (lastColor & 0x00FF00);
						double g2 = multiplier * (colorStop.Color & 0x00FF00);
						double b1 = (1.0 - multiplier) * (lastColor & 0x0000FF);
						double b2 = multiplier * (colorStop.Color & 0x0000FF);

						ColorMap[i] = ((uint)(a1 + a2) & 0xFF000000)
						  | ((uint)(r1 + r2) & 0xFF0000)
						  | ((uint)(g1 + g2) & 0x00FF00)
						  | ((uint)(b1 + b2) & 0x0000FF);
					}
					lastColor = colorStop.Color;
					lastOffset = offsetEnd;
				}
			}
		}

		/// <summary>
		/// Returns an array of 65536 colors
		/// </summary>
		public uint[] ColorMap { get; } = new uint[65536];

		public static string[] ColorScalesAvailable
		{
			get
			{
				var displayNames = typeof(HeatmapColors)
				  .GetProperties()
				  .SelectMany(propertyInfo => propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), false))
				  .OfType<DisplayNameAttribute>()
				  .Where(displayName => displayName != DisplayNameAttribute.Default)
				  .Select(displayName => displayName.DisplayName);

				return displayNames.ToArray();
			}
		}

		[DisplayName("Grayscale")]
		public ColorStop[] Grayscale { get; } =
		  new ColorStop[]
		  {
		new ColorStop{ Color=0x000000, Offset=0},
		new ColorStop{ Color=0xFFFFFF, Offset=1},
		  };

		[DisplayName("Black Body Radiation")]
		public ColorStop[] BlackBodyRadiation { get; } =
		  new ColorStop[]
		  {
		new ColorStop{ Color=0x000000, Offset=0.00000},
		new ColorStop{ Color=0xF82600, Offset=0.11111},
		new ColorStop{ Color=0xFC7700, Offset=0.22222},
		new ColorStop{ Color=0xFFAB1F, Offset=0.33333},
		new ColorStop{ Color=0xFFC360, Offset=0.44444},
		new ColorStop{ Color=0xFFDDA4, Offset=0.55555},
		new ColorStop{ Color=0xFFF4E4, Offset=0.66666},
		new ColorStop{ Color=0xE8EDFF, Offset=0.77777},
		new ColorStop{ Color=0xDBE3FF, Offset=0.88888},
		new ColorStop{ Color=0xCBD7FF, Offset=0.99999},
		  };

		[DisplayName("Aqua")]
		public ColorStop[] Aqua { get; } =
		  new ColorStop[]
		  {
		new ColorStop{ Color=0x000000, Offset=0},
		new ColorStop{ Color=0x00FFFF, Offset=0.5},
		new ColorStop{ Color=0xFFFFFF, Offset=1},
		  };

		[DisplayName("Deap Sea")]
		public ColorStop[] DeapSea { get; } =
		  new ColorStop[]
		  {
		new ColorStop{ Color=0x000000, Offset=0},
		new ColorStop{ Color=0x183567, Offset=0.6},
		new ColorStop{ Color=0x2e649e, Offset=0.75},
		new ColorStop{ Color=0x17adcb, Offset=0.9},
		new ColorStop{ Color=0x00fafa, Offset=1.0},
		  };

		[DisplayName("Color Spectrum")]
		public ColorStop[] ColorSpectrum { get; } =
		  new ColorStop[]
		  {
		new ColorStop{ Color=0x000000, Offset=0},
		new ColorStop{ Color=0x000080, Offset=0.005},
		new ColorStop{ Color=0x0000FF, Offset=0.25},
		new ColorStop{ Color=0x008000, Offset=0.5},
		new ColorStop{ Color=0xFFFF00, Offset=0.75},
		new ColorStop{ Color=0xFF0000, Offset=1.0},
		  };

		[DisplayName("Incandescent")]
		public ColorStop[] Incandescent { get; } =
		  new ColorStop[]
		  {
		new ColorStop{ Color=0x000000, Offset=0},
		new ColorStop{ Color=0x8B0000, Offset=0.33},
		new ColorStop{ Color=0xFFFF00, Offset=0.66},
		new ColorStop{ Color=0xFFFFFF, Offset=1.0},
		  };

		[DisplayName("Heated Metal")]
		public ColorStop[] HeatedMetal { get; } =
		  new ColorStop[]
		  {
		new ColorStop{ Color=0x000000, Offset=0},
		new ColorStop{ Color=0x800080, Offset=0.4},
		new ColorStop{ Color=0xFF0000, Offset=0.6},
		new ColorStop{ Color=0xFFFF00, Offset=0.8},
		new ColorStop{ Color=0xFFFFFF, Offset=1.0},
		  };

		[DisplayName("Sun Rise")]
		public ColorStop[] SunRise { get; } =
		  new ColorStop[]
		  {
		new ColorStop{ Color=0xFF0000, Offset=0},
		new ColorStop{ Color=0xFFFF00, Offset=0.66},
		new ColorStop{ Color=0xFFFFFF, Offset=1.0},
		  };

		[DisplayName("Stepped Colors")]
		public ColorStop[] SteppedColors { get; } =
		  new ColorStop[]
		  {
		new ColorStop{ Color=0x000080, Offset=0},
		new ColorStop{ Color=0x000080, Offset=0.25},
		new ColorStop{ Color=0x008000, Offset=0.26},
		new ColorStop{ Color=0x008000, Offset=0.5},
		new ColorStop{ Color=0xFFFF00, Offset=0.51},
		new ColorStop{ Color=0xFFFF00, Offset=0.75},
		new ColorStop{ Color=0xFF0000, Offset=0.76},
		new ColorStop{ Color=0xFF0000, Offset=1.0},
		  };

		[DisplayName("Visible Spectrum")]
		public ColorStop[] VisibleSpectrum { get; } =
		  new ColorStop[]
		  {
		new ColorStop{ Color=0xff00ff, Offset=0},
		new ColorStop{ Color=0x0000ff, Offset=0.25},
		new ColorStop{ Color=0x00ff00, Offset=0.5},
		new ColorStop{ Color=0xffff00, Offset=0.75},
		new ColorStop{ Color=0xff0000, Offset=1.0},
		  };
	}
}
