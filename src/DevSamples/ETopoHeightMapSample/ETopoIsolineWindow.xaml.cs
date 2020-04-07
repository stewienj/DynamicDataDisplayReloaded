using DynamicDataDisplay.Common.Auxiliary;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ETopoHeightMapSample
{
	/// <summary>
	/// Interaction logic for ETopoIsolineWindow.xaml
	/// </summary>
	public partial class ETopoIsolineWindow : Window
	{
		public ETopoIsolineWindow()
		{
			InitializeComponent();
		}

		private void plotter_Loaded(object sender, RoutedEventArgs e)
		{
			plotter.BeginLongOperation();

			var task = Task.Run(() =>
			{
				var dataSource = ReliefReader.ReadDataSource();

				Dispatcher.BeginInvoke(() =>
				{
					plotter.EndLongOperation();
					DataContext = dataSource;
				}, DispatcherPriority.Send);
			});
		}
	}
}
