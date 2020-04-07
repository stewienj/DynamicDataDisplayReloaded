using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace DynamicDataDisplay.Charts.Selectors
{
	public class SelectorObservableCollection<T> : ObservableCollection<T>
	{
		private bool changing = false;

		public bool Changing
		{
			get { return changing; }
		}

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			changing = true;
			base.OnCollectionChanged(e);
			changing = false;
		}
	}
}
