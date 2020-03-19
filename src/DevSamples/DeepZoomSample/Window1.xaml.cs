using System;
using System.Linq;
using System.Windows;
using System.Xml.Schema;

namespace DeepZoomSample
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			//XmlReaderSettings settings = new XmlReaderSettings();
			//XmlSchema schema = null;
			//using (FileStream fs = new FileStream(@"..\..\DeepZoomSchema.xsd", FileMode.Open))
			//{
			//    schema = XmlSchema.Read(fs, new ValidationEventHandler(SchemaValidationCallback));
			//}
			//settings.Schemas.Add(schema);

			string path = Environment.CommandLine.Trim("\" \\".ToArray());
			var directory = System.IO.Path.GetDirectoryName(path);


			viewer.ImagePath = System.IO.Path.Combine(directory, "DeepZoomSchema.xsd"); ;
		}

		private void SchemaValidationCallback(object sender, ValidationEventArgs e) { }
	}
}
