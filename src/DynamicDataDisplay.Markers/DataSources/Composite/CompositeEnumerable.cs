using System.Collections;
using System.Collections.Generic;

namespace DynamicDataDisplay.Markers.DataSources
{
	internal sealed class CompositeEnumerable : IEnumerable<DynamicItem>
	{
		private readonly DataSourcePartCollection parts;
		public CompositeEnumerable(DataSourcePartCollection parts)
		{
			this.parts = parts;
		}

		#region IEnumerable<DynamicItem> Members

		public IEnumerator<DynamicItem> GetEnumerator()
		{
			return new CompositeEnumerator(parts);
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
