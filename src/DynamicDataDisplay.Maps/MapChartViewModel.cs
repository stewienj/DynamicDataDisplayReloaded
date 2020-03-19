using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;
using Microsoft.Research.DynamicDataDisplay.Maps.Servers.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Maps
{
	//---------------------------------------------------------------------------
	public class MapChartViewModel : INotifyPropertyChanged
	{
		private Dictionary<ViewportShape, IEnumerable<ViewportShape>> _itemToExtendedMapItems = new Dictionary<ViewportShape, IEnumerable<ViewportShape>>();

		//-------------------------------------------------------------------------
		public MapViewPosition CurrentMapPosition { get; private set; } = new MapViewPosition();

		//-------------------------------------------------------------------------
		// Called by the map display whenever the visible limits change (panned / zoomed)
		public virtual void OnViewChanged(double centreLatDeg, double centreLonDeg, double widthDeg, double heightDeg)
		{
			CurrentMapPosition.Update(centreLatDeg, centreLonDeg, widthDeg, heightDeg, Map.TileProvider.Level);
		}

		//-------------------------------------------------------------------------
		// Called by the map to let the model know the map has been created and is attached to this model
		public virtual void OnMapDisplayInitialised()
		{

		}



		//-------------------------------------------------------------------------
		public DataTransform MapTransform
		{
			get { return Plotter?.DataTransform; }
		}

		//-------------------------------------------------------------------------
		public CoordinateTransform CoordinateTransform
		{
			get { return Plotter?.Transform; }
		}

		//-------------------------------------------------------------------------
		private Plotter2D mPlotter = null;
		public Plotter2D Plotter
		{
			get { return mPlotter; }
			set { SetValue(ref mPlotter, value); }
		}

		//-------------------------------------------------------------------------
		private Map mMap = null;
		public Map Map
		{
			get { return mMap; }
			set
			{
				if (SetValue(ref mMap, value) && value != null)
				{
					HostPanel = new ViewportHostPanel();
					Map.Children.Add(HostPanel);
					Map.TileSystem.ModeChanged += TileSystem_ModeChanged;
				}
			}
		}

		//-------------------------------------------------------------------------
		private TileSystemMode mMapCacheMode = TileSystemMode.CacheOnly;
		public TileSystemMode MapCacheMode
		{
			get { return mMapCacheMode; }
			set { SetValue(ref mMapCacheMode, value); }
		}

		//-------------------------------------------------------------------------
		private void TileSystem_ModeChanged(object sender, Microsoft.Research.DynamicDataDisplay.Maps.Servers.ModeChangedEventArgs e)
		{
			MapCacheMode = e.Mode;
		}

		//-------------------------------------------------------------------------
		private ViewportHostPanel mHostPanel = null;
		public ViewportHostPanel HostPanel
		{
			get { return mHostPanel; }
			set { SetValue(ref mHostPanel, value); }
		}

		//-------------------------------------------------------------------------
		// Use ViewportPanel.SetViewportBounds(element, new DataRect(P1, P2)) to 
		// set location of the element on the map, in Viewport co-ords
		public void AddElement(FrameworkElement element)
		{
			if (element is IPlotterElement)
			{
				throw new ArgumentException("IPlotterElements should use the AddToPlotter() method.");
			}

			if (HostPanel != null)
			{
				HostPanel.Children.Add(element);
			}
		}

		//-------------------------------------------------------------------------
		public void RemoveElement(FrameworkElement element)
		{
			if (element is IPlotterElement)
			{
				throw new ArgumentException("IPlotterElements should use the RemoveFromPlotter() method.");
			}

			if (HostPanel != null)
			{
				HostPanel.Children.Remove(element);
			}
		}

		//-------------------------------------------------------------------------
		// Points are in Data (world) co-ordinates
		public void AddElementWithFixedCoords(FrameworkElement element, Point northWestDeg, Point southEastDeg)
		{
			if (element == null || HostPanel == null) return;

			var a = northWestDeg.DataToViewport(MapTransform);
			var b = southEastDeg.DataToViewport(MapTransform);

			ViewportPanel.SetX(element, 0.5 * (a.X + b.X));
			ViewportPanel.SetY(element, 0.5 * (a.Y + b.Y));
			ViewportPanel.SetViewportWidth(element, Math.Abs(a.X - b.X));
			ViewportPanel.SetViewportHeight(element, Math.Abs(a.Y - b.Y));

			AddElement(element);
		}

		//-------------------------------------------------------------------------
		// Makes sure the lat/lon point is visible in the current map view, if not
		// centre the view on the co-ordinates without changing the zoom level.
		public void EnsurePointIsInView(double latitudeDeg, double longitudeDeg)
		{
			if (CurrentMapPosition == null || !CurrentMapPosition.CanSeePoint(latitudeDeg, longitudeDeg))
			{
				CentreMap(latitudeDeg, longitudeDeg);
			}
		}


		//-------------------------------------------------------------------------
		public class MapViewPosition : INotifyPropertyChanged
		{
			public double CentreLatDeg { get; set; }
			public double CentreLonDeg { get; set; }
			public double WidthDeg { get; set; }
			public double HeightDeg { get; set; }

			//-------------------------------------------------------------------------
			private double mZoomLevel = 1;
			public double ZoomLevel
			{
				get { return mZoomLevel; }
				set { SetValue(ref mZoomLevel, value); }
			}


			//-------------------------------------------------------------------------
			public void Update(double centreLatDeg, double centreLonDeg, double widthDeg, double heightDeg, double level)
			{
				CentreLatDeg = centreLatDeg;
				CentreLonDeg = centreLonDeg;
				WidthDeg = widthDeg;
				HeightDeg = heightDeg;
				ZoomLevel = level;
			}

			//-------------------------------------------------------------------------
			// Returns true if the co-ordinates are within the view bounds
			public bool CanSeePoint(double latDeg, double lonDeg)
			{
				double minLon = (CentreLonDeg - WidthDeg / 2);
				double maxLon = (CentreLonDeg + WidthDeg / 2);
				double minLat = (CentreLatDeg - HeightDeg / 2);
				double maxLat = (CentreLatDeg + HeightDeg / 2);

				return minLon <= lonDeg && lonDeg <= maxLon
					&& minLat <= latDeg && latDeg <= maxLat;
			}

			#region INotifyPropertyChanged Members
			//-----------------------------------------------------------------------------
			protected void OnPropertyChanged(string propertyName)
			{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
				}
			}

			//-------------------------------------------------------------------------
			protected bool SetValue<T>(ref T current, T newValue, [CallerMemberName] string propertyName = "")
			{
				if (!EqualityComparer<T>.Default.Equals(current, newValue))
				{
					current = newValue;
					OnPropertyChanged(propertyName);

					return true;
				}

				return false;
			}

			//-------------------------------------------------------------------------
			protected bool SetValue<T>(ref T current, T newValue, string[] propertyNames)
			{
				if (!EqualityComparer<T>.Default.Equals(current, newValue))
				{
					current = newValue;
					foreach (string name in propertyNames)
						OnPropertyChanged(name);

					return true;
				}

				return false;
			}

			public event PropertyChangedEventHandler PropertyChanged;

			#endregion
		}

		#region DataSourceChangedEvent event source

		//-------------------------------------------------------------------------
		public class DataSourceChangedEventArgs : System.EventArgs
		{

			public enum EStatus { SUCCESS, FAIL }

			public DataSourceChangedEventArgs(SourceTileServer tileSource, DataTransform dataTransform, EStatus status, string statusMessage = "SUCCESS")
			{
				TileSource = tileSource;
				DataTransform = dataTransform;
				Status = status;
				StatusMessage = statusMessage;
			}

			public SourceTileServer TileSource { get; private set; }
			public DataTransform DataTransform { get; private set; }

			public EStatus Status { get; private set; }
			public string StatusMessage { get; private set; }
		}

		//-------------------------------------------------------------------------
		public event EventHandler<DataSourceChangedEventArgs> DataSourceChangedEvent;

		//-------------------------------------------------------------------------
		protected virtual void RaiseDataSourceChangedEvent(DataSourceChangedEventArgs e)
		{
			DataSourceChangedEvent?.Invoke(this, e);
		}

		//-------------------------------------------------------------------------
		public void ChangeServer(SourceTileServer server, DataTransform dataTransform)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				try
				{
					RaiseDataSourceChangedEvent(new DataSourceChangedEventArgs(server, dataTransform, DataSourceChangedEventArgs.EStatus.SUCCESS));
					OnPropertyChanged(nameof(MapTransform));
					OnPropertyChanged(nameof(CoordinateTransform));
				}
				catch (Exception ex)
				{
					RaiseDataSourceChangedEvent(new DataSourceChangedEventArgs(null, null, DataSourceChangedEventArgs.EStatus.FAIL, ex.Message));
				}
			});
		}

		//-------------------------------------------------------------------------
		public void ChangeServer(string url, string layer, int maxLevel)
		{
			Wms.Client.Server service = null;
			try
			{
				service = Wms.Client.Server.CreateService(url, WMSTileServer.CachePath);
			}
			catch (Exception ex)
			{
				RaiseDataSourceChangedEvent(new DataSourceChangedEventArgs(null, null, DataSourceChangedEventArgs.EStatus.FAIL, ex.Message));
				service = null;
			}

			if (service != null)
			{
				Application.Current.Dispatcher.Invoke(() =>
				{
					try
					{
						var wmsServer = new WMSTileServer(service, layer, maxLevel);
						if (wmsServer.Layer != null && wmsServer.SRSConverter != null)
						{
							RaiseDataSourceChangedEvent(new DataSourceChangedEventArgs(wmsServer, wmsServer.SRSConverter.DataTransform, DataSourceChangedEventArgs.EStatus.SUCCESS));
							OnPropertyChanged(nameof(MapTransform));
							OnPropertyChanged(nameof(CoordinateTransform));
						}
					}
					catch (Exception ex)
					{
						RaiseDataSourceChangedEvent(new DataSourceChangedEventArgs(null, null, DataSourceChangedEventArgs.EStatus.FAIL, ex.Message));
					}
				});
			}
		}

		//-------------------------------------------------------------------------
		public async Task ChangeServerAsync(string url, string layer, int maxLevel)
		{
			await Task.Run(() => { ChangeServer(url, layer, maxLevel); });
		}

		#endregion


		#region CentreMapEvent event source

		//-------------------------------------------------------------------------
		public void CentreMap(double latDeg, double lonDeg)
		{
			RaiseCentreMapEvent(new CentreMapEventEventArgs(latDeg, lonDeg));
		}

		//-------------------------------------------------------------------------
		public class CentreMapEventEventArgs : System.EventArgs
		{
			public CentreMapEventEventArgs(double latDeg, double lonDeg)
			{
				LatDeg = latDeg;
				LonDeg = lonDeg;
			}

			public double LatDeg { get; private set; }
			public double LonDeg { get; private set; }
		}

		//-------------------------------------------------------------------------
		public event EventHandler<CentreMapEventEventArgs> CentreMapEvent;

		//-------------------------------------------------------------------------
		protected void RaiseCentreMapEvent(CentreMapEventEventArgs e)
		{
			CentreMapEvent?.Invoke(this, e);
		}

		#endregion

		#region ChangeViewEvent event source

		//-------------------------------------------------------------------------
		public void ChangeView(double centreLatDeg, double centreLonDeg, double widthDeg, double heightDeg)
		{
			RaiseChangeViewEvent(new ChangeViewEventEventArgs(centreLatDeg, centreLonDeg, widthDeg, heightDeg));

			double level = 1.0;
			if (Map?.TileProvider != null) level = Map.TileProvider.Level;

			CurrentMapPosition.Update(centreLatDeg, centreLonDeg, widthDeg, heightDeg, level);
		}

		//-------------------------------------------------------------------------
		public class ChangeViewEventEventArgs : System.EventArgs
		{
			public ChangeViewEventEventArgs(double centreLatDeg, double centreLonDeg, double widthDeg, double heightDeg)
			{
				CentreLatDeg = centreLatDeg;
				CentreLonDeg = centreLonDeg;
				WidthDeg = widthDeg;
				HeightDeg = heightDeg;
			}

			public double CentreLatDeg { get; private set; }
			public double CentreLonDeg { get; private set; }
			public double WidthDeg { get; private set; }
			public double HeightDeg { get; private set; }
		}

		//-------------------------------------------------------------------------
		public event EventHandler<ChangeViewEventEventArgs> ChangeViewEvent;

		//-------------------------------------------------------------------------
		public void RaiseChangeViewEvent(ChangeViewEventEventArgs e)
		{
			ChangeViewEvent?.Invoke(this, e);
		}

		#endregion

		#region Add / Remove plotter objects

		//-------------------------------------------------------------------------
		public void AddToPlotter(IEnumerable<IPlotterElement> items)
		{
			// What we should have here is a content layer that we add content to
			Plotter?.Dispatcher?.Invoke(() =>
			{
				foreach (var item in items)
				{
					Plotter.Children.Add(item);
				}
			});
		}

		//-------------------------------------------------------------------------
		public void AddToPlotter(IPlotterElement item)
		{
			// What we should have here is a content layer that we add content to.
			Plotter?.Dispatcher?.Invoke(() =>
			{
				Plotter.Children.Add(item);
			});
		}


		//-------------------------------------------------------------------------
		public void RemoveFromPlotter(IEnumerable<IPlotterElement> items)
		{
			Plotter?.Dispatcher?.Invoke(() =>
			{
				foreach (var item in items)
				{
					Plotter.Children.Remove(item);
				}
			});
		}

		//-------------------------------------------------------------------------
		public void RemoveFromPlotter(IPlotterElement item)
		{
			Plotter?.Dispatcher?.Invoke(() =>
			{
				Plotter.Children.Remove(item);
			});
		}

		#endregion

		//-------------------------------------------------------------------------
		public void GetMapCursorLocation(out double latDeg, out double lonDeg)
		{
			var ll = GetMapCursorLocation();

			latDeg = ll.Y;
			lonDeg = ll.X;
		}

		//-------------------------------------------------------------------------
		public Point GetMapCursorLocation()
		{
			var chart = Plotter as MapChart;
			if (chart != null)
			{
				return chart.GetCursorLatLonDeg();
			}
			return new Point(0.0, 0.0);
		}

		public delegate void MapClickCallback(double latDeg, double lonDeg);
		public delegate void MapClickMultipleCompleteCallback();


		#region CancelGetLocationCoordsEvent event source

		//-------------------------------------------------------------------------
		public void CancelGetLocationCoords()
		{
			RaiseCancelGetLocationCoordsEvent();
		}

		//-------------------------------------------------------------------------
		public event EventHandler<EventArgs> CancelGetLocationCoordsEvent;

		//-------------------------------------------------------------------------
		protected void RaiseCancelGetLocationCoordsEvent()
		{
			CancelGetLocationCoordsEvent?.Invoke(this, EventArgs.Empty);
		}

		#endregion

		#region MapGetSingleLocationEvent event source


		//-------------------------------------------------------------------------
		public void GetSingleLocationCoords(MapClickCallback callback)
		{
			RaiseMapGetSingleLocationEvent(new MapGetSingleLocationEventEventArgs(callback));
		}

		//-------------------------------------------------------------------------
		public void GetSingleLocationCoords(MapClickCallback callback, MapClickMultipleCompleteCallback finished)
		{
			RaiseMapGetSingleLocationEvent(new MapGetSingleLocationEventEventArgs(callback, finished));
		}

		//-------------------------------------------------------------------------
		public event EventHandler<MapGetSingleLocationEventEventArgs> MapGetSingleLocationEvent;

		//-------------------------------------------------------------------------
		public class MapGetSingleLocationEventEventArgs : System.EventArgs
		{
			public MapGetSingleLocationEventEventArgs(MapClickCallback callback)
			{
				Callback = callback;
				Finished = null;
			}
			public MapGetSingleLocationEventEventArgs(MapClickCallback callback, MapClickMultipleCompleteCallback finished)
			{
				Callback = callback;
				Finished = finished;
			}

			public MapClickCallback Callback { get; private set; }

			public MapClickMultipleCompleteCallback Finished { get; private set; }
		}

		//-------------------------------------------------------------------------
		protected void RaiseMapGetSingleLocationEvent(MapGetSingleLocationEventEventArgs e)
		{
			MapGetSingleLocationEvent?.Invoke(this, e);
		}


		#endregion

		#region MapGetMultipleLocationEvent event source

		//-------------------------------------------------------------------------
		public void GetMultipleLocationCoords(MapClickCallback onClick, MapClickMultipleCompleteCallback onComplete)
		{
			RaiseMapGetMultipleLocationEvent(new MapGetMultipleLocationEventEventArgs(onClick, onComplete));
		}

		//-------------------------------------------------------------------------
		public class MapGetMultipleLocationEventEventArgs : System.EventArgs
		{
			public MapGetMultipleLocationEventEventArgs(MapClickCallback onClick, MapClickMultipleCompleteCallback onComplete)
			{
				ClickCallback = onClick;
				CompleteCallback = onComplete;
			}

			public MapClickCallback ClickCallback { get; private set; }
			public MapClickMultipleCompleteCallback CompleteCallback { get; private set; }
		}

		//-------------------------------------------------------------------------
		public event EventHandler<MapGetMultipleLocationEventEventArgs> MapGetMultipleLocationEvent;

		//-------------------------------------------------------------------------
		protected void RaiseMapGetMultipleLocationEvent(MapGetMultipleLocationEventEventArgs e)
		{
			MapGetMultipleLocationEvent?.Invoke(this, e);
		}

		#endregion

		#region INotifyPropertyChanged Members
		//-----------------------------------------------------------------------------
		protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		//-------------------------------------------------------------------------
		protected bool SetValue<T>(ref T current, T newValue, [CallerMemberName] string propertyName = "")
		{
			if (!EqualityComparer<T>.Default.Equals(current, newValue))
			{
				current = newValue;
				OnPropertyChanged(propertyName);

				return true;
			}

			return false;
		}

		//-------------------------------------------------------------------------
		protected bool SetValue<T>(ref T current, T newValue, string[] propertyNames)
		{
			if (!EqualityComparer<T>.Default.Equals(current, newValue))
			{
				current = newValue;
				foreach (string name in propertyNames)
					OnPropertyChanged(name);

				return true;
			}

			return false;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

	}
}
