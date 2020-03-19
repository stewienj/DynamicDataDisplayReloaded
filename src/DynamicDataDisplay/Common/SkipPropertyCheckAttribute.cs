using System;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay.Common
{
	[Conditional("DEBUG")]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class SkipPropertyCheckAttribute : Attribute
	{
	}
}
