using DynamicDataDisplay;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Security;
using System.Windows.Markup;

[module: SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]

[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "DynamicDataDisplay")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "DynamicDataDisplay.Navigation")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "DynamicDataDisplay.Charts")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "DynamicDataDisplay.Charts.Navigation")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "DynamicDataDisplay.DataSources")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "DynamicDataDisplay.Common.Palettes")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "DynamicDataDisplay.Common.Auxiliary")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "DynamicDataDisplay.Charts.Axes")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "DynamicDataDisplay.Charts.Axes.Numeric")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "DynamicDataDisplay.PointMarkers")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "DynamicDataDisplay.Charts.Shapes")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "DynamicDataDisplay.Charts.Markers")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "DynamicDataDisplay.Converters")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "DynamicDataDisplay.MarkupExtensions")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "DynamicDataDisplay.Charts.Isolines")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "DynamicDataDisplay.ViewportRestrictions")]

[assembly: XmlnsPrefix(D3AssemblyConstants.DefaultXmlNamespace, "d3")]

[assembly: CLSCompliant(true)]

[assembly: AllowPartiallyTrustedCallers]

namespace DynamicDataDisplay
{
	public static class D3AssemblyConstants
	{
		public const string DefaultXmlNamespace = "http://research.microsoft.com/DynamicDataDisplay/1.0";
	}
}
