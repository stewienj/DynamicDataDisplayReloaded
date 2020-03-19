using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts.Isolines;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ETopoHeightMapSample
{
	/// <summary>
	/// Interaction logic for ETopoThreadedIsoline.xaml
	/// </summary>
	public partial class ETopoThreadedIsolineWindow : Window
	{
		public ETopoThreadedIsolineWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			renderingMap.FileTileServer = new AutoDisposableFileServer();

			plotter.BeginLongOperation();

			Task task = Task.Run(() =>
			{
				var dataSource = ReliefReader.ReadDataSource();

				Dispatcher.BeginInvoke(() =>
				{
					plotter.EndLongOperation();
				}, DispatcherPriority.Send);

				tileServer.Dispatcher.BeginInvoke(() =>
				{
					tileServer.ContentBounds = new DataRect(-180, -90, 360, 180);
				}, DispatcherPriority.Send);

				tileServer.ChildCreateHandler = () =>
				{
					FastIsolineDisplay isoline = new FastIsolineDisplay { WayBeforeTextMultiplier = 100, LabelStringFormat = "F0" };
					Viewport2D.SetContentBounds(isoline, new DataRect(-180, -90, 360, 180));
					isoline.DataSource = dataSource;

					return isoline;
				};
			});
		}
	}
}
