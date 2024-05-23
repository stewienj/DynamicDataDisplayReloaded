using System;
using System.Windows.Markup;

namespace DynamicDataDisplay.RadioBand;

/// <summary>
/// XAML mark up extension to convert a double value into a string value.
/// This allows you to use your mapper like this inside your binding expression.
/// <d3:CursorCoordinateGraph
///     XTextMapping="{d3:RadioBandXMapper SignificantFigures=4}"
///     YTextMapping="{d3:RadioBandYMapper}"/>
/// </summary>
[MarkupExtensionReturnType(typeof(Func<double, string>))]
public class RadioBandXMapper : MarkupExtension
{
    public override object ProvideValue(IServiceProvider serviceProvider) => GetMapper();

    private Func<double, string> GetMapper() => xValue => xValue.ToEngineeringNotation(SignificantFigures) + "Hz";

    public int SignificantFigures { get; set; } = 3;
}
