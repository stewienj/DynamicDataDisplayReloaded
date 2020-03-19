using Microsoft.Research.DynamicDataDisplay;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace BitmapbaseGraphSample
{
    public class BitmapGraph : BitmapBasedGraph
    {
        protected override BitmapSource RenderFrame(DataRect visible, Rect output, RenderRequest renderRequest)
        {
            return new BitmapImage(new Uri(@"C:\Users\Mikhail\Pictures\Wallpapers\img10.jpg"));
        }
    }
}
