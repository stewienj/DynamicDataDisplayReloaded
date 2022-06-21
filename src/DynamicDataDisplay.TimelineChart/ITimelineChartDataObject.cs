using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DynamicDataDisplay.TimelineChart
{
  public interface ITimelineChartDataObject
  {
    public double X { get; }
    public double Y { get; }
    public double Width { get; }
    public double Height { get; }
    public string Id { get; }
    public Color Color { get; }
  }
}
