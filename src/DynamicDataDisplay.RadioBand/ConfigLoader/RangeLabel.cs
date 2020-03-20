using System.Windows.Media;
using System.Xml.Linq;

namespace DynamicDataDisplay.RadioBand.ConfigLoader
{
	public class RangeLabel
	{
		public double Start { get; set; } = 0;
		public double End { get; set; } = 0;
		public Color? Color { get; set; } = null;
		public string[] LabelText { get; set; } = new string[0];

		public RangeLabel(XElement node)
		{
			foreach (var attribute in node.Attributes())
			{
				var name = attribute.Name.LocalName;
				var value = attribute.Value;
				switch (name)
				{
					case nameof(Start):
						Start = double.Parse(value);
						break;
					case nameof(End):
						End = double.Parse(value);
						break;
					case nameof(Color):
						Color = (Color)ColorConverter.ConvertFromString(value);
						break;
				}
			}
			LabelText = FrequencyLabelSet.GetDescriptionFromXml(node);
		}
	}
}
