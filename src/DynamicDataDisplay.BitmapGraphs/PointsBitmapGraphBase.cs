using Microsoft.Research.DynamicDataDisplay.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Microsoft.Research.DynamicDataDisplay.BitmapGraphs
{
  public abstract class PointsBitmapGraphBase : BitmapBasedGraph
  {
    protected PointsAggregatorAndSelector _pointsCalculator = null;
    private AggregatedPointsChangedArgs _lastPointsArgs = null;
    private SelectedPointsChangedArgs _lastSelectionArgs = null;
    private ArrayBitmapSource<uint> _emptyBitmap = null;

    public PointsBitmapGraphBase()
    {
      IsHitTestVisible = false;
      _emptyBitmap = new ArrayBitmapSource<uint>();
      _emptyBitmap.Freeze();
    }


    public PointsBitmapGraphBase(double aggregatedBlockPixelWidth, double aggregatedBlockPixelHeight) : this()
    {
      _pointsCalculator = new PointsAggregatorAndSelector(this) { AggregatedBlockPixelWidth = aggregatedBlockPixelWidth, AggregatedBlockPixelHeight = aggregatedBlockPixelHeight };
      _pointsCalculator.NewPointsReady += PointsCalculator_NewPointsReady;
      _pointsCalculator.SelectedPointsChanged += PointsCalculator_SelectedPointsChanged;
    }

    public void SetPointToLatLonDegConverter<T>(Func<T, Point> pointToLatLonDegConverter)
    {
      Func<object, Point> converter = x => pointToLatLonDegConverter((T)x);
      PointToLatLonDegConverter = converter;
    }

    public Func<object, Point> PointToLatLonDegConverter
    {
      get { return (Func<object, Point>)GetValue(PointToLatLonDegConverterProperty); }
      set { SetValue(PointToLatLonDegConverterProperty, value); }
    }

    // Using a DependencyProperty as the backing store for PointsSource.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty PointToLatLonDegConverterProperty =
        DependencyProperty.Register("PointToLatLonDegConverter", typeof(Func<object, Point>), typeof(PointsBitmapGraphBase), new PropertyMetadata(null, (s, e) =>
        {
          if (s is PointsBitmapGraphBase control)
          {
            var converter = e.NewValue as Func<object, Point>;
            control._pointsCalculator.SetPointObjectToPointConverter(converter);
          }
        }));


    public IEnumerable PointsSource
    {
      get { return (IEnumerable)GetValue(PointsSourceProperty); }
      set { SetValue(PointsSourceProperty, value); }
    }

    // Using a DependencyProperty as the backing store for PointsSource.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty PointsSourceProperty =
        DependencyProperty.Register("PointsSource", typeof(IEnumerable), typeof(PointsBitmapGraphBase), new PropertyMetadata(null, (s, e) =>
        {
          if (s is PointsBitmapGraphBase control)
          {
            var pointsSource = e.NewValue as IEnumerable<object>;
            control._pointsCalculator.SetPointsSource(pointsSource);
            if (e.OldValue is INotifyCollectionChanged oldCollectionChanged)
            {
              oldCollectionChanged.CollectionChanged -= control.CollectionChanged_CollectionChanged;
            }
            if (e.NewValue is INotifyCollectionChanged newCollectionChanged)
            {
              newCollectionChanged.CollectionChanged += control.CollectionChanged_CollectionChanged;
            }
          }
        }));

    public IEnumerable SelectedPoints
    {
      get { return (IEnumerable)GetValue(SelectedPointsProperty); }
      set { SetValue(SelectedPointsProperty, value); }
    }

    // Using a DependencyProperty as the backing store for SelectedPointsDeg.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SelectedPointsProperty =
        DependencyProperty.Register("SelectedPoints", typeof(IEnumerable), typeof(PointsBitmapGraphBase), new FrameworkPropertyMetadata { BindsTwoWayByDefault = true });

    public Point SelectedPointsLocation
    {
      get { return (Point)GetValue(SelectedPointsLocationProperty); }
      set { SetValue(SelectedPointsLocationProperty, value); }
    }

    // Using a DependencyProperty as the backing store for SelectedPointsDeg.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SelectedPointsLocationProperty =
        DependencyProperty.Register("SelectedPointsLocation", typeof(Point), typeof(PointsBitmapGraphBase), new FrameworkPropertyMetadata { BindsTwoWayByDefault = true });

    public (IEnumerable Points,Point Location) SelectedPointsAndLocation
    {
      get { return ((IEnumerable Points, Point Location))GetValue(SelectedPointsAndLocationProperty); }
      set { SetValue(SelectedPointsAndLocationProperty, value); }
    }

    // Using a DependencyProperty as the backing store for SelectedPointsDeg.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SelectedPointsAndLocationProperty =
        DependencyProperty.Register("SelectedPointsAndLocation", typeof((IEnumerable Points, Point Location)), typeof(PointsBitmapGraphBase), new FrameworkPropertyMetadata { BindsTwoWayByDefault = true });

    private bool _debugModeOn = false;
    public bool DebugModeOn
    {
      get { return (bool)GetValue(DebugModeOnProperty); }
      set { SetValue(DebugModeOnProperty, value); }
    }

    // Using a DependencyProperty as the backing store for DebugModeOn.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty DebugModeOnProperty =
        DependencyProperty.Register("DebugModeOn", typeof(bool), typeof(PointsBitmapGraphBase), new PropertyMetadata(false, (s,e)=>
        {
          if (s is PointsBitmapGraphBase control)
          {
            if (e.NewValue is bool debugModeOn)
            {
              // Need to keep this state in a non DependencyProperty as it needs to be read on a background thread
              control._debugModeOn = debugModeOn;
            }
            control.UpdateVisualization();
          }
        }));

    private BitmapDebugType _debugType = BitmapDebugType.BitmapBoundary;
    public BitmapDebugType DebugType
    {
      get { return (BitmapDebugType)GetValue(DebugTypeProperty); }
      set { SetValue(DebugTypeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for DebugModeOn.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty DebugTypeProperty =
        DependencyProperty.Register("DebugType", typeof(BitmapDebugType), typeof(PointsBitmapGraphBase), new PropertyMetadata(BitmapDebugType.BitmapBoundary, (s, e) =>
        {
          if (s is PointsBitmapGraphBase control)
          {
            if (e.NewValue is BitmapDebugType debugType)
            {
              // Need to keep this state in a non DependencyProperty as it needs to be read on a background thread
              control._debugType = debugType;
            }
            control.UpdateVisualization();
          }
        }));

    private void CollectionChanged_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      _pointsCalculator.Recalculate();
    }

    public void CreateRandomMapData()
    {
      var points = new List<Point>();
      Random random = new Random(100);
      for (int i = 0; i < 500_000; ++i)
      {
        var x = random.NextDouble() * 360 - 180;
        var y = random.NextDouble() * 175 - 87.5;
        points.Add(new Point(x,y));
      }
      var victoriaSquareAdelaide = new Point(138.599960, -34.928625);
      points.Add(victoriaSquareAdelaide);
      _pointsCalculator.SetPointObjectToPointConverter(p => (Point)p);
      _pointsCalculator.SetPointsSource(points);
    }

    protected void PointsCalculator_NewPointsReady(object sender, AggregatedPointsChangedArgs e)
    {
      _lastPointsArgs = e;
      Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() => UpdateVisualization()));
    }

    protected void PointsCalculator_SelectedPointsChanged(object sender, SelectedPointsChangedArgs e)
    {
      _lastSelectionArgs = e;
      Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() =>
      {
        SelectedPointsAndLocation = (e.SelectedPoints, e.SelectionLocation);
        SelectedPointsLocation = e.SelectionLocation;
        SelectedPoints = e.SelectedPoints;
        UpdateVisualization();
      }));
    }

    protected override void OnPlotterAttached(Plotter plotter)
    {
      _pointsCalculator.Plotter = plotter as Plotter2D;
      base.OnPlotterAttached(plotter);
      _pointsCalculator.Recalculate();
    }

    protected override void OnPlotterDetaching(Plotter plotter)
    {
      _pointsCalculator.Plotter = null;
      base.OnPlotterDetaching(plotter);
    }

    protected override BitmapSource RenderFrame(DataRect data, Rect output, RenderRequest renderRequest)
    {
      var dataFrame = RenderDataFrame(data, output, renderRequest);

      if (!_debugModeOn || dataFrame == null)
      {
        return dataFrame;
      }
      else
      {
        BitmapSource debugFrame = null;
        switch (_debugType)
        {
          case BitmapDebugType.BitmapBoundary:
            debugFrame = RenderDiagnostics(data, output);
            break;
          case BitmapDebugType.GridCentrePoints:
          case BitmapDebugType.GridBoxes:
            debugFrame = _pointsCalculator.RenderAggregationGrid(data, output, _debugType);
            break;
        }

        return !renderRequest.IsCancellingRequested ?
          CombineFrames(new[] { dataFrame, debugFrame }, output) :
          null;
      }
    }

    protected abstract BitmapSource RenderDataFrame(DataRect data, Rect output, RenderRequest renderRequest);

    private BitmapSource CombineFrames(IEnumerable<BitmapSource> sources, Rect output)
    {
      DrawingVisual drawingVisual = new DrawingVisual();
      using (DrawingContext drawingContext = drawingVisual.RenderOpen())
      {
        foreach (var source in sources)
        {
          if (source != null)
          {
            drawingContext.DrawImage(source, output);
          }
        }
      }

      RenderTargetBitmap rtb = new RenderTargetBitmap((int)output.Width, (int)output.Height, 96, 96, PixelFormats.Default);
      rtb.Render(drawingVisual);
      rtb.Freeze();
      return rtb;
    }

    /// <summary>
    /// This just renders the coordinates of the corners and a timestamp onto the screen,
    /// as well as a blue border around the edge of the bitmap.
    /// </summary>
    /// <param name="dataRect"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    private BitmapSource RenderDiagnostics(DataRect dataRect, Rect output)
    {
      Grid grid = new Grid();
      grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
      grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
      grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

      TextBlock lt = new TextBlock
      {
        VerticalAlignment = VerticalAlignment.Top,
        HorizontalAlignment = HorizontalAlignment.Left,
        Padding = new Thickness(16),
        Text = String.Format("({0},{1})", dataRect.XMin, dataRect.YMax)
      };
      grid.Children.Add(lt);
      Grid.SetRow(lt, 0);


      TextBlock rb = new TextBlock
      {
        VerticalAlignment = VerticalAlignment.Bottom,
        HorizontalAlignment = HorizontalAlignment.Right,
        Padding = new Thickness(16),
        Text = String.Format("({0},{1})", dataRect.XMax, dataRect.YMin)
      };
      grid.Children.Add(rb);
      Grid.SetRow(rb, 2);
      Border border = new Border();
      border.BorderThickness = new Thickness(2);
      border.BorderBrush = Brushes.Blue;
      border.Child = grid;

      RenderTargetBitmap rtb = new RenderTargetBitmap((int)output.Width, (int)output.Height, 96, 96, PixelFormats.Default);
      border.Measure(new Size(output.Width, output.Height));
      border.Arrange(output);
      rtb.Render(border);


      rtb.Freeze();
      return rtb;
    }

    public IEnumerable<(Point Point, int Count, Rect Bin)> GetLastPointsInDataRect(DataRect data)
    {
      var pointsInfo = _lastPointsArgs;
      if (pointsInfo == null || data.Width == 0 || data.Height == 0 || pointsInfo.ViewportAggregatedPoints.Count == 0)
        return null;

      double widthScaler = 1.0 / data.Width;
      double heightScaler = 1.0 / data.Height;

      Func<Point, Point> scalePoint = p => new Point((p.X - data.Location.X) * widthScaler, (data.Location.Y - p.Y + data.Height) * heightScaler);

      var points = pointsInfo
          .ViewportAggregatedPoints
          .AsParallel()
          .Where(l => data.Contains(l.Point))
          .Select(l => (
            scalePoint(l.Point)
            , l.Count
            , new Rect(l.Bin.TopLeft, l.Bin.BottomRight)
          ));

      return points;
    }

    public ArrayBitmapSource<uint> EmptyBitmap => _emptyBitmap;

    protected SelectedPointsChangedArgs LastSelectionArgs => _lastSelectionArgs;
  }

  public enum BitmapDebugType
  {
    BitmapBoundary,
    GridCentrePoints,
    GridBoxes
  }
}
