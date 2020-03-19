using System;
using System.Collections;
using System.Collections.Generic;

namespace DynamicDataDisplay.Markers.DataSources
{
	public sealed class DataSourcePart
	{
		public DataSourcePart(IEnumerable collection, string propertyName)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");
			if (string.IsNullOrEmpty(propertyName))
				throw new ArgumentNullException("propertyName");

			this.collection = collection;
			this.propertyName = propertyName;

			Type[] types = IEnumerableHelper.GetGenericInterfaceArgumentTypes(collection, typeof(IEnumerable<>));
			propertyType = (types != null && types.Length == 1) ? types[0] : typeof(object);
		}

		private readonly string propertyName;
		public string PropertyName
		{
			get { return propertyName; }
		}

		private readonly IEnumerable collection;
		public IEnumerable Collection
		{
			get { return collection; }
		}

		private readonly Type propertyType;
		public Type PropertyType
		{
			get { return propertyType; }
		}
	}
}
