using DynamicDataDisplay;
using System.Runtime.CompilerServices;
using System.Security;
using System.Windows.Markup;

[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "DynamicDataDisplay.Charts")]
[assembly: Dependency("DynamicDataDisplay", LoadHint.Always)]
[assembly: AllowPartiallyTrustedCallers]