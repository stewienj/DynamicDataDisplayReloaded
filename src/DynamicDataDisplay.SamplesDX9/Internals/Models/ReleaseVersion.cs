using System.ComponentModel;
using System.Linq;

namespace DynamicDataDisplay.SamplesDX9.Internals.Models
{
	[TypeConverter(typeof(VersionTypeConverter))]
	public class ReleaseVersion
	{
		public ReleaseVersion(string major, string minor, string revision)
		{
			this.Major = major;
			this.Minor = minor;
			this.Revision = revision;
		}

		public string Major { get; set; }
		public string Minor { get; set; }
		public string Revision { get; set; }

		public override string ToString()
		{
			return new string[] { Major, Minor, Revision }.Where(x => !string.IsNullOrWhiteSpace(x)).Aggregate((a, b) => a + "." + b);
		}
	}
}
