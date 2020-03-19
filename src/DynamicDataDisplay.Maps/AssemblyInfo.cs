using System.Windows.Markup;
using Microsoft.Research.DynamicDataDisplay;
using System.Runtime.CompilerServices;
using System.Security;

[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "Microsoft.Research.DynamicDataDisplay.Charts.Maps")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "Microsoft.Research.DynamicDataDisplay.Charts.Maps.Network")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "Microsoft.Research.DynamicDataDisplay.Maps")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "Microsoft.Research.DynamicDataDisplay.Maps.Charts")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "Microsoft.Research.DynamicDataDisplay.Maps.Servers")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "Microsoft.Research.DynamicDataDisplay.Maps.Servers.Network")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "Microsoft.Research.DynamicDataDisplay.Maps.Servers.FileServers")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "Microsoft.Research.DynamicDataDisplay.Maps.Charts.TiledRendering")]
[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "Microsoft.Research.DynamicDataDisplay.Maps.DeepZoom")]

[assembly: AllowPartiallyTrustedCallers]

// Allow calling of Native Win32 methods
[assembly: SecurityRules(SecurityRuleSet.Level1)]