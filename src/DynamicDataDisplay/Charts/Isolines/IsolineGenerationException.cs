using System;
using System.Runtime.Serialization;

namespace DynamicDataDisplay.Charts.Isolines
{
	/// <summary>
	/// Exception that is thrown when error occurs while building isolines.
	/// </summary>
	public sealed class IsolineGenerationException : Exception
	{
		public IsolineGenerationException() { }
		public IsolineGenerationException(string message) : base(message) { }
		public IsolineGenerationException(string message, Exception inner) : base(message, inner) { }
	}
}
