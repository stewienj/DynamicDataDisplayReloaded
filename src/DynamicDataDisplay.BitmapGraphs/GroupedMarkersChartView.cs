using Microsoft.Research.DynamicDataDisplay.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Microsoft.Research.DynamicDataDisplay.BitmapGraphs
{
  public class GroupedMarkersChartView : PointsBitmapGraphBase
  {
    private GroupedMarkers _groupedMarkers = new GroupedMarkers();

    public GroupedMarkersChartView() : base()
    {
      var aggregatedBlockPixelWidth = 40;
      var aggregatedBlockPixelHeight = 25;
      //40, 25

      _pointsCalculator = new GroupedMarkersAggregatorAndSelector(this) { AggregatedBlockPixelWidth = aggregatedBlockPixelWidth, AggregatedBlockPixelHeight = aggregatedBlockPixelHeight };
      _pointsCalculator.NewPointsReady += PointsCalculator_NewPointsReady;
      _pointsCalculator.SelectedPointsChanged += PointsCalculator_SelectedPointsChanged;

    }


    protected override BitmapSource RenderDataFrame(DataRect data, Rect output, RenderRequest renderRequest)
    {
      var points = GetLastPointsInDataRect(data);
      if (points == null)
      {
        return EmptyBitmap;
      }
      else
      {
        _groupedMarkers.Transform = Plotter2D.Transform;
        _groupedMarkers.LastSelection = LastSelectionArgs;
        return _groupedMarkers.DrawImage((int)output.Width, (int)output.Height, points, renderRequest);
      }
    }
  }
}
