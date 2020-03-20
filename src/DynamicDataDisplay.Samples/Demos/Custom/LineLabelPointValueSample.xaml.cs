using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.Samples.Demos.v02.CurrencyExchange;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.Samples.Demos.Custom
{
	/// <summary>
	/// Interaction logic for CurrencyExchangeSample.xaml
	/// </summary>
	public partial class LineLabelPointValueSample : Page
	{
		public LineLabelPointValueSample()
		{
			InitializeComponent();

			Loaded += new RoutedEventHandler(Window1_Loaded);
		}

		List<CurrencyInfo> usd;
		List<CurrencyInfo> eur;
		List<CurrencyInfo> gbp;
		List<CurrencyInfo> jpy;
		private void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			usd = LoadCurrencyRates("usd.txt");
			eur = LoadCurrencyRates("eur.txt");
			gbp = LoadCurrencyRates("gbp.txt");
			jpy = LoadCurrencyRates("jpy.txt");

			Color[] colors = ColorHelper.CreateRandomColors(4);
			plotter.AddLineGraph(CreateCurrencyDataSource(usd), colors[0], 1, "RUB / $");
			plotter.AddLineGraph(CreateCurrencyDataSource(eur), colors[1], 1, "RUB / €");
			plotter.AddLineGraph(CreateCurrencyDataSource(gbp), colors[2], 1, "RUB / £");
			plotter.AddLineGraph(CreateCurrencyDataSource(jpy), colors[3], 1, "RUB / ¥");
		}

		private EnumerableDataSource<CurrencyInfo> CreateCurrencyDataSource(List<CurrencyInfo> rates)
		{
			EnumerableDataSource<CurrencyInfo> ds = new EnumerableDataSource<CurrencyInfo>(rates);
			ds.SetXMapping(ci => dateAxis.ConvertToDouble(ci.Date));
			ds.SetYMapping(ci => ci.Rate);
			return ds;
		}

		private static List<CurrencyInfo> LoadCurrencyRates(string fileName)
		{
			var assembly = Assembly.GetExecutingAssembly();
			using (Stream resourceStream = assembly.GetManifestResourceStream("Microsoft.Research.DynamicDataDisplay.Samples.Demos.v02.CurrencyExchange." + fileName))
			{
				using (StreamReader reader = new StreamReader(resourceStream))
				{
					var strings = reader.ReadToEnd().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

					var res = new List<CurrencyInfo>(strings.Length - 1);
					for (int i = 1; i < strings.Length; i++)
					{
						string line = strings[i];
						string[] subLines = line.Split('\t');

						DateTime date = DateTime.Parse(subLines[1]);
						double rate = double.Parse(subLines[5], CultureInfo.InvariantCulture);

						res.Add(new CurrencyInfo { Date = date, Rate = rate });
					}

					return res;
				}
			}
		}
	}
}
