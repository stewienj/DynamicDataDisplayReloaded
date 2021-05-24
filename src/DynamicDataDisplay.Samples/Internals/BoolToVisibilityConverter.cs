using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace DynamicDataDisplay.Samples.Internals
{
    /// <summary>
    /// XAML mark up extension to convert a bool value into a visibility value.
    /// This is a singleton so you don't have to instantiate it in your resources.
    /// This allows you to use you converter like this inside your binding expression.
    /// Converter={local:BoolToVisibilityConverter}}
    /// </summary>
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : MarkupExtension, IValueConverter
    {
        private static readonly Lazy<BoolToVisibilityConverter> _converter = new Lazy<BoolToVisibilityConverter>(true);

        /// <summary> 
        /// Converts a bool to a control Visibiltiy
        /// </summary> 
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool boolValue && targetType == typeof(Visibility))
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            throw new ArgumentException("Invalid argument/return type. Expected argument: bool and return type: Visibility");
        }

        /// <summary> 
        /// Converts a control Visibiltiy to a bool
        /// </summary> 
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Visibility visibility && targetType == typeof(bool))
            {
                return visibility == Visibility.Visible;
            }
            throw new ArgumentException("Invalid argument/return type. Expected argument: Visibility and return type: bool");
        }

        /// <summary>
        /// When implemented in a derived class, returns an object that is provided
        /// as the value of the target property for this markup extension.
        /// 
        /// When a XAML processor processes a type node and member value that is a markup extension,
        /// it invokes the ProvideValue method of that markup extension and writes the mResult into the
        /// object graph or serialization stream. The XAML object writer passes service context to each
        /// such implementation through the serviceProvider parameter.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter.Value;
        }
    }
}
