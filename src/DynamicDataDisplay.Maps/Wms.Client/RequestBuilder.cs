// $File: //depot/WMS/WMS Overview/Wms.Client/RequestBuilder.cs $ $Revision: #1 $ $Change: 20 $ $DateTime: 2004/05/23 23:42:06 $

using System;

namespace Wms.Client
{
	/// <summary>
	/// Provides an interface for conveniently building WMS request strings.
	/// All the WMS query parameters are held as strings in an array. When
	/// the parameters are needed to form a URI, the code assembles them as
	/// attribute-value pairs separated by ampersands, all within one string
	/// whose leading portion is the server's GetMap URI.
	/// 
	/// Two concrete classes exist, one for GetCapabilities requests and
	/// one for GetMap requests.
	/// </summary>
	public abstract class RequestBuilder
	{
		private System.UriBuilder uri;
		private System.Collections.Hashtable clientInfo;
		protected System.Collections.Hashtable queryParameters;

		public RequestBuilder()
		{
			this.initializeParams();
			this.clientInfo = new System.Collections.Hashtable();
		}

		public RequestBuilder(System.Uri uri)
		{
			this.uri = new System.UriBuilder(uri.Scheme, uri.Host, uri.Port, uri.AbsolutePath);
			this.initializeParams();
			this.clientInfo = new System.Collections.Hashtable();
		}

		private void initializeParams()
		{
			this.queryParameters = new System.Collections.Hashtable();
			this.queryParameters["version"] = "1.1.1";
			this.queryParameters["service"] = "WMS";
			//this.queryParameters["EXCEPTIONS"] = "application/vnd.ogc.se_xml";
		}

		public System.Collections.Hashtable ClientInfo
		{
			get { return this.clientInfo; }
		}

		public void SetParam(string name, string param)
		{
			if (name != null && !name.Equals(string.Empty, StringComparison.Ordinal))
			{
				this.queryParameters[name] = param;
			}
		}

		public string GetParam(string name)
		{
			if (name == null)
				return null;
			else
				return this.queryParameters[name] as string;
		}

		public string Service
		{
			set { this.SetParam("SERVICE", value); }
			get { return this.GetParam("SERVICE"); }
		}

		public string Version
		{
			set { this.SetParam("VERSION", value); }
			get { return this.GetParam("VERSION"); }
		}

		public string Exceptions
		{
			set { this.SetParam("EXCEPTIONS", value); }
			get { return this.GetParam("EXCEPTIONS"); }
		}

		public string Request
		{
			set { this.SetParam("REQUEST", value); }
			get { return this.GetParam("REQUEST"); }
		}

		public System.Uri Uri
		{
			set
			{
				this.uri = new System.UriBuilder(value.Scheme, value.Host, value.Port, value.AbsolutePath);

			}
			get { return new System.Uri(this.ToString()); }
		}

		private string getQueryString()
		{
			// Assemble a query string from all the individual parameters.
			System.Text.StringBuilder queryString = new System.Text.StringBuilder();
			System.Collections.IDictionaryEnumerator iter = this.queryParameters.GetEnumerator();

			while (iter.MoveNext())
			{
				queryString.Append(iter.Key as string);
				queryString.Append("=");
				queryString.Append(iter.Value as string);
				queryString.Append("&");
			}

			return queryString.ToString();
		}

		public override string ToString()
		{
			this.uri.Query = this.getQueryString();
			return this.uri.Uri.ToString();
		}
	}

	public class CapabilitiesRequestBuilder : Wms.Client.RequestBuilder
	{
		public CapabilitiesRequestBuilder()
		{
			this.initializeParams();
		}

		public CapabilitiesRequestBuilder(System.Uri uri) : base(uri)
		{
			this.initializeParams();
		}

		private void initializeParams()
		{
			this.queryParameters["request"] = "GetCapabilities";
		}
	}

	public class MapRequestBuilder : Wms.Client.RequestBuilder
	{
		public MapRequestBuilder()
		{
			this.initializeParams();
		}

		public MapRequestBuilder(System.Uri getMapUri) : base(getMapUri)
		{
			this.initializeParams();
		}

		private void initializeParams()
		{
			this.queryParameters["REQUEST"] = "GetMap";
			this.queryParameters["SRS"] = "EPSG:4326";
			this.queryParameters["BBOX"] = "-180.0,-90.0,180.0,90.0";
			this.queryParameters["WIDTH"] = "300";
			this.queryParameters["HEIGHT"] = "150";
			this.queryParameters["TRANSPARENT"] = "TRUE";
			this.queryParameters["FORMAT"] = "image/gif";
		}

		public string Layers
		{
			set { this.SetParam("LAYERS", value); }
			get { return this.GetParam("LAYERS"); }
		}

		public string Styles
		{
			set { this.SetParam("STYLES", value); }
			get { return this.GetParam("STYLES"); }
		}

		public string Srs
		{
			set { this.SetParam("SRS", value); }
			get { return this.GetParam("SRS"); }
		}

		public string BoundingBox
		{
			set { this.SetParam("BBOX", value); }
			get { return this.GetParam("BBOX"); }
		}

		public int Width
		{
			set { this.SetParam("WIDTH", value.ToString()); }
			get { return int.Parse(this.GetParam("WIDTH")); }
		}

		public int Height
		{
			set { this.SetParam("HEIGHT", value.ToString()); }
			get { return int.Parse(this.GetParam("HEIGHT")); }
		}

		public string Format
		{
			set { this.SetParam("FORMAT", value); }
			get { return this.GetParam("FORMAT"); }
		}

		public bool Transparent
		{
			set { this.SetParam("TRANSPARENT", value ? "TRUE" : "FALSE"); }
			get { return this.GetParam("TRANSPARENT").Equals("TRUE", StringComparison.Ordinal) ? true : false; }
		}

		public string BackgroundColor
		{
			set { this.SetParam("BGCOLOR", value); }
			get { return this.GetParam("BGCOLOR"); }
		}

		public string Time
		{
			set { this.SetParam("TIME", value); }
			get { return this.GetParam("TIME"); }
		}

		public string Elevation
		{
			set { this.SetParam("ELEVATION", value); }
			get { return this.GetParam("ELEVATION"); }
		}

		public string Sld
		{
			set { this.SetParam("SLD", value); }
			get { return this.GetParam("SLD"); }
		}

		public string Wfs
		{
			set { this.SetParam("WFS", value); }
			get { return this.GetParam("WFS"); }
		}

		// This function adds the default extents to the GetMap URI as explicit
		// parameters, rather than implicit ones. This is necessary, for example,
		// to identify URIs for cached maps. If the URI did not contain explicit
		// extents, then each URI for the same layer would appear the same even
		// if the default changed on the WMS server. Using explicit default
		// extents eliminates that condition.
		public void IncludeDefaultExtents(Layer layer)
		{
			foreach (Layer.ExtentType extent in layer.Extents)
			{
				if (extent.Default != null && !extent.Default.Equals(string.Empty, StringComparison.Ordinal))
				{
					if (extent.Name.Equals("elevation", StringComparison.Ordinal) || extent.Name.Equals("Elevation", StringComparison.Ordinal) || extent.Name.Equals("ELEVATION", StringComparison.Ordinal))
					{
						this.Elevation = extent.Default;
					}
					else if (extent.Name.Equals("time", StringComparison.Ordinal) || extent.Name.Equals("Time", StringComparison.Ordinal) || extent.Name.Equals("TIME", StringComparison.Ordinal))
					{
						this.Time = extent.Default;
					}
					else
					{
						this.SetParam(extent.Name, extent.Default);
					}
				}
			}
		}
	}
}
