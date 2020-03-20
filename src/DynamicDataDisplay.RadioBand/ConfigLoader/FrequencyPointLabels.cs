using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;

namespace DynamicDataDisplay.RadioBand.ConfigLoader
{
  public class FrequencyPointLabels : FrequencyLabelSet<PointLabel>
  {
    public Color LineMarkerColor { get; set; } = Colors.Transparent;

    public double LineMarkerThickness { get; set; } = 0;

    internal static FrequencyPointLabels FromXmlNode(XElement node)
    {
      return new FrequencyPointLabels(node);
    }

    private FrequencyPointLabels(XElement node)
    {
      string colorStr = node.Attribute(nameof(LineMarkerColor))?.Value;
      if (!string.IsNullOrEmpty(colorStr))
      {
        LineMarkerColor = (Color)ColorConverter.ConvertFromString(colorStr);
      }
      string thicknessStr = node.Attribute(nameof(LineMarkerThickness))?.Value;
      if (!string.IsNullOrEmpty(thicknessStr))
      {
        if (double.TryParse(thicknessStr, out double thickness))
        {
          LineMarkerThickness = thickness;
        }
      }
      Description = GetDescription(node);
      Labels = node
        .Elements()
        .Where(e => e.Name == nameof(PointLabel))
        .Select(e => new PointLabel(e))
        .ToArray();
    }
  }
}
