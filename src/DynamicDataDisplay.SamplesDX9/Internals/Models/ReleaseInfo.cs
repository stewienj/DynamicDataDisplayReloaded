using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace DynamicDataDisplay.SamplesDX9.Internals.Models
{
	[ContentProperty("Demonstrations")]
	public class ReleaseInfo
	{
		public ReleaseVersion Version { get; set; }

		private readonly ObservableCollection<Demonstration> demonstrations = new ObservableCollection<Demonstration>();

		public ObservableCollection<Demonstration> Demonstrations { get { return demonstrations; } }
	}
}
