using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;

namespace DynamicDataDisplay.RadioBand
{
	/// <summary>
	/// Provides the logic to add and remove logical children from the parent control when items are added and removed
	/// </summary>
	public class NearestVisualSourceCollection : IList<NearestVisualSource>, ICollection<NearestVisualSource>, IEnumerable<NearestVisualSource>, IEnumerable, IList, ICollection
	{
		private readonly FrameworkElement _owner;
		private List<NearestVisualSource> _items = new List<NearestVisualSource>();
		private MethodInfo _addLogicalChild;
		private MethodInfo _removeLogicalChild;

		internal NearestVisualSourceCollection(FrameworkElement owner)
		{
			_owner = owner;
			_addLogicalChild = typeof(FrameworkElement).GetMethod("AddLogicalChild", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
			_removeLogicalChild = typeof(FrameworkElement).GetMethod("RemoveLogicalChild", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
		}

		private void AddLogicalChild(int index, NearestVisualSource item)
		{
			AddLogicalChild(item);
		}


		private void AddLogicalChild(NearestVisualSource item)
		{
			_addLogicalChild.Invoke(_owner, new[] { item });
		}

		private void RemoveLogicalChild(NearestVisualSource item)
		{
			_removeLogicalChild.Invoke(_owner, new[] { item });
		}


		public int Count
		{
			get
			{
				return _items.Count;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return _items[index];
			}
			set
			{
				RemoveLogicalChild(_items[index]);
				_items[index] = value as NearestVisualSource;
				AddLogicalChild(index, value as NearestVisualSource);
			}
		}

		public NearestVisualSource this[int index]
		{
			get
			{
				return _items[index];
			}
			set
			{
				RemoveLogicalChild(_items[index]);
				_items[index] = value;
				AddLogicalChild(index, value);
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection)_items).CopyTo(array, index);
		}

		public void CopyTo(NearestVisualSource[] array, int index)
		{
			_items.CopyTo(array, index);
		}

		int IList.Add(object value)
		{
			_items.Add(value as NearestVisualSource);
			AddLogicalChild(_items.Count - 1, value as NearestVisualSource);
			return _items.Count - 1;
		}

		public void Add(NearestVisualSource value)
		{
			_items.Add(value);
			AddLogicalChild(_items.Count - 1, value);
		}

		public void Clear()
		{
			foreach (var item in _items)
			{
				RemoveLogicalChild(item);
			}
			_items.Clear();
		}

		bool IList.Contains(object value)
		{
			return ((IList)_items).Contains(value);
		}

		public bool Contains(NearestVisualSource value)
		{
			return _items.Contains(value);
		}

		int IList.IndexOf(object value)
		{
			return IndexOf(value as NearestVisualSource);
		}

		public int IndexOf(NearestVisualSource value)
		{
			if (value == null)
				return -1;
			return _items.IndexOf(value);
		}

		void IList.Insert(int index, object value)
		{
			_items.Insert(index, value as NearestVisualSource);
			AddLogicalChild(index, value as NearestVisualSource);
		}

		public void Insert(int index, NearestVisualSource value)
		{
			_items.Insert(index, value);
			AddLogicalChild(index, value);
		}

		void IList.Remove(object value)
		{
			RemoveLogicalChild(value as NearestVisualSource);
			_items.Remove(value as NearestVisualSource);
		}

		public bool Remove(NearestVisualSource value)
		{
			RemoveLogicalChild(value);
			return _items.Remove(value);
		}

		public void RemoveAt(int index)
		{
			RemoveLogicalChild(_items[index]);
			_items.RemoveAt(index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		IEnumerator<NearestVisualSource> IEnumerable<NearestVisualSource>.GetEnumerator()
		{
			return _items.GetEnumerator();
		}
	}
}
