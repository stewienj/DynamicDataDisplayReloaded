using Microsoft.Research.DynamicDataDisplay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace DynamicDataDisplay.Markers.DataSources
{
	public sealed class DataSourcePartCollection : IEnumerable<DataSourcePart>, INotifyCollectionChanged
	{
		private readonly List<DataSourcePart> parts = new List<DataSourcePart>();

		public DataSourcePartCollection(IEnumerable<DataSourcePart> parts)
		{
			if (parts == null)
				throw new ArgumentNullException("parts");

			this.parts.AddRange(parts);
		}

		public DataSourcePartCollection(params DataSourcePart[] parts)
		{
			if (parts == null)
				throw new ArgumentNullException("parts");

			this.parts.AddRange(parts);
		}

		public void ReplacePart(string name, DataSourcePart newDataPart)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentException("Part's name should not be null or empty string.", "name");

			var index = parts.FindIndex(part => part.PropertyName == name);
			if (index >= 0)
			{
				parts[index] = newDataPart;
				CollectionChanged.Raise(this);
			}
			else
			{
				throw new ArgumentException(string.Format("Cannot find part with name \"{0}\"", name), "name");
			}
		}

		public int Count
		{
			get { return parts.Count; }
		}

		#region IEnumerable<DataSourcePart> Members

		public IEnumerator<DataSourcePart> GetEnumerator()
		{
			return parts.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region INotifyCollectionChanged Members

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		#endregion
	}
}
