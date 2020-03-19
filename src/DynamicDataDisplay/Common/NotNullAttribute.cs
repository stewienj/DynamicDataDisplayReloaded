using System;
using System.Diagnostics;

namespace Microsoft.Research.DynamicDataDisplay
{
	[Conditional("DEBUG")]
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public sealed class NotNullAttribute : Attribute
	{
	}
}
