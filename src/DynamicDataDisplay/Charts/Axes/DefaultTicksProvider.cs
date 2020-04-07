namespace DynamicDataDisplay.Charts
{
	public static class DefaultTicksProvider
	{
		public static readonly int DefaultTicksCount = 10;

		public static ITicksInfo<T> GetTicks<T>(this ITicksProvider<T> provider, Range<T> range)
		{
			return provider.GetTicks(range, DefaultTicksCount);
		}
	}
}
