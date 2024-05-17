using DynamicDataDisplay.Charts.Maps;
using DynamicDataDisplay.Common.Auxiliary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Wms.Client;

namespace DynamicDataDisplay.Maps.Servers.Network
{
    public class WMSTileServer : NetworkTileServer
	{
		private static string mCachePath = null;
		//-------------------------------------------------------------------------
		// Set in static constructor to be %TEMP%/<AssemblyID>/
		public static string CachePath
		{
			get { return mCachePath; }
			set
			{
				if (mCachePath != value)
				{
					try
					{
						var hasPath = !string.IsNullOrEmpty(value);

                        if (hasPath && !Directory.Exists(value))
						{
							Directory.CreateDirectory(value);
						}

						mCachePath = value;
					}
					catch (Exception)
					{
						Console.Out.WriteLine($"Unable to use '{value}' as WMSTileSeerver.CachePath, reverting to previous path '{mCachePath}'");
					}
				}
			}
		}

		//-------------------------------------------------------------------------
		// All supported SRS
		private static EPSG_Converter[] SRSConverters = new EPSG_Converter[] {
	  new EPSG_3857(),    // aka EPSG:900913  / (google, bing) web mercator
      new EPSG_900913(),  // same as EPSG:3857
      new EPSG_4326()     // aka WGS-84 / Lat-Lon
    };

		//-------------------------------------------------------------------------
		public EPSG_Converter SRSConverter { get; protected set; }

		//-------------------------------------------------------------------------
		// The actual Web Map Service that gets data
		public Wms.Client.Server Service
		{
			get; private set;
		}

		//-------------------------------------------------------------------------
		// Selected map layer to display, defaults to first layer on service
		private Wms.Client.Layer mLayer = null;
		public Wms.Client.Layer Layer
		{
			get { return mLayer; }
			private set
			{
				if (mLayer != value)
				{
					mLayer = value;
				}
			}
		}

		//-------------------------------------------------------------------------
		public override string ServerName
		{
			get
			{
				int ServiceHash = Service.Uri.AbsolutePath.GetDeterministicHashCode();
				return string.Format("WMS-{0}-{1}", ServiceHash, Layer?.Name ?? String.Empty);
			}
		}

		public double MaxConversionLatitude
		{
			get
			{
				return SRSConverter?.MaxLatitude ?? EPSG_Converter.MaxLatitudeDefault;
			}
			set
			{
				if (SRSConverter != null)
				{
					SRSConverter.MaxLatitude = value;
				}
			}
		}

		//-------------------------------------------------------------------------
		// Static constructor to set the CachePath location where server capability
		// xml files and tile images are saved
		static WMSTileServer()
		{
			CachePath = FileSystemTileServer.AppendAssemblyId(Path.GetTempPath());
		}

		//-------------------------------------------------------------------------
		// Defaults to Open Street Maps
		public WMSTileServer()
		  //      : this(@"http://neowms.sci.gsfc.nasa.gov/wms/wms", "BlueMarbleNG", 4)
		  : this(@"http://ows.terrestris.de/osm/service", "OSM-WMS", 15)
		{
		}

		//-------------------------------------------------------------------------
		public WMSTileServer(string url, string layerName, int maxLevel = 4)
		  : this(Server.CreateService(url, CachePath), layerName, maxLevel)
		{
		}

		//-------------------------------------------------------------------------
		// Prefered constructor, this allows the slow network download to create the 
		// WMS.Client.Server to be done before creating the Tile Server (which must 
		// be done on the UI thread)
		public WMSTileServer(Wms.Client.Server wmsService, string layerName, int maxLevel = 4)
		{
			Service = wmsService;

			MinLevel = 1;
			MaxLevel = maxLevel;
			MaxConcurrentDownloads = 4;


			// Try to use the specific layer
			Layer = Service.FindLayerByName(layerName);
			if (Layer == null || string.IsNullOrWhiteSpace(layerName))
			{
				MapsTraceSource.Instance.ServerInformationTraceSource.TraceInformation("WMSServer {0} has no layer named {1}, defaulting to first valid layer.", Service.Uri, layerName);

				// Find and use the first valid (named) layer if the user specified layer doesn't exist
				Layer = Service.FindLayer((l) => !string.IsNullOrWhiteSpace(l.Name));
				if (Layer == null)
				{
					MapsTraceSource.Instance.ServerInformationTraceSource.TraceInformation("WMSServer {0} has no map layers - can't display anything.", Service.Uri);
				}
			}

			SRSConverter = null;
			// Set a suitable co-ordinate system
			foreach (var conv in SRSConverters)
			{
				foreach (var l in Layer.Srs)
				{
					if (conv.EPSGCode == l)
					{
						SRSConverter = conv;
						break;
					}
				}
				if (SRSConverter != null)
				{
					break;
				}
			}


			if (SRSConverter == null)
			{
				MapsTraceSource.Instance.ServerInformationTraceSource.TraceInformation("WMSServer {0} does not support any of the same SRS (co-ordinate) systems as us.  Can't display anything.", Service.Uri);
				Layer = null;
			}
		}



		//-------------------------------------------------------------------------
		public IEnumerable<string> AllLayerNames()
		{
			List<string> names = new List<string>();

			Service.FindLayer((l) =>
			{
				if (!string.IsNullOrWhiteSpace(l.Name))
				{
					names.Add(l.Name);
				}
				return false;
			});

			return names;
		}

		//-------------------------------------------------------------------------
		protected override string CreateRequestUriCore(TileIndex index)
		{
			if (Service == null || Layer == null) return null;

			MapRequestBuilder mapRequest = new MapRequestBuilder(new Uri(Service.Capabilities.GetMapRequestUri));
			mapRequest.Layers = Layer.Name;
			mapRequest.Styles = ""; // use default style for each layer
			mapRequest.Format = "image/png";

			mapRequest.Srs = SRSConverter.EPSGCode;
			mapRequest.BoundingBox = SRSConverter.TileIndexToBBox(index);

			mapRequest.Height = 256;
			mapRequest.Width = 256;
			mapRequest.Transparent = false;

			return mapRequest.Uri.ToString();
		}


		//-------------------------------------------------------------------------
		public abstract class EPSG_Converter
		{
			//-----------------------------------------------------------------------
			public virtual string EPSGCode { get; protected set; }

			//-----------------------------------------------------------------------
			public abstract string TileIndexToBBox(TileIndex tile);


			//-----------------------------------------------------------------------
			public abstract string BBoxToString(double lat1, double lon1, double lat2, double lon2);

			//-----------------------------------------------------------------------
			public DataTransform DataTransform { get; protected set; }

			//-----------------------------------------------------------------------
			public EPSG_Converter(string epsgCode)
			{
				EPSGCode = epsgCode;
				DataTransform = null;
			}

			//-----------------------------------------------------------------------
			public EPSG_Converter(string epsgCode, DataTransform dataTransform)
			{
				EPSGCode = epsgCode;
				DataTransform = dataTransform;
			}

			//-----------------------------------------------------------------------
			// Converts a TileIndex to a Lon/Lat point (*note order*) and optionally 
			// applies a transform to the lattitude.
			public Point TileToWorldPos(TileIndex tile)
			{
				double z = Math.Pow(2.0, tile.Level);

				Point p = new Point();
				p.X = (float)((360 * tile.X) / z);

				// Note: The D3.Maps TileIndex uses 85.0 not 85.0511... 
				p.Y = (tile.Y * 2 * MaxLatitude) / z;

				if (DataTransform != null)
				{
					p = DataTransform.ViewportToData(p);
				}

				return p;
			}

			public double MaxLatitude { get; set; } = MaxLatitudeDefault;

			public static double MaxLatitudeDefault { get; } = 85.0511287798;
		}

		//-------------------------------------------------------------------------
		// EPSG:4326
		// aka WGS84
		public class EPSG_4326 : EPSG_Converter
		{
			//-----------------------------------------------------------------------
			private static AspectRatioTransform _aspectTransform = new AspectRatioTransform();

			//-----------------------------------------------------------------------
			public EPSG_4326()
			  : base("EPSG:4326", _aspectTransform)
			{

			}

			//-----------------------------------------------------------------------
			public override string TileIndexToBBox(TileIndex index)
			{
				Point a = TileToWorldPos(index);
				Point b = TileToWorldPos(new TileIndex(index.X + 1, index.Y + 1, index.Level));
				return BBoxToString(a.Y, a.X, b.Y, b.X);
			}

			public override string BBoxToString(double lat1, double lon1, double lat2, double lon2)
			{
				return string.Format("{0},{1},{2},{3}", Math.Min(lon1, lon2), Math.Min(lat1, lat2), Math.Max(lon1, lon2), Math.Max(lat1, lat2));
			}

		}

		//-------------------------------------------------------------------------
		// EPSG:3857 / EPSG:900913 / (Google, Bing, OSM) Web Mercator
		public class EPSG_3857 : EPSG_Converter
		{
			//-----------------------------------------------------------------------
			public static double RADIUS = 6378137.0;
			private static double DEG_TO_RAD = Math.PI / 180.0;
			private static double RAD_TO_DEG = 180.0 / Math.PI;

			//-----------------------------------------------------------------------
			private static MercatorTransform _mercatorTransform = new MercatorTransform();

			//-----------------------------------------------------------------------
			public EPSG_3857()
			  : base("EPSG:3857", _mercatorTransform)
			{

			}

			//-----------------------------------------------------------------------
			public static double y2lat(double aY)
			{
				return RAD_TO_DEG * (2 * Math.Atan(Math.Exp(DEG_TO_RAD * (aY / RADIUS))) - Math.PI / 2);
			}
			//-----------------------------------------------------------------------
			public static double lat2y(double aLat)
			{
				return Math.Log(Math.Tan(Math.PI / 4 + (DEG_TO_RAD * (aLat)) / 2)) * RADIUS;
			}

			//-----------------------------------------------------------------------
			public static double x2lon(double aX)
			{
				return RAD_TO_DEG * (aX / RADIUS);
			}
			//-----------------------------------------------------------------------
			public static double lon2x(double aLong)
			{
				return DEG_TO_RAD * aLong * RADIUS;
			}

			//-----------------------------------------------------------------------
			public override string TileIndexToBBox(TileIndex tile)
			{
				Point a = TileToWorldPos(tile);
				Point b = TileToWorldPos(new TileIndex(tile.X + 1, tile.Y + 1, tile.Level));
				return BBoxToString(a.Y, a.X, b.Y, b.X);
			}

			//-----------------------------------------------------------------------
			public override string BBoxToString(double lat1, double lon1, double lat2, double lon2)
			{
				Point a = new Point(lon2x(lon1), lat2y(lat1));
				Point b = new Point(lon2x(lon2), lat2y(lat2));

				return string.Format("{0},{1},{2},{3}", Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
			}
		}

		//-------------------------------------------------------------------------
		// Straight copy of EPSG:3857
		public class EPSG_900913 : EPSG_3857
		{
			public EPSG_900913()
			{
				EPSGCode = "EPSG:900913";
			}
		}


		//-------------------------------------------------------------------------
		public override bool Equals(object obj)
		{
			var other = obj as WMSTileServer;
			if (other != null)
			{
				return other.ServerName.Equals(ServerName, StringComparison.Ordinal)
				  && other.Service.Uri.Equals(Service.Uri);
			}

			return base.Equals(obj);
		}

		//-------------------------------------------------------------------------
		public override int GetHashCode()
		{
			return ServerName.GetDeterministicHashCode()
			  + 3 * Service.Uri.ToString().GetDeterministicHashCode();
		}
	}
}
