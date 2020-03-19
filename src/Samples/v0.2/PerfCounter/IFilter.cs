using System.Collections.Generic;

namespace PerfCounterChart
{
	public interface IFilter<T>
	{
		IList<T> Filter(IList<T> c);
	}
}
