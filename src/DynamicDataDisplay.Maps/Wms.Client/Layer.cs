// $File: //depot/WMS/WMS Overview/Wms.Client/Layer.cs $ $Revision: #1 $ $Change: 20 $ $DateTime: 2004/05/23 23:42:06 $

namespace Wms.Client
{
	/// <summary>
	/// Represents a layer within a WMS capabilities description.
	/// See Wms.Client.Capabilities and Wms.Client.WMSDialog for
	/// example usage.
	/// </summary>
	public class Layer
	{
		private Server server; // The WMS server object representing the server holding this layer.

		private System.Xml.XPath.XPathNavigator nav;        // Keep constant; never give this out. Give out only clones,
		public System.Xml.XPath.XPathNavigator Navigator    // <-- using this accessor.
		{
			get { return this.nav.Clone(); }
		}

		// The XPathNavigator is to the XML capabilities document of the WMS server.
		internal Layer(System.Xml.XPath.XPathNavigator nav, Server server)
		{
			this.nav = nav;
			this.server = server;
		}

		public Server Server
		{
			get { return this.server; }
		}

		// The following are utilities for Layers.

		public bool HasParentLayer
		{
			get
			{
				// Returns true if this layer is a child of another layer. Only the top-most layer of a
				// WMS server will return null from this accessor.
				System.Xml.XPath.XPathNodeIterator iter = this.nav.Select(Capabilities.ExpandPattern(@"../../Layer"));
				return iter.Count > 0;
			}
		}

		public Layer GetParentLayer()
		{
			Layer parent = null;
			if (this.HasParentLayer)
			{
				System.Xml.XPath.XPathNavigator parentNode = this.Navigator;
				if (parentNode.MoveToParent())
				{
					parent = new Layer(parentNode, this.server);
				}
			}
			return parent;
		}

		internal static Layer[] GetLayers(System.Xml.XPath.XPathNavigator parentNode, Server server)
		{
			// The incoming parentNode might not be a Layer node, but we assume it is here in
			// order to re-use the Layers property of the Layer class. If parentNode has
			// no Layer children, then the Layers property returns an empty array.
			Layer layerParent = new Layer(parentNode, server);
			return layerParent.Layers;
		}

		public class UriAndFormatType // This is an often-used data type in the WMS schema.
		{
			public string Uri;
			public string Format;

			internal UriAndFormatType()
			{
				this.Format = string.Empty;
				this.Uri = string.Empty;
			}

			public override bool Equals(object obj)
			{
				if (obj == null)
					return false;

				if (this == obj)
					return true;

				if (obj.GetType() != this.GetType())
					return false;

				UriAndFormatType t = (UriAndFormatType)obj;
				return this.Uri.Equals(t.Uri) && this.Format.Equals(t.Format);
			}

			public override int GetHashCode()
			{
				return (Uri, Format).GetHashCode();
			}

			public bool IsEmpty
			{
				get { return this.Uri.Equals(string.Empty); }
			}
		}

		// All the following use XPath search strings to find various bits of information in the
		// server's WMS capabilities description.

		internal static UriAndFormatType[] GetUriAndFormatInstances(System.Xml.XPath.XPathNavigator node, string pattern)
		{
			System.Xml.XPath.XPathNodeIterator iter = node.Select(Capabilities.ExpandPattern(pattern));
			UriAndFormatType[] retVal = new UriAndFormatType[iter.Count];
			while (iter.MoveNext())
			{
				int i = iter.CurrentPosition - 1;
				retVal[i] = new UriAndFormatType();
				retVal[i].Format = Capabilities.GetStringInstance(iter.Current, @"./Format");
				retVal[i].Uri = Capabilities.GetOnlineResourceInstance(iter.Current, @"./OnlineResource");
			}
			return retVal;
		}

		//
		// The following are non-inherited properties.
		//

		public string Name
		{
			get { return Capabilities.GetStringInstance(this.nav, @"./Name"); }
		}

		public string Title
		{
			get { return Capabilities.GetStringInstance(this.nav, @"./Title"); }
		}

		public string Abstract
		{
			get { return Capabilities.GetStringInstance(this.nav, @"./Abstract"); }
		}

		public string[] KeywordList
		{
			get { return Capabilities.GetStringsInstance(this.nav, @"./KeywordList/Keyword"); }
		}

		public class IdentifierType
		{
			public string Identifier;
			public string Authority;

			internal IdentifierType()
			{
				this.Identifier = string.Empty;
				this.Authority = string.Empty;
			}
		}

		public IdentifierType[] Identifiers
		{
			get
			{
				System.Xml.XPath.XPathNodeIterator iter = this.nav.Select(Capabilities.ExpandPattern(@"./Identifier"));
				IdentifierType[] retVal = new IdentifierType[iter.Count];
				while (iter.MoveNext())
				{
					int i = iter.CurrentPosition - 1;
					retVal[i] = new IdentifierType();
					retVal[i].Identifier = iter.Current.Value;
					retVal[i].Authority = Capabilities.GetStringInstance(iter.Current, @"./@Authority");
				}
				return retVal;
			}
		}

		public UriAndFormatType[] DataUris
		{
			get { return GetUriAndFormatInstances(this.nav, @"./DataUrl"); }
		}

		public UriAndFormatType[] FeatureListUris
		{
			get { return GetUriAndFormatInstances(this.nav, @"./FeatureListUrl"); }
		}

		public class MetadataUriType
		{
			public UriAndFormatType MetadataUri;
			public string Type;

			internal MetadataUriType()
			{
				this.Type = string.Empty;
				this.MetadataUri = new UriAndFormatType();
			}
		}

		public MetadataUriType[] MetadataUris
		{
			get
			{
				System.Xml.XPath.XPathNodeIterator iter =
					this.nav.Select(Capabilities.ExpandPattern(@"./MetadataUrl"));
				MetadataUriType[] retVal = new MetadataUriType[iter.Count];
				while (iter.MoveNext())
				{
					int i = iter.CurrentPosition - 1;
					retVal[i] = new MetadataUriType();
					retVal[i].Type = Capabilities.GetStringInstance(iter.Current, @"./@Type");
					UriAndFormatType[] t = GetUriAndFormatInstances(iter.Current, @".");
					if (t.Length > 0)
						retVal[i].MetadataUri = t[0];
				}
				return retVal;
			}
		}

		public Layer[] Layers
		{
			get
			{
				System.Xml.XPath.XPathNodeIterator iter = this.nav.Select(Capabilities.ExpandPattern(@"./Layer"));
				Layer[] retVal = new Layer[iter.Count];
				while (iter.MoveNext())
				{
					retVal[iter.CurrentPosition - 1] = new Layer(iter.Current.Clone(), this.server);
				}
				return retVal;
			}
		}

		//
		// The following are properties that have "Replace" inheritance.
		//

		public class BoundingBoxType
		{
			public string Srs;
			public double MinX;
			public double MinY;
			public double MaxX;
			public double MaxY;
			public double ResX;
			public double ResY;

			public override string ToString()
			{
				string retVal = this.MinY + " to " + this.MaxY + " Latitude, " + this.MinX + " to " + this.MaxX + " Longitude";
				if (this.Srs != null && !this.Srs.Equals(string.Empty))
				{
					retVal += ", SRS: " + this.Srs;
				}
				if (this.ResX != 0 || this.ResY != 0)
				{
					retVal += ", Resolution = (" + this.ResX.ToString() + ", " + this.ResY.ToString() + ")";
				}
				return retVal;
			}

		}

		public BoundingBoxType LatLonBoundingBox
		{
			get
			{
				System.Xml.XPath.XPathNodeIterator iter =
					this.nav.Select(Capabilities.ExpandPattern(@"./LatLonBoundingBox"));
				BoundingBoxType retVal = new BoundingBoxType();
				if (iter.MoveNext())
				{
					retVal.Srs = "EPSG:4326";
					retVal.MinX = double.Parse(Capabilities.GetStringInstance(iter.Current, @"./@MinX|./@minX"));
					retVal.MinY = double.Parse(Capabilities.GetStringInstance(iter.Current, @"./@MinY|./@minY"));
					retVal.MaxX = double.Parse(Capabilities.GetStringInstance(iter.Current, @"./@MaxX|./@maxX"));
					retVal.MaxY = double.Parse(Capabilities.GetStringInstance(iter.Current, @"./@MaxY|./@maxY"));
				}
				else
				{
					// See whether one is defined higher up the Layer hierarchy.
					Layer parent = this.GetParentLayer();
					if (parent != null)
					{
						retVal = parent.LatLonBoundingBox;
					}
				}
				return retVal;
			}
		}

		public BoundingBoxType[] BoundingBoxes
		{
			get
			{
				System.Xml.XPath.XPathNodeIterator iter = this.nav.Select(Capabilities.ExpandPattern(@"./BoundingBox"));
				BoundingBoxType[] retVal = new BoundingBoxType[iter.Count];
				if (iter.MoveNext())
				{
					do
					{
						int i = iter.CurrentPosition - 1;
						retVal[i] = new BoundingBoxType();
						retVal[i].Srs = Capabilities.GetStringInstance(iter.Current, @"./@Srs");
						retVal[i].MinX = double.Parse(Capabilities.GetStringInstance(iter.Current, @"./@MinX|./@minX"));
						retVal[i].MinY = double.Parse(Capabilities.GetStringInstance(iter.Current, @"./@MinY|./@minY"));
						retVal[i].MaxX = double.Parse(Capabilities.GetStringInstance(iter.Current, @"./@MaxX|./@maxX"));
						retVal[i].MaxY = double.Parse(Capabilities.GetStringInstance(iter.Current, @"./@MaxY|./@maxY"));
						string resX = Capabilities.GetStringInstance(iter.Current, @"./@ResX|./@resX");
						if (resX.Length > 0)
						{
							retVal[i].ResX = double.Parse(resX);
						}
						string resY = Capabilities.GetStringInstance(iter.Current, @"./@ResY|./@resY");
						if (resY.Length > 0)
						{
							retVal[i].ResY = double.Parse(resY);
						}
					} while (iter.MoveNext());
				}
				else
				{
					// See whether some are defined higher up the Layer hierarchy.
					Layer parent = this.GetParentLayer();
					if (parent != null)
					{
						retVal = parent.BoundingBoxes;
					}
				}
				return retVal;
			}
		}

		public class ExtentType
		{
			public string Name;
			public string Default;
			public string Extent;

			public ExtentType()
			{
				this.Name = string.Empty;
				this.Default = string.Empty;
				this.Extent = string.Empty;
			}

			public bool IsEmpty
			{
				get
				{
					return this.Name.Equals(string.Empty) && this.Default.Equals(string.Empty) && this.Extent.Equals(string.Empty);
				}
			}

			public override string ToString()
			{
				string retVal = "";
				if (!this.Name.Equals(string.Empty))
				{
					retVal += this.Name;
				}
				if (!this.Extent.Equals(string.Empty))
				{
					retVal += ", " + this.Extent;
				}
				if (!this.Default.Equals(string.Empty))
				{
					retVal += ", Default = " + this.Default;
				}
				return retVal;
			}
		}

		public ExtentType[] Extents
		{
			get
			{
				System.Xml.XPath.XPathNodeIterator iter = this.nav.Select(Capabilities.ExpandPattern(@"./Extent"));
				ExtentType[] retVal = new ExtentType[iter.Count];
				if (iter.MoveNext())
				{
					do
					{
						int i = iter.CurrentPosition - 1;
						retVal[i] = new ExtentType();
						retVal[i].Extent = iter.Current.Value;
						retVal[i].Name = Capabilities.GetStringInstance(iter.Current, @"./@Name");
						retVal[i].Default = Capabilities.GetStringInstance(iter.Current, @"./@Default");
					} while (iter.MoveNext());
				}
				else
				{
					// See whether some are defined higher up the Layer hierarchy.
					Layer parent = this.GetParentLayer();
					if (parent != null)
					{
						retVal = parent.Extents;
					}
				}
				return retVal;
			}
		}

		public class ScaleHintType
		{
			// The WMS 1.1.1 spec doesn't demand that these values be numbers. Therefore 
			// treat them as strings. The application can easily parse them as numbers if
			// appropriate. (Using double.Parse())
			public string Min;
			public string Max;

			public ScaleHintType()
			{
				this.Min = string.Empty;
				this.Max = string.Empty;
			}

			public bool IsEmpty
			{
				get
				{
					return this.Min.Equals(string.Empty) && this.Max.Equals(string.Empty);
				}
			}
		}

		public ScaleHintType ScaleHint
		{
			get
			{
				System.Xml.XPath.XPathNodeIterator iter = this.nav.Select(Capabilities.ExpandPattern(@"./ScaleHint"));
				ScaleHintType retVal = new ScaleHintType();
				if (iter.MoveNext())
				{
					retVal.Min = Capabilities.GetStringInstance(iter.Current, @"./@Min");
					retVal.Max = Capabilities.GetStringInstance(iter.Current, @"./@Max");
				}
				else
				{
					// See whether one is defined higher up the Layer hierarchy.
					Layer parent = this.GetParentLayer();
					if (parent != null)
					{
						retVal = parent.ScaleHint;
					}
				}
				return retVal;
			}
		}

		public class LogoOrLegendUriType
		{
			public UriAndFormatType Uri;
			public double Width;
			public double Height;

			internal LogoOrLegendUriType()
			{
				this.Uri = new UriAndFormatType();
			}

			public bool IsEmpty
			{
				get { return this.Uri.IsEmpty; }
			}
		}

		private static LogoOrLegendUriType GetLogoOrLegendUriInstance(System.Xml.XPath.XPathNavigator node, string pattern)
		{
			System.Xml.XPath.XPathNodeIterator iter = node.Select(Capabilities.ExpandPattern(pattern));
			LogoOrLegendUriType retVal = new LogoOrLegendUriType();
			if (iter.MoveNext())
			{
				UriAndFormatType[] t = GetUriAndFormatInstances(iter.Current, @".");
				if (t.Length > 0)
					retVal.Uri = t[0];
				retVal.Width = double.Parse(Capabilities.GetStringInstance(iter.Current, @"./@Width"));
				retVal.Height = double.Parse(Capabilities.GetStringInstance(iter.Current, @"./@Height"));
			}
			return retVal;
		}

		public class AttributionType
		{
			public string Title;
			public string Uri;
			public LogoOrLegendUriType LogoUri;

			internal AttributionType()
			{
				this.Title = string.Empty;
				this.Uri = string.Empty;
				this.LogoUri = new LogoOrLegendUriType();
			}

			public bool IsEmpty
			{
				get
				{
					return this.Title.Equals(string.Empty)
						&& this.Uri.Equals(string.Empty)
						&& this.LogoUri.IsEmpty;
				}
			}
		}

		public AttributionType Attribution
		{
			get
			{
				System.Xml.XPath.XPathNodeIterator iter = this.nav.Select(Capabilities.ExpandPattern(@"./Attribution"));
				AttributionType retVal = new AttributionType();
				if (iter.MoveNext())
				{
					retVal.Title = Capabilities.GetStringInstance(iter.Current, @"./Title");
					retVal.Uri = Capabilities.GetOnlineResourceInstance(iter.Current, @"./OnlineResource");
					retVal.LogoUri = Layer.GetLogoOrLegendUriInstance(iter.Current, @"./LogoUrl");
				}
				else
				{
					// See whether one is defined higher up the Layer hierarchy.
					Layer parent = this.GetParentLayer();
					if (parent != null)
					{
						retVal = parent.Attribution;
					}
				}
				return retVal;
			}
		}

		public bool Queryable
		{
			get
			{
				bool retVal = false;
				string q = Capabilities.GetStringInstance(this.nav, @"./@Queryable").Trim();
				if (!q.Equals(string.Empty))
				{
					retVal = q.Equals("1");
				}
				else
				{
					// See whether one is defined higher up the Layer hierarchy.
					Layer parent = this.GetParentLayer();
					if (parent != null)
					{
						retVal = parent.Queryable;
					}
				}
				return retVal;
			}
		}

		public bool Opaque
		{
			get
			{
				bool retVal = false;
				string q = Capabilities.GetStringInstance(this.nav, @"./@Opaque").Trim();
				if (!q.Equals(string.Empty))
				{
					retVal = q.Equals("1");
				}
				else
				{
					// See whether one is defined higher up the Layer hierarchy.
					Layer parent = this.GetParentLayer();
					if (parent != null)
					{
						retVal = parent.Opaque;
					}
				}
				return retVal;
			}
		}

		public bool NoSubsets
		{
			get
			{
				bool retVal = false;
				string q = Capabilities.GetStringInstance(this.nav, @"./@NoSubsets|./@noSubsets").Trim();
				if (!q.Equals(string.Empty))
				{
					retVal = q.Equals("1");
				}
				else
				{
					// See whether one is defined higher up the Layer hierarchy.
					Layer parent = this.GetParentLayer();
					if (parent != null)
					{
						retVal = parent.NoSubsets;
					}
				}
				return retVal;
			}
		}

		public int Cascaded
		{
			get
			{
				int retVal = 0;
				string q = Capabilities.GetStringInstance(this.nav, @"./@Cascaded");
				if (!q.Equals(string.Empty))
				{
					retVal = int.Parse(q);
				}
				else
				{
					// See whether one is defined higher up the Layer hierarchy.
					Layer parent = this.GetParentLayer();
					if (parent != null)
					{
						retVal = parent.Cascaded;
					}
				}
				return retVal;
			}
		}

		public int FixedWidth
		{
			get
			{
				int retVal = 0;
				string q = Capabilities.GetStringInstance(this.nav, @"./@FixedWidth|./@fixedWidth");
				if (!q.Equals(string.Empty))
				{
					retVal = int.Parse(q);
				}
				else
				{
					// See whether one is defined higher up the Layer hierarchy.
					Layer parent = this.GetParentLayer();
					if (parent != null)
					{
						retVal = parent.FixedWidth;
					}
				}
				return retVal;
			}
		}

		public int FixedHeight
		{
			get
			{
				int retVal = 0;
				string q = Capabilities.GetStringInstance(this.nav, @"./@FixedHeight|./@fixedHeight");
				if (!q.Equals(string.Empty))
				{
					retVal = int.Parse(q);
				}
				else
				{
					// See whether one is defined higher up the Layer hierarchy.
					Layer parent = this.GetParentLayer();
					if (parent != null)
					{
						retVal = parent.FixedHeight;
					}
				}
				return retVal;
			}
		}

		//
		// The following properties have "Add" inheritance.
		//

		public class DimensionType
		{
			public string Name;
			public string Units;
			public string UnitSymbol;

			public DimensionType()
			{
				this.Name = string.Empty;
				this.Units = string.Empty;
				this.UnitSymbol = string.Empty;
			}

			public override bool Equals(object obj)
			{
				if (obj == null)
					return false;

				if (this == obj)
					return true;

				if (obj.GetType() != this.GetType())
					return false;

				DimensionType d = (DimensionType)obj;
				return this.Name.Equals(d.Name)
					&& this.Units.Equals(d.Units)
					&& this.UnitSymbol.Equals(d.UnitSymbol);
			}

			public override int GetHashCode()
			{
				return (Name, Units, UnitSymbol).GetHashCode();
			}

			public override string ToString()
			{
				string retVal = this.Name + ", " + this.Units;
				if (!this.UnitSymbol.Equals(string.Empty))
				{
					retVal += " (" + this.UnitSymbol + ")";
				}
				return retVal;
			}

		}

		public DimensionType[] Dimensions
		{
			get
			{
				// Collect any values from ancestors.
				System.Collections.ArrayList retVal = new System.Collections.ArrayList();
				if (this.HasParentLayer)
				{
					DimensionType[] parentValues = this.GetParentLayer().Dimensions;
					if (parentValues.Length > 0)
					{
						retVal.AddRange(parentValues);
					}
				}

				// Add those uniquely defined at this Layer.
				System.Xml.XPath.XPathNodeIterator iter = this.nav.Select(Capabilities.ExpandPattern(@"./Dimension"));
				DimensionType dim = new DimensionType();
				while (iter.MoveNext())
				{
					dim = new DimensionType();
					dim.Name = Capabilities.GetStringInstance(iter.Current, @"./@Name");
					dim.Units = Capabilities.GetStringInstance(iter.Current, @"./@Units");
					dim.UnitSymbol = Capabilities.GetStringInstance(iter.Current, @"./@UnitSymbol|./@unitSymbol");
					if (!retVal.Contains(dim))
					{
						retVal.Add(dim);
					}
				}
				return retVal.ToArray(dim.GetType()) as DimensionType[];
			}
		}

		public string[] Srs
		{
			get
			{
				// Collect any values from ancestors.
				System.Collections.ArrayList retVal = new System.Collections.ArrayList();
				if (this.HasParentLayer)
				{
					string[] parentValues = this.GetParentLayer().Srs;
					if (parentValues.Length > 0)
					{
						retVal.AddRange(parentValues);
					}
				}

				// Add those uniquely defined at this Layer.
				string[] local = Capabilities.GetStringsInstance(this.nav, @"./Srs");
				foreach (string s in local)
				{
					if (!retVal.Contains(s))
					{
						retVal.Add(s);
					}
				}
				return retVal.ToArray(string.Empty.GetType()) as string[];
			}
		}

		public class AuthorityUriType
		{
			public string Name;
			public string Uri;

			internal AuthorityUriType()
			{
				this.Name = string.Empty;
				this.Uri = string.Empty;
			}

			public override bool Equals(object obj)
			{
				if (obj == null)
					return false;

				if (this == obj)
					return true;

				if (obj.GetType() != this.GetType())
					return false;

				return this.Name.Equals(((AuthorityUriType)obj).Name)
					&& this.Uri.Equals(((AuthorityUriType)obj).Uri);
			}

			public override int GetHashCode()
			{
				return (Name, Uri).GetHashCode();
			}
		}

		public AuthorityUriType[] AuthorityUris
		{
			get
			{
				// Collect any values from ancestors.
				System.Collections.ArrayList retVal = new System.Collections.ArrayList();
				if (this.HasParentLayer)
				{
					AuthorityUriType[] parentValues = this.GetParentLayer().AuthorityUris;
					if (parentValues.Length > 0)
					{
						retVal.AddRange(parentValues);
					}
				}

				// Add those uniquely defined at this Layer.
				System.Xml.XPath.XPathNodeIterator iter =
					this.nav.Select(Capabilities.ExpandPattern(@"./AuthorityUrl"));
				AuthorityUriType auth = new AuthorityUriType();
				while (iter.MoveNext())
				{
					auth = new AuthorityUriType();
					auth.Name = Capabilities.GetStringInstance(iter.Current, @"./@Name");
					auth.Uri = Capabilities.GetOnlineResourceInstance(iter.Current, @"./OnlineResource");
					if (!retVal.Contains(auth))
					{
						retVal.Add(auth);
					}
				}
				return retVal.ToArray(auth.GetType()) as AuthorityUriType[];
			}
		}

		public class StyleType
		{
			public string Name;
			public string Title;
			public string Abstract;
			public UriAndFormatType StyleUri;
			public UriAndFormatType StyleSheetUri;
			public LogoOrLegendUriType LegendUri;

			internal StyleType()
			{
				this.Name = string.Empty;
				this.Title = string.Empty;
				this.Abstract = string.Empty;
				this.StyleUri = new UriAndFormatType();
				this.StyleSheetUri = new UriAndFormatType();
				this.LegendUri = new LogoOrLegendUriType();
			}

			public override bool Equals(object obj)
			{
				if (obj == null)
					return false;

				if (this == obj)
					return true;

				if (obj.GetType() != this.GetType())
					return false;

				StyleType s = (StyleType)obj;
				return this.Name.Equals(s.Name)
					&& this.Title.Equals(s.Title)
					&& this.Abstract.Equals(s.Abstract)
					&& this.StyleUri.Equals(s.StyleUri)
					&& this.StyleSheetUri.Equals(s.StyleSheetUri)
					&& this.LegendUri.Equals(s.LegendUri);
			}

			public override int GetHashCode()
			{
				return (Name, Title, Abstract, StyleUri, StyleSheetUri, LegendUri).GetHashCode();
			}
		}

		public StyleType[] Styles
		{
			get
			{
				// Collect any values from ancestors.
				System.Collections.ArrayList retVal = new System.Collections.ArrayList();
				if (this.HasParentLayer)
				{
					StyleType[] parentValues = this.GetParentLayer().Styles;
					if (parentValues.Length > 0)
					{
						retVal.AddRange(parentValues);
					}
				}

				// Add those uniquely defined at this Layer.
				System.Xml.XPath.XPathNodeIterator iter = this.nav.Select(Capabilities.ExpandPattern(@"./Style"));
				StyleType style = new StyleType();
				while (iter.MoveNext())
				{
					style = new StyleType();
					style.Name = Capabilities.GetStringInstance(iter.Current, @"./Name");
					style.Title = Capabilities.GetStringInstance(iter.Current, @"./Title");
					style.Abstract = Capabilities.GetStringInstance(iter.Current, @"./Abstract");
					UriAndFormatType[] su = GetUriAndFormatInstances(iter.Current, @"./StyleUrl");
					if (su.Length > 0)
						style.StyleUri = su[0];
					UriAndFormatType[] ssu = GetUriAndFormatInstances(iter.Current, @"./StyleSheetUrl");
					if (ssu.Length > 0)
						style.StyleSheetUri = ssu[0];
					style.LegendUri = GetLogoOrLegendUriInstance(iter.Current, @"./LegendUrl");
					if (!retVal.Contains(style))
					{
						retVal.Add(style);
					}
				}
				return retVal.ToArray(style.GetType()) as StyleType[];
			}
		}
	}
}