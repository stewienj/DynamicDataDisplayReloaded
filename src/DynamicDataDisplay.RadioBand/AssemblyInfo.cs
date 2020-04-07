using DynamicDataDisplay;
using System.Security;
using System.Windows.Markup;

[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "DynamicDataDisplay.RadioBand")]
[assembly: AllowPartiallyTrustedCallers]

// Allow calling of Native Win32 methods
[assembly: SecurityRules(SecurityRuleSet.Level1)]
