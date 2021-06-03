using DynamicDataDisplay;
using DynamicDataDisplay.Charts.Isolines;
using DynamicDataDisplay.Charts.Maps;
using DynamicDataDisplay.Common.Auxiliary;
using System;
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

				Dispatcher.BeginInvoke((Action)(() =>
				{
					plotter.EndLongOperation();
				}), DispatcherPriority.Send);

				tileServer.Dispatcher.BeginInvoke((Action)(() =>
				{
					tileServer.ContentBounds = new DataRect(-180, -90, 360, 180);
				}), DispatcherPriority.Send);

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
