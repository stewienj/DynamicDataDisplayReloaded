using DynamicDataDisplay;
using System.Runtime.CompilerServices;
using System.Windows.Markup;

[assembly: XmlnsDefinition(D3AssemblyConstants.DefaultXmlNamespace, "DynamicDataDisplay.Fractals")]
[assembly: Dependency("DynamicDataDisplay", LoadHint.Always)]
[assembly: Dependency("DynamicDataDisplay.Maps", LoadHint.Always)]
