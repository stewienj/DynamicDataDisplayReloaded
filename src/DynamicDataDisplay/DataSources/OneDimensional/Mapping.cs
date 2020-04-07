using System;
using System.Windows;

namespace DynamicDataDisplay.DataSources
{
	/// <summary>Mapping class holds information about mapping of TSource type
	/// to some DependencyProperty.</summary>
	/// <typeparam name="TSource">Mapping source type.</typeparam>
	public sealed class Mapping<TSource>
	{
		/// <summary>Property that will be set.</summary>
		public DependencyProperty Property { get; set; }
		/// <summary>Function that computes value for property from TSource type.</summary>
		public Func<TSource, object> F { get; set; }
	}
}
