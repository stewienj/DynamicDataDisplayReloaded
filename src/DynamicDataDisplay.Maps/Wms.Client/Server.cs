// $File: //depot/WMS/WMS Overview/Wms.Client/Server.cs $ $Revision: #1 $ $Change: 20 $ $DateTime: 2004/05/23 23:42:06 $

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Wms.Client
{

	/// <summary>
	/// Represents a WMS server and holds its capabilities description.
	/// </summary>
	public class Server
	{
		//-------------------------------------------------------------------------
		private Wms.Client.Capabilities capabilities;

		//-------------------------------------------------------------------------
		public Server(string filePath)
		{
			this.capabilities = new Capabilities(filePath, this);
		}

		//-------------------------------------------------------------------------
		public Capabilities Capabilities
		{
			get { return this.capabilities; }
		}

		//-------------------------------------------------------------------------
		public System.Uri Uri
		{
			get { return new System.Uri(this.capabilities.GetCapabilitiesRequestUri); }
		}

		//-------------------------------------------------------------------------
		public MapRequestBuilder CreateMapRequest()
		{
			return new MapRequestBuilder(new System.Uri(this.capabilities.GetMapRequestUri));
		}


		//-------------------------------------------------------------------------
		public Layer FindLayerByName(string layerName)
		{
			return FindLayer((l) => layerName == l.Name);
		}

		//-------------------------------------------------------------------------
		// Recursively runs a function on all Layers in this server
		public Layer FindLayer(Func<Layer, bool> test)
		{
			if (Capabilities == null) return null;
			return FindLayer(Capabilities.Layers, test);
		}

		//-------------------------------------------------------------------------
		private Layer FindLayer(IEnumerable<Layer> list, Func<Layer, bool> test)
		{
			foreach (var l in list)
			{
				if (test(l)) return l;

				var r = FindLayer(l.Layers, test);
				if (r != null) return r;
			}

			return null;
		}

		//-------------------------------------------------------------------------
		public static Server CreateService(string url, string localCachePath)
		{
			UriBuilder uriBuilder = new UriBuilder(url);
			CapabilitiesRequestBuilder capsRequest = new CapabilitiesRequestBuilder(uriBuilder.Uri);

			string fileName = Path.Combine(localCachePath, string.Format("{0}-{1}.xml", capsRequest.Uri.Host, uriBuilder.Uri.AbsolutePath.GetHashCode()));

			if (!File.Exists(fileName))
			{
				// Retrieve the capabilities document and cache it locally.
				WebRequest wr = WebRequest.Create(capsRequest.Uri);
				wr.Proxy.Credentials = CredentialCache.DefaultCredentials;
				try
				{
					WebResponse response = wr.GetResponse();
					copyStreamToFile(response.GetResponseStream(), fileName);
				}
				catch (Exception ex)
				{
					Console.Error.WriteLine("WMS.Client.Server {0} failed while trying to GetCapabilities.  {1}", uriBuilder.Uri, ex.Message);
					return null;
				}
			}

			if (File.Exists(fileName))
			{
				// if the server file exists use it to create a new Server
				return new Server(fileName);
			}

			return null;
		}

		//-------------------------------------------------------------------------
		public List<Layer> AllLayers()
		{
			var layers = new List<Layer>();

			FindLayer((l) =>
			{
				if (!string.IsNullOrWhiteSpace(l.Name))
				{
					layers.Add(l);
				}
				return false;
			});

			return layers;
		}

		//-------------------------------------------------------------------------
		private static void copyStreamToFile(System.IO.Stream stream, string destination)
		{
			using (System.IO.BufferedStream bs = new System.IO.BufferedStream(stream))
			{
				using (System.IO.FileStream os = System.IO.File.OpenWrite(destination))
				{
					byte[] buffer = new byte[2 * 4096];
					int nBytes;
					while ((nBytes = bs.Read(buffer, 0, buffer.Length)) > 0)
					{
						os.Write(buffer, 0, nBytes);
					}
				}
			}
		}

	}
}