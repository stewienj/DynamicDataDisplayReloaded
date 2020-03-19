using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Research.DynamicDataDisplay.BitmapGraphs
{
  public class GroupedMarkersCalculationProgressArgs : EventArgs
  {
    public double? Progress { get; set; }

    public TimeSpan CalculationTime { get; set; }
  }
}
