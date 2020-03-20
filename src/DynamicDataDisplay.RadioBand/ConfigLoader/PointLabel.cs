using System.Windows.Media;
using System.Xml.Linq;

namespace DynamicDataDisplay.RadioBand.ConfigLoader
{
	public class PointLabel
	{
		public double Position { get; set; } = 0;
		public Color? ColorStop { get; set; } = null;
		public string[] LabelText { get; set; } = new string[0];

		public PointLabel(XElement node)
		{
			foreach (var attribute in node.Attributes())
			{
				var name = attribute.Name.LocalName;
				var value = attribute.Value;
				switch (name)
				{
					case nameof(Position):
						Position = double.Parse(value);
						break;
					case nameof(ColorStop):
						ColorStop = (Color)ColorConverter.ConvertFromString(value);
						break;
				}
			}
			LabelText = FrequencyLabelSet.GetDescriptionFromXml(node);
		}

	}
}
