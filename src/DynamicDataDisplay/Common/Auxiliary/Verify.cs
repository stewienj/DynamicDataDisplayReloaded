using System;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	public static class Verify
	{
		[DebuggerStepThrough]
		public static void IsTrue(this bool condition)
		{
			if (!condition)
			{
				throw new ArgumentException(Strings.Exceptions.AssertionFailedSearch);
			}
		}

		[DebuggerStepThrough]
		public static void IsTrue(this bool condition, string paramName)
		{
			if (!condition)
			{
				throw new ArgumentException(Strings.Exceptions.AssertionFailedSearch, paramName);
			}
		}

		public static void IsTrueWithMessage(this bool condition, string message)
		{
			if (!condition)
				throw new ArgumentException(message);
		}

		[DebuggerStepThrough]
		public static void AssertNotNull(object obj)
		{
			Verify.IsTrue(obj != null);
		}

		public static void VerifyNotNull(this object obj, string paramName)
		{
			if (obj == null)
				throw new ArgumentNullException(paramName);
		}

		public static void VerifyNotNull(this object obj)
		{
			VerifyNotNull(obj, "value");
		}

		[DebuggerStepThrough]
		public static void AssertIsNotNaN(this double d)
		{
			Verify.IsTrue(!double.IsNaN(d));
		}

		[DebuggerStepThrough]
		public static void AssertIsFinite(this double d)
		{
			Verify.IsTrue(!double.IsInfinity(d) && !(double.IsNaN(d)));
		}
	}
}
