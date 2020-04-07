using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace DynamicDataDisplay.Common.Auxiliary
{
	public static class ObservableCollectionHelper
	{
		public static void ApplyChanges<T>(this ObservableCollection<T> collection, NotifyCollectionChangedEventArgs args)
		{
			if (args.NewItems != null)
			{
				int startingIndex = args.NewStartingIndex;
				var newItems = args.NewItems;

				for (int i = 0; i < newItems.Count; i++)
				{
					T addedItem = (T)newItems[i];
					collection.Insert(startingIndex + i, addedItem);
				}
			}
			if (args.OldItems != null)
			{
				for (int i = 0; i < args.OldItems.Count; i++)
				{
					T removedItem = (T)args.OldItems[i];
					collection.Remove(removedItem);
				}
			}
		}
	}
}
