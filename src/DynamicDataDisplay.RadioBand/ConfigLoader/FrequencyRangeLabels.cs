using System.Linq;
using System.Xml.Linq;

namespace DynamicDataDisplay.RadioBand.ConfigLoader
{
	public class FrequencyRangeLabels : FrequencyLabelSet<RangeLabel>
	{

		internal static FrequencyRangeLabels FromXmlNode(XElement node)
		{
			return new FrequencyRangeLabels(node);
		}

		private FrequencyRangeLabels(XElement node)
		{
			Description = GetDescription(node);
			Labels = node
			  .Elements()
			  .Where(e => e.Name == nameof(RangeLabel))
			  .Select(e => new RangeLabel(e))
			  .ToArray();
		}
	}
}
