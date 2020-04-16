// $File: //depot/WMS/WMS Overview/Wms.Client/WmsException.cs $ $Revision: #1 $ $Change: 20 $ $DateTime: 2004/05/23 23:42:06 $

namespace Wms.Client
{
	/// <summary>
	/// Converts WMS exceptions returned from a WMS server as XML to .Net exceptions.
	/// </summary>
	public class WmsException : System.ApplicationException
	{
		internal WmsException(string exceptionFile) : base(Wms.Client.WmsException.extractMessage(exceptionFile))
		{
		}

		private static string extractMessage(string exceptionFile)
		{
			System.Text.StringBuilder retVal = new System.Text.StringBuilder();
			try
			{
				System.Xml.XPath.XPathDocument doc = new System.Xml.XPath.XPathDocument(exceptionFile);
				System.Xml.XPath.XPathNavigator nav = doc.CreateNavigator();
				System.Xml.XPath.XPathNodeIterator iter = nav.Select(@"/*/ServiceException");
				while (iter.MoveNext())
				{
					if (retVal.Length > 0)
						retVal.Append(System.Environment.NewLine);
					System.Xml.XPath.XPathNodeIterator attIter = iter.Current.Select(@"/code|/Code|/CODE");
					if (attIter.MoveNext())
					{
						retVal.Append("(" + iter.Current.Value.Trim() + ") ");
					}
					retVal.Append(iter.Current.Value.Trim());
				}
			}
			catch (System.Exception)
			{
				retVal.Append("Unable to extract the error description from the information returned by the WMS server.");
			}
			return retVal.ToString();
		}
	}
}