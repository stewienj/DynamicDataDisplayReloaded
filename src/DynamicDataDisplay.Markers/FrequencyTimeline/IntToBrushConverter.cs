using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace DynamicDataDisplay.FrequencyTimeline
{
    internal class IntToBrushConverter : IValueConverter
    {
        private static Lazy<IntToBrushConverter> _instance = new Lazy<IntToBrushConverter>(true);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int colorARGB)
            {
                return new SolidColorBrush(ToColor(colorARGB));
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private Color ToColor(int colorARGB)
        {
            var bytes = BitConverter.GetBytes(colorARGB);
            byte a = bytes[3];
            byte r = bytes[2];
            byte g = bytes[1];
            byte b = bytes[0];

            return Color.FromArgb(a, r, g, b);
        }


        public static IntToBrushConverter Instance => _instance.Value;
    }
}
