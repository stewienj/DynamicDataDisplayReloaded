using System.Collections.Generic;

namespace DynamicDataDisplay.Samples.Internals.ViewModels
{
	public class FlatViewModel
	{
		public FlatViewModel(List<DemonstrationViewModel> demonstrations)
		{
			this.demonstrations = demonstrations;
		}

		private readonly List<DemonstrationViewModel> demonstrations = new List<DemonstrationViewModel>();
		public List<DemonstrationViewModel> Demonstrations { get { return demonstrations; } }
	}
}
