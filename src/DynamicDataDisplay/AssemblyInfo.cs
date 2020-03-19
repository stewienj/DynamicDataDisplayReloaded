using Microsoft.Research.DynamicDataDisplay;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Security;
using System.Windows.Markup;

[module: SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]

[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "Microsoft.Research.DynamicDataDisplay")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "Microsoft.Research.DynamicDataDisplay.Navigation")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "Microsoft.Research.DynamicDataDisplay.Charts")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "Microsoft.Research.DynamicDataDisplay.Charts.Navigation")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "Microsoft.Research.DynamicDataDisplay.DataSources")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "Microsoft.Research.DynamicDataDisplay.Common.Palettes")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "Microsoft.Research.DynamicDataDisplay.Common.Auxiliary")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "Microsoft.Research.DynamicDataDisplay.Charts.Axes")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "Microsoft.Research.DynamicDataDisplay.Charts.Axes.Numeric")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "Microsoft.Research.DynamicDataDisplay.PointMarkers")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "Microsoft.Research.DynamicDataDisplay.Charts.Shapes")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "Microsoft.Research.DynamicDataDisplay.Charts.Markers")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "Microsoft.Research.DynamicDataDisplay.Converters")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "Microsoft.Research.DynamicDataDisplay.MarkupExtensions")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "Microsoft.Research.DynamicDataDisplay.Charts.Isolines")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "Microsoft.Research.DynamicDataDisplay.ViewportRestrictions")]

[assembly: XmlnsPrefix(D3AssemblyConstants.DefaultXmlNamespace, "d3")]

[assembly: CLSCompliant(true)]

[assembly: AllowPartiallyTrustedCallers]

namespace Microsoft.Research.DynamicDataDisplay
{
	public static class D3AssemblyConstants
	{
		public const string DefaultXmlNamespace = "http://research.microsoft.com/DynamicDataDisplay/1.0";
	}
}
