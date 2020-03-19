namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	public static class ResourcePoolExtensions
	{
		public static T GetOrCreate<T>(this ResourcePool<T> pool) where T : new()
		{
			T instance = pool.Get();
			if (instance == null)
			{
				instance = new T();
			}

			return instance;
		}


		public static bool PutIfType<T>(this ResourcePool<T> pool, object item)
		{
			if (item is T)
			{
				pool.Put((T)item);
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
