using System;

namespace Microsoft.Research.DynamicDataDisplay.Common
{
	public sealed class WeakReference<T>
	{
		private readonly WeakReference reference;

		public WeakReference(WeakReference reference)
		{
			this.reference = reference;
		}

		public WeakReference(T referencedObject)
		{
			this.reference = new WeakReference(referencedObject);
		}

		public bool IsAlive
		{
			get { return reference.IsAlive; }
		}

		public T Target
		{
			get { return (T)reference.Target; }
		}
	}
}
