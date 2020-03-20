using System.Windows.Markup;
using DynamicDataDisplay;
using System.Runtime.CompilerServices;
using System.Security;
using Microsoft.Research.DynamicDataDisplay;

[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "DynamicDataDisplay.RadioBand")]
[assembly: AllowPartiallyTrustedCallers]

// Allow calling of Native Win32 methods
[assembly: SecurityRules(SecurityRuleSet.Level1)]
