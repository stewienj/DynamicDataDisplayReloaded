﻿using System;
using System.Runtime.Serialization;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Isolines
{
	/// <summary>
	/// Exception that is thrown when error occurs while building isolines.
	/// </summary>
	[Serializable]
	public sealed class IsolineGenerationException : Exception
	{
		public IsolineGenerationException() { }
		public IsolineGenerationException(string message) : base(message) { }
		public IsolineGenerationException(string message, Exception inner) : base(message, inner) { }
		public IsolineGenerationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
