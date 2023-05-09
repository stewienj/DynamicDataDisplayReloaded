using System;
using System.Windows.Markup;

namespace DynamicDataDisplay.Samples.Helpers
{
    /// <summary>
    /// XAML mark up extension to convert a double value into a string value.
    /// This is a singleton so you don't have to instantiate it in your resources.
    /// This allows you to use your mapper like this inside your binding expression.
    /// Converter={sf:MinutesToUTCStringMapper}}
    /// </summary>
    [MarkupExtensionReturnType(typeof(Func<double, string>))]
    public class MinutesToUTCStringMapper : MarkupExtension
    {
        private static readonly Lazy<MinutesToUTCStringMapper> _mapper = new Lazy<MinutesToUTCStringMapper>(true);

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _mapper.Value.GetMapper();
        }

        public static MinutesToUTCStringMapper GetInstance() => _mapper.Value;

        private Func<double, string> GetMapper()
        {
            Func<double, string> mapper = d =>
            {
                var dateTime = new DateTime();
                if (d > 0.0)
                {
                    dateTime = dateTime.AddMinutes(d);
                }

                dateTime = dateTime.ToUniversalTime();

                return dateTime.ToString("dd MMM yyyy HH:mm");
            };

            return mapper;
        }
    }
}
