// $File: //depot/WMS/WMS Overview/Wms.Client/Capabilities.cs $ $Revision: #1 $ $Change: 20 $ $DateTime: 2004/05/23 23:42:06 $

namespace Wms.Client
{
	///
	/// Class for determining WMS server capabilities. Contains accessors to
	/// fields in the capabilities description returned as an XML document
	/// from a WMS server.
	///
	public class Capabilities
	{
		private System.Xml.XPath.XPathDocument doc; // The root of the XML document.
		private Server server; // The Wms.Client.Server object associated with these capabilities.

		public Capabilities(string filePath, Server server)
		{
			// Constructor requires a path to an XML capabilities file,
			// and a reference to a Wms.Client.Server object.
			this.server = server;

			// Disable the XmlResolver since it fails if proxy authentication is required
			// when retreiving schemas listed in the Xml file
			var xmlReader = new System.Xml.XmlTextReader(filePath);
			xmlReader.XmlResolver = null;
			this.doc = new System.Xml.XPath.XPathDocument(xmlReader);
		}

		// The next several "Get" functions are utilities for parsing the capabilities
		// document.

		static public string ExpandPattern(string pattern)
		{
			// The 'Url' vs. 'URL' case is so prevalent that we account for it here rather
			// than in all the invoking instances.
			if ((!pattern.Equals(string.Empty)) && (pattern.IndexOf("Url") >= 0))
			{
				pattern = pattern + "|" + pattern.Replace("Url", "URL");
			}
			return pattern = pattern + "|" + pattern.ToLower() + "|" + pattern.ToUpper();
		}

		internal static string GetStringInstance(System.Xml.XPath.XPathNavigator node, string selectPattern)
		{
			string retVal = string.Empty;
			System.Xml.XPath.XPathNodeIterator iter = node.Select(ExpandPattern(selectPattern));
			if (iter.MoveNext())
			{
				retVal = iter.Current.Value;
			}
			return retVal;
		}

		internal static string GetStringInstance(System.Xml.XPath.XPathDocument doc, string selectPattern)
		{
			return GetStringInstance(doc.CreateNavigator(), selectPattern);
		}

		internal static string[] GetStringsInstance(System.Xml.XPath.XPathNavigator node, string selectPattern)
		{
			System.Xml.XPath.XPathNodeIterator iter = node.Select(ExpandPattern(selectPattern));
			string[] retVal = new string[iter.Count];
			while (iter.MoveNext())
			{
				retVal[iter.CurrentPosition - 1] = iter.Current.Value;
			}
			return retVal;
		}

		internal static string[] GetStringsInstance(System.Xml.XPath.XPathDocument doc, string selectPattern)
		{
			return GetStringsInstance(doc.CreateNavigator(), selectPattern);
		}

		internal static string GetOnlineResourceInstance(System.Xml.XPath.XPathNavigator node, string selectPattern)
		{
			string retVal = string.Empty;
			System.Xml.XPath.XPathNodeIterator iter = node.Select(ExpandPattern(selectPattern));
			if (iter.MoveNext())
			{
				retVal = iter.Current.GetAttribute("href", "http://www.w3.org/1999/xlink");
			}
			return retVal;
		}

		internal static string GetOnlineResourceInstance(System.Xml.XPath.XPathDocument doc, string selectPattern)
		{
			return GetOnlineResourceInstance(doc.CreateNavigator(), selectPattern);
		}

		internal static bool GetBooleanInstance(System.Xml.XPath.XPathNavigator node, string selectPattern)
		{
			string q = Capabilities.GetStringInstance(node, selectPattern);
			return q.Trim().Equals("1");
		}

		// These following functions use an XPath string to identify the elements of
		// the XML capabilities to find and return.

		public string ServiceOnlineResource
		{
			get { return GetOnlineResourceInstance(this.doc, @"/*/Service/OnlineResource"); }
		}

		public string Version
		{
			get { return GetStringInstance(this.doc, @"/*/@Version"); }
		}

		public string UpdateSequence
		{
			get { return GetStringInstance(this.doc, @"/*/@updateSequence|./*/UpdateSequence"); }
		}

		public string ServiceName
		{
			get { return GetStringInstance(this.doc, @"/*/Service/Name"); }
		}

		public string ServiceTitle
		{
			get { return GetStringInstance(this.doc, @"/*/Service/Title"); }
		}

		public string ServiceAbstract
		{
			get { return GetStringInstance(this.doc, @"/*/Service/Abstract"); }
		}

		public string[] ServiceKeywordList
		{
			get { return GetStringsInstance(this.doc, @"/*/Service/KeywordList/Keyword"); }
		}

		public string ServiceFees
		{
			get { return GetStringInstance(this.doc, @"/*/Service/Fees"); }
		}

		public string ServiceAccessConstraints
		{
			get { return GetStringInstance(this.doc, @"/*/Service/AccessConstraints"); }
		}

		public string ServiceContactPerson
		{
			get { return GetStringInstance(this.doc, @"/*/Service/ContactInformation/ContactPersonPrimary/ContactPerson"); }
		}

		public string ServiceContactOrganization
		{
			get { return GetStringInstance(this.doc, @"/*/Service/ContactInformation/ContactPersonPrimary/ContactOrganization"); }
		}

		public string ServiceContactPosition
		{
			get { return GetStringInstance(this.doc, @"/*/Service/ContactInformation/ContactPosition"); }
		}

		public string ServiceContactVoiceTelephone
		{
			get { return GetStringInstance(this.doc, @"/*/Service/ContactInformation/ContactVoiceTelephone"); }
		}

		public string ServiceContactFacsimileTelephone
		{
			get { return GetStringInstance(this.doc, @"/*/Service/ContactInformation/ContactFacsimileTelephone"); }
		}

		public string ServiceContactElectronicMailAddress
		{
			get { return GetStringInstance(this.doc, @"/*/Service/ContactInformation/ContactElectronicMailAddress"); }
		}

		public string ServiceContactAddressType
		{
			get { return GetStringInstance(this.doc, @"/*/Service/ContactInformation/ContactAddress/AddressType"); }
		}

		public string ServiceContactAddress
		{
			get { return GetStringInstance(this.doc, @"/*/Service/ContactInformation/ContactAddress/Address"); }
		}

		public string ServiceContactAddressCity
		{
			get { return GetStringInstance(this.doc, @"/*/Service/ContactInformation/ContactAddress/City"); }
		}

		public string ServiceContactAddressStateOrProvince
		{
			get { return GetStringInstance(this.doc, @"/*/Service/ContactInformation/ContactAddress/StateOrProvince"); }
		}

		public string ServiceContactAddressPostCode
		{
			get { return GetStringInstance(this.doc, @"/*/Service/ContactInformation/ContactAddress/PostCode"); }
		}

		public string ServiceContactAddressCountry
		{
			get { return GetStringInstance(this.doc, @"/*/Service/ContactInformation/ContactAddress/Country"); }
		}

		public string GetCapabilitiesRequestUri
		{
			get { return GetOnlineResourceInstance(this.doc, @"/*/Capability/Request/GetCapabilities/DCPType/HTTP/Get/OnlineResource"); }
		}

		public string GetMapRequestUri
		{
			get { return GetOnlineResourceInstance(this.doc, @"/*/Capability/Request/GetMap/DCPType/HTTP/Get/OnlineResource"); }
		}

		public string GetFeatureInfoRequestUri
		{
			get { return GetOnlineResourceInstance(this.doc, @"/*/Capability/Request/GetFeatureInfo/DCPType/HTTP/Get/OnlineResource"); }
		}

		public string DescribeLayerRequestUri
		{
			get { return GetOnlineResourceInstance(this.doc, @"/*/Capability/Request/DescribeLayer/DCPType/HTTP/Get/OnlineResource"); }
		}

		public string GetLegendGraphicRequestUri
		{
			get { return GetOnlineResourceInstance(this.doc, @"/*/Capability/Request/GetLegendGraphic/DCPType/HTTP/Get/OnlineResource"); }
		}

		public string GetStylesRequestUri
		{
			get { return GetOnlineResourceInstance(this.doc, @"/*/Capability/Request/GetStyles/DCPType/HTTP/Get/OnlineResource"); }
		}

		public string PutStylesRequestUri
		{
			get { return GetOnlineResourceInstance(this.doc, @"/*/Capability/Request/PutStyles/DCPType/HTTP/Get/OnlineResource"); }
		}

		public string[] GetCapabilitiesRequestFormats
		{
			get { return GetStringsInstance(this.doc, @"/*/Capability/Request/GetCapabilities/Format"); }
		}

		public string[] GetMapRequestFormats
		{
			get { return GetStringsInstance(this.doc, @"/*/Capability/Request/GetMap/Format"); }
		}

		public string[] GetFeatureInfoRequestFormats
		{
			get { return GetStringsInstance(this.doc, @"/*/Capability/Request/GetFeatureInfo/Format"); }
		}

		public string[] DescribeLayerRequestFormats
		{
			get { return GetStringsInstance(this.doc, @"/*/Capability/Request/DescribeLayer/Format"); }
		}

		public string[] GetLegendGraphicRequestFormats
		{
			get { return GetStringsInstance(this.doc, @"/*/Capability/Request/GetLegendGraphic/Format"); }
		}

		public string[] GetStylesRequestFormats
		{
			get { return GetStringsInstance(this.doc, @"/*/Capability/Request/GetStyles/Format"); }
		}

		public string[] PutStylesRequestFormats
		{
			get { return GetStringsInstance(this.doc, @"/*/Capability/Request/PutStyles/Format"); }
		}

		public string[] ExceptionFormats
		{
			get { return GetStringsInstance(this.doc, @"/*/Capability/Exception/Format"); }
		}

		public Layer[] Layers
		{
			get
			{
				System.Xml.XPath.XPathNodeIterator iter
				  = this.doc.CreateNavigator().Select(ExpandPattern(@"/*/Capability"));
				if (iter.MoveNext())
				{
					return Layer.GetLayers(iter.Current, this.server);
				}
				else
				{
					return new Layer[0];
				}
			}
		}

		public class UserDefinedSymbolizationType
		{
			public bool SupportSld;
			public bool UserLayer;
			public bool UserStyle;
			public bool RemoteWfs;
		}

		public UserDefinedSymbolizationType UserDefinedSymbolization
		{
			get
			{
				System.Xml.XPath.XPathNodeIterator iter
				  = this.doc.CreateNavigator().Select(ExpandPattern(@"./UserDefinedSymbolization"));
				UserDefinedSymbolizationType retVal = new UserDefinedSymbolizationType();
				if (iter.MoveNext())
				{
					retVal.SupportSld = GetBooleanInstance(iter.Current, @"./@SupportSld|./@SupportSLD");
					retVal.UserLayer = GetBooleanInstance(iter.Current, @"./@UserLayer");
					retVal.UserStyle = GetBooleanInstance(iter.Current, @"./@UserStyle");
					retVal.RemoteWfs = GetBooleanInstance(iter.Current, @"./@RemoteWfs|./@RemoteWFS");
				}
				return retVal;
			}
		}
	}
}