using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace DynamicDataDisplay.Samples.Helpers
{
    /// <summary>
    /// XAML mark up extension to convert a double value into a string value.
    /// This is a singleton so you don't have to instantiate it in your resources.
    /// This allows you to use your mapper like this inside your binding expression.
    /// Converter={sf:DoubleToEngineeringNotationMapper}}
    /// </summary>
    [MarkupExtensionReturnType(typeof(Func<double, string>))]
    public class DoubleToEngineeringNotationMapper : MarkupExtension
    {
        private static readonly Lazy<DoubleToEngineeringNotationMapper> _mapper = new Lazy<DoubleToEngineeringNotationMapper>(true);

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _mapper.Value.GetMapper();
        }

        public static DoubleToEngineeringNotationMapper GetInstance() => _mapper.Value;

        private Func<double, string> GetMapper()
        {
            Func<double, string> mapper = d => d.ToEngineeringNotation() + "Hz";

            return mapper;
        }
    }
}
