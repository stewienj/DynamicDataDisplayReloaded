using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DynamicDataDisplay.RadioBand.ConfigLoader
{
	public class FrequencyLabelSet
	{
		public static string[] GetDescriptionFromXml(XElement node)
		{
			if (node.FirstNode is XText text)
			{
				var description =
				  text.Value
				  .Replace("\r", "")
				  .Trim(" \n".ToArray())
				  .Split("\n".ToArray(), StringSplitOptions.None)
				  .Select(c => c.Trim())
				  .ToArray();
				return description;
			}
			else
			{
				return new string[0];
			}
		}
		public string[] Description { get; set; } = new string[0];
	}

	public class FrequencyLabelSet<LabelType> : FrequencyLabelSet, IEnumerable<LabelType>
	{
		public string[] GetDescription(XElement node)
		{
			return FrequencyLabelSet.GetDescriptionFromXml(node);
		}

		public IEnumerator<LabelType> GetEnumerator()
		{
			return Labels.AsEnumerable().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Labels.GetEnumerator();
		}

		public LabelType[] Labels { get; set; } = new LabelType[0];
	}
}
