using System;
using System.Diagnostics;

namespace DynamicDataDisplay.Common
{
	[Conditional("DEBUG")]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class SkipPropertyCheckAttribute : Attribute
	{
	}
}
