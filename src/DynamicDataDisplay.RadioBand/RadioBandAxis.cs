using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace DynamicDataDisplay.RadioBand
{
  /// <summary>
  /// Represents an axis with ticks of <see cref="System.DateTime"/> type.
  /// </summary>
  [TemplatePart(Name = "PART_ContentsGrid", Type = typeof(Grid))]
  public class RadioBandAxis : GeneralAxis
  {
    private const string _templateKey = "radioBandAxisControlTemplate";
    private Grid _mainGrid;
    protected Grid _scalableGrid;

    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimeAxis"/> class.
    /// </summary>
    public RadioBandAxis(AxisPlacement placement)
    {
      Focusable = false;

      Loaded += OnLoaded;

      Placement = placement;

      HorizontalContentAlignment = HorizontalAlignment.Stretch;
      VerticalContentAlignment = VerticalAlignment.Stretch;

      HorizontalAlignment = HorizontalAlignment.Stretch;
      VerticalAlignment = VerticalAlignment.Stretch;

      Focusable = false;
      UpdateUIResources();
    }

    private void UpdateUIResources()
    {
      ResourceDictionary resources = new ResourceDictionary
      {
        Source = new Uri("/DynamicDataDisplay.RadioBand;component/RadioBandAxisControlStyle.xaml", UriKind.Relative)
      };

      var template = (ControlTemplate)resources[_templateKey];
      var content = (FrameworkElement)template.LoadContent();

      _mainGrid = (Grid)content.FindName("PART_ContentsGrid");
      _scalableGrid = (Grid)content.FindName("scalableGrid");

      _mainGrid.SetBinding(Control.BackgroundProperty, new Binding { Path = new PropertyPath("Background"), Source = this });
      _mainGrid.SizeChanged += new SizeChangedEventHandler(MainGrid_SizeChanged);

      Content = _mainGrid;
    }

    void MainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (Placement.IsBottomOrTop() && e.WidthChanged ||
         e.HeightChanged)
      {
        // this is performed because if not, whole axisControl's size was measured wrongly.
        InvalidateMeasure();
        UpdateUI();
      }
    }

    /// <summary>
    /// Ensure that this is in the Horizontal placement
    /// </summary>
    /// <param name="newPlacement"></param>
    protected override void ValidatePlacement(AxisPlacement newPlacement)
    {
      /*
      if (newPlacement == AxisPlacement.Left || newPlacement == AxisPlacement.Right)
        throw new ArgumentException(Strings.Exceptions.HorizontalAxisCannotBeVertical);
      */
    }

    internal void UpdateUI()
    {
      switch (Placement)
      {
        case AxisPlacement.Left:
        case AxisPlacement.Right:
          var marginHeight = -_transform.ScreenRect.Height / _transform.ViewportRect.Size.Height;
          _scalableGrid.Margin = new Thickness(0,(1.0- _transform.ViewportRect.YMax) * marginHeight, 0, (_transform.ViewportRect.YMin) * marginHeight);
          break;
        case AxisPlacement.Bottom:
        case AxisPlacement.Top:
          var marginWidth = -_transform.ScreenRect.Width / _transform.ViewportRect.Size.Width;
          _scalableGrid.Margin = new Thickness(_transform.ViewportRect.XMin * marginWidth, 0, (1.0 - _transform.ViewportRect.XMax) * marginWidth, 0);
          break;
      }

      // Do this last to trigger changes up the chain
      RaiseTicksChanged();
    }

    public override void ForceUpdate() => UpdateUI();


    private void OnLoaded(object sender, RoutedEventArgs e) => RaiseTicksChanged();

    private ITicksProvider<double> _ticksProvider;
    /// <summary>
    /// Gets or sets the ticks provider - generator of ticks for given range.
    /// 
    /// Should not be null.
    /// </summary>
    /// <value>The ticks provider.</value>
    public ITicksProvider<double> TicksProvider
    {
      get => _ticksProvider;
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");

        if (_ticksProvider != value)
        {
          DetachTicksProvider();
          _ticksProvider = value;
          AttachTicksProvider();
          UpdateUI();
        }
      }
    }

    public override MajorTickInfo<double>[] MajorScreenTicks => throw new NotImplementedException();

    public override MinorTickInfo<double>[] MinorScreenTicks => throw new NotImplementedException();

    private void AttachTicksProvider()
    {
      if (_ticksProvider != null)
      {
        _ticksProvider.Changed += TicksProvider_Changed;
      }
    }

    private void DetachTicksProvider()
    {
      if (_ticksProvider != null)
      {
        _ticksProvider.Changed -= TicksProvider_Changed;
      }
    }

    private void TicksProvider_Changed(object sender, EventArgs e) => UpdateUI();


    protected override void OnPlotterAttached(Plotter2D plotter)
    {
      plotter.Viewport.PropertyChanged += OnViewportPropertyChanged;

      Panel panel = GetPanelByPlacement(Placement);

      if (panel != null)
      {
        int index = GetInsertionIndexByPlacement(Placement, panel);
        panel.Children.Insert(index, this);
      }

      TicksProvider = ((HorizontalAxis)(((RadioBandChartPlotter)plotter).MainHorizontalAxis)).TicksProvider;

      UpdateAxisControl(plotter.Viewport);
    }


    private CoordinateTransform _transform = CoordinateTransform.CreateDefault();
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public CoordinateTransform Transform
    {
      get => _transform;
      set
      {
        _transform = value;
        if (_updateOnCommonChange)
        {
          UpdateUI();
        }
      }
    }


    private Range<double> _range;
    /// <summary>
    /// Gets or sets the range, which ticks are generated for.
    /// </summary>
    /// <value>The range.</value>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Range<double> Range
    {
      get => _range;
      set
      {
        _range = value;
        if (_updateOnCommonChange)
        {
          UpdateUI();
        }
      }
    }


    private bool _updateOnCommonChange = true;
    protected void UpdateAxisControl(Viewport2D viewPort)
    {
      _updateOnCommonChange = false;
      Transform = viewPort.Transform;
      Range = CreateRangeFromRect(viewPort.Visible.ViewportToData(viewPort.Transform));
      _updateOnCommonChange = true;
      UpdateUI();
    }

    private int GetInsertionIndexByPlacement(AxisPlacement placement, Panel panel)
    {
      int index = panel.Children.Count;

      switch (placement)
      {
        case AxisPlacement.Left:
          index = 0;
          break;
        case AxisPlacement.Top:
          index = 0;
          break;
        default:
          break;
      }

      return index;
    }

    ExtendedPropertyChangedEventArgs _visibleChangedEventArgs;
    int _viewportPropertyChangedEnters = 0;
    DataRect _prevDataRect = DataRect.Empty;
    protected void OnViewportPropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
    {
      if (_viewportPropertyChangedEnters > 4)
      {
        if (e.PropertyName == "Visible")
        {
          _visibleChangedEventArgs = e;
        }
        return;
      }

      _viewportPropertyChangedEnters++;

      Viewport2D viewport = (Viewport2D)sender;

      DataRect visible = viewport.Visible;

      DataRect dataRect = visible.ViewportToData(viewport.Transform);
      bool forceUpdate = dataRect != _prevDataRect;
      _prevDataRect = dataRect;

      Range<double> range = CreateRangeFromRect(dataRect);

      UpdateAxisControl(viewport);

      Dispatcher.BeginInvoke(() =>
      {
        _viewportPropertyChangedEnters--;
        if (_visibleChangedEventArgs != null)
        {
          OnViewportPropertyChanged(Plotter.Viewport, _visibleChangedEventArgs);
        }
        _visibleChangedEventArgs = null;
      }, DispatcherPriority.Render);
    }


    private Range<double> CreateRangeFromRect(DataRect visible)
    {
      double min, max;

      Range<double> range;
      switch (Placement)
      {
        case AxisPlacement.Left:
        case AxisPlacement.Right:
          min = visible.YMin;
          max = visible.YMax;
          break;
        case AxisPlacement.Top:
        case AxisPlacement.Bottom:
          min = visible.XMin;
          max = visible.XMax;
          break;
        default:
          throw new NotSupportedException();
      }

      range = new Range<double>(Math.Min(min, max), Math.Max(min, max));
      return range;
    }

    protected override void OnPlacementChanged(AxisPlacement oldPlacement, AxisPlacement newPlacement)
    {
      if (ParentPlotter != null)
      {
        Panel panel = GetPanelByPlacement(oldPlacement);
        panel.Children.Remove(this);

        Panel newPanel = GetPanelByPlacement(newPlacement);
        int index = GetInsertionIndexByPlacement(newPlacement, newPanel);
        newPanel.Children.Insert(index, this);
      }
    }

    protected override void OnPlotterDetaching(Plotter2D plotter)
    {
      if (plotter == null)
        return;

      Panel panel = GetPanelByPlacement(Placement);
      if (panel != null)
      {
        panel.Children.Remove(this);
      }

      plotter.Viewport.PropertyChanged -= OnViewportPropertyChanged;
    }
  }
}
