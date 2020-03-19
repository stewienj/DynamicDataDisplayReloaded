using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;
using Microsoft.Research.DynamicDataDisplay.Maps.Servers.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Research.DynamicDataDisplay.Maps
{
  //---------------------------------------------------------------------------
  public class MapChart : ChartPlotter
  {
    //-------------------------------------------------------------------------
    protected MapChartViewModel Model { get { return DataContext as MapChartViewModel; } }

    //-------------------------------------------------------------------------
    public MapChart()
    {
      Viewport.EndPanning += Viewport_EndPanning;
      Viewport.PropertyChanged += Viewport_PropertyChanged;
      DataContextChanged += FlewseMapDisplay_DataContextChanged;
      MouseUp += FlewseMapDisplay_MouseUp;
      MouseDown += FlewseMapDisplay_MouseDown;
      KeyUp += FlewseMapDisplay_KeyUp;

      Legend.RemoveFromPlotter(this);

    }


    //-------------------------------------------------------------------------
    private void FlewseMapDisplay_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var oldModel = e.OldValue as MapChartViewModel;
      if (oldModel != null)
      {
        oldModel.CentreMapEvent -= Model_CentreMapEvent;
        oldModel.ChangeViewEvent -= Model_ChangeViewEvent;
        oldModel.MapGetSingleLocationEvent -= Model_MapGetSingleLocationEvent;
        oldModel.MapGetMultipleLocationEvent -= Model_MapGetMultipleLocationEvent;
        oldModel.CancelGetLocationCoordsEvent -= Model_CancelGetLocationCoordsEvent;
        oldModel.DataSourceChangedEvent -= Model_ChangeDataSourceEvent;
      }

      if (Model != null)
      {
        Model.Plotter = this;
        Model.CentreMapEvent += Model_CentreMapEvent;
        Model.ChangeViewEvent += Model_ChangeViewEvent;
        Model.MapGetSingleLocationEvent += Model_MapGetSingleLocationEvent;
        Model.MapGetMultipleLocationEvent += Model_MapGetMultipleLocationEvent;
        Model.CancelGetLocationCoordsEvent += Model_CancelGetLocationCoordsEvent;
        Model.DataSourceChangedEvent += Model_ChangeDataSourceEvent;

        Model.OnMapDisplayInitialised();
      }
    }

    #region WMS Map Source changing

    //-------------------------------------------------------------------------
    public Map GetMapComponent()
    {
      return Children.Where(c => c is Map).FirstOrDefault() as Map;
    }

    //-------------------------------------------------------------------------
    private void Model_ChangeDataSourceEvent(object sender, MapChartViewModel.DataSourceChangedEventArgs e)
    {
      var map = GetMapComponent();

      if (map != null && e.Status == MapChartViewModel.DataSourceChangedEventArgs.EStatus.SUCCESS)
      {
        map.SourceTileServer = e.TileSource;
        DataTransform = e.DataTransform;

        if (e.TileSource is WMSTileServer)
        {
          // this needs to be done after the SourceTileServer has been set
          var fts = map.FileTileServer as FileSystemTileServer;
          fts.CacheLocation = CacheLocation.CustomPath;
          fts.CachePath = System.IO.Path.Combine(WMSTileServer.CachePath, e.TileSource.ServerName);
        }
      }
    }

    #endregion


    #region Get Coordinate Clicks event handlers

    //-------------------------------------------------------------------------
    private void Model_CancelGetLocationCoordsEvent(object sender, EventArgs e)
    {
      FinishGetMouseClickLocations();
    }

    //-------------------------------------------------------------------------
    private void FlewseMapDisplay_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Pressed)
      {
        MouseButtonDownTime = DateTime.Now;
        MouseMoveStartPoint = e.GetPosition(CentralGrid);
      }
    }

    //-------------------------------------------------------------------------
    private void FlewseMapDisplay_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape && 
        (  MouseMode == EClickMode.MULTIPLE_COORD
        || MouseMode == EClickMode.SINGLE_COORD ))
      {
        FinishGetMouseClickLocations();
        e.Handled = true;
      }
    }

    //-------------------------------------------------------------------------
    private void FlewseMapDisplay_MouseUp(object sender, MouseButtonEventArgs e)
    {
      if (e.ChangedButton == MouseButton.Left)
      {
        if (MouseMode == EClickMode.SINGLE_COORD)
        {
          if (MouseClickCallbackFunc != null)
          {
            var coord = GetCursorLatLonDeg();
            MouseClickCallbackFunc?.Invoke(coord.Y, coord.X);
          }
          MouseClickCallbackCompleteFunc?.Invoke();
          Cursor = Cursors.Arrow;
          MouseMode = EClickMode.SELECT;
          MouseClickCallbackFunc = null;
          MouseClickCallbackCompleteFunc = null;
        }
        else if (MouseMode == EClickMode.MULTIPLE_COORD)
        {
          if (MouseClickCallbackFunc != null)
          {
            var coord = GetCursorLatLonDeg();
            MouseClickCallbackFunc(coord.Y, coord.X);
          }
        }

        e.Handled = true;
      }
      else if (e.ChangedButton == MouseButton.Right)
      {
        if (MouseMode == EClickMode.MULTIPLE_COORD)
        {
          FinishGetMouseClickLocations();
          MouseMode = EClickMode.SELECT;
          e.Handled = true;
        }
      }
    }


    //-------------------------------------------------------------------------
    public enum EClickMode { SELECT, SINGLE_COORD, MULTIPLE_COORD, DRAG, CUSTOM_1, CUSTOM_2, CUSTOM_3 };
    public EClickMode MouseMode = EClickMode.SELECT;
    public DateTime MouseButtonDownTime = DateTime.MinValue;
    public Point MouseMoveStartPoint = new Point();


    //-------------------------------------------------------------------------
    private MapChartViewModel.MapClickCallback MouseClickCallbackFunc = null;
    private MapChartViewModel.MapClickMultipleCompleteCallback MouseClickCallbackCompleteFunc = null;

    //-------------------------------------------------------------------------
    private void Model_MapGetSingleLocationEvent(object sender, MapChartViewModel.MapGetSingleLocationEventEventArgs e)
    {
      Focus();
      MouseMode = EClickMode.SINGLE_COORD;
      MouseClickCallbackFunc = e.Callback;
      MouseClickCallbackCompleteFunc = e.Finished;
      Cursor = Cursors.Cross;
    }

    //-------------------------------------------------------------------------
    private void Model_MapGetMultipleLocationEvent(object sender, MapChartViewModel.MapGetMultipleLocationEventEventArgs e)
    {
      Focus();
      MouseMode = EClickMode.MULTIPLE_COORD;
      MouseClickCallbackFunc = e.ClickCallback;
      MouseClickCallbackCompleteFunc = e.CompleteCallback;
      Cursor = Cursors.Cross;
    }

    #endregion

    #region View related event handlers

    //-------------------------------------------------------------------------
    private void Model_ChangeViewEvent(object sender, MapChartViewModel.ChangeViewEventEventArgs e)
    {
      ChangeView(e.CentreLatDeg, e.CentreLonDeg, e.WidthDeg, e.HeightDeg);
    }

    //-------------------------------------------------------------------------
    private void Model_CentreMapEvent(object sender, MapChartViewModel.CentreMapEventEventArgs e)
    {
      CentreOn(e.LatDeg, e.LonDeg);
    }

    //-------------------------------------------------------------------------
    // Called when the map is panned
    private void Viewport_EndPanning(object sender, EventArgs e)
    {
      if (Model != null)
      {
        Point centre = new Point(Visible.CenterX, Visible.CenterY);
        if (DataTransform != null)
        {
          centre = DataTransform.ViewportToData(centre);
        }

        Model.OnViewChanged(centre.Y, centre.X, Visible.Size.Width, Visible.Size.Height);
      }
    }

    //-------------------------------------------------------------------------
    // Called when the map zoom level changes
    private void Viewport_PropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
    {
      if (Model != null && e.PropertyName == nameof(Viewport.Visible))
      {
        Viewport_EndPanning(null, null);
      }
    }

    #endregion

    //-------------------------------------------------------------------------
    // Centres the map on the given point without changing the zoom level/width/height
    public void CentreOn(double latDeg, double lonDeg)
    {
      if (latDeg.IsNaN() || lonDeg.IsNaN()) return;

      var centre = new Point(lonDeg, latDeg);

      if (DataTransform != null)
      {
        centre = DataTransform.DataToViewport(centre);
      }

      centre.X -= (Visible.Size.Width / 2);
      centre.Y -= (Visible.Size.Height / 2);

      Visible = new DataRect(centre, Visible.Size);
    }

    //-------------------------------------------------------------------------
    // Centres the map on the given point and sets the view width and height
    public void ChangeView(double centreLatDeg, double centreLonDeg, double widthDeg, double heightDeg)
    {
      Point a = new Point(centreLonDeg - widthDeg / 2, centreLatDeg - heightDeg / 2);
      Point b = new Point(centreLonDeg + widthDeg / 2, centreLatDeg + heightDeg / 2);

      if (DataTransform != null)
      {
        a = DataTransform.DataToViewport(a);
        b = DataTransform.DataToViewport(b);
      }

      Dispatcher.Invoke(() =>
      {
        Visible = new DataRect(a, b);
      });
    }

    //-------------------------------------------------------------------------
    public Point GetCursorLatLonDeg()
    {
      Point ll = Mouse.GetPosition(CentralGrid);
      return ll.ScreenToData(Transform);
    }

    //-------------------------------------------------------------------------
    public void FinishGetMouseClickLocations()
    {
      MouseClickCallbackCompleteFunc?.Invoke();
      MouseClickCallbackCompleteFunc = null;
      MouseClickCallbackFunc = null;
      MouseMode = EClickMode.SELECT;
      Cursor = Cursors.Arrow;
    }


  }
}
