using System;
using System.Windows.Markup;

namespace DynamicDataDisplay.Samples.Helpers;

/// <summary>
/// XAML mark up extension to convert a double value into a string value.
/// This allows you to use your mapper like this inside your binding expression.
/// Converter={sf:MinutesToUTCStringMapper}}
/// </summary>
[MarkupExtensionReturnType(typeof(Func<double, string>))]
public class MinutesToUTCStringMapper : MarkupExtension
{
    public override object ProvideValue(IServiceProvider serviceProvider) => GetMapper();

    private Func<double, string> GetMapper() => minutes =>
    {
        var dateTime = DateTime.MinValue;

        try
        {
            if (minutes > 0.0)
            {
                if (minutes < (DateTime.MaxValue - dateTime).TotalMinutes)
                {
                    dateTime = dateTime.AddMinutes(minutes);
                }
                else
                {
                    dateTime = DateTime.MaxValue;
                }
            }
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }
        catch { }

        return dateTime.ToString("dd MMM yyyy HH:mm");
    };
}
