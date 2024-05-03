using DynamicDataDisplay.BitmapGraphs;
using DynamicDataDisplay.Charts.Maps;
using DynamicDataDisplay.Common.Auxiliary;
using System;
using System.Collections;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace DynamicDataDisplay.Samples.Demos.Custom
{
	/// <summary>
	/// Interaction logic for HeatMapOnMapSample.xaml
	/// </summary>
	public partial class HeatMapOnMapSample : Page
	{
		public HeatMapOnMapSample()
		{
			InitializeComponent();
			HeatMapOnMapSampleViewModel heatMapOnMapSampleViewModel = new HeatMapOnMapSampleViewModel(chartPlotter, theGrid);
			this.DataContext = heatMapOnMapSampleViewModel;
			heatMapOnMapSampleViewModel.HeatmapVisible = true;
		}
	}

	public class HeatMapOnMapSampleViewModel : D3NotifyPropertyChanged
	{
		private ChartPlotter _chartPlotter;
		private Grid _grid;

		public HeatMapOnMapSampleViewModel(ChartPlotter chartPlotter, Grid grid)
		{
			_chartPlotter = chartPlotter;
			_grid = grid;
		}

		private HeatmapChartView _heatmap = null;
		public HeatmapChartView Heatmap
		{
			get => _heatmap;
			set => SetProperty(ref _heatmap, value);
		}

		WeakReference _heatmapControl;
		private bool _heatmapVisible = false;
		public bool HeatmapVisible
		{
			get => _heatmapVisible;
			set
			{
				if (SetProperty(ref _heatmapVisible, value))
				{
					if (value)
					{
						var heatmap = new HeatmapChartView();
						_heatmapControl = new WeakReference(heatmap);
						heatmap.CreateRandomMapData();
						Binding binding = new Binding
						{
							Source = this,
							Path = new System.Windows.PropertyPath(nameof(SelectedPoints))
						};
						heatmap.SetBinding(HeatmapChartView.SelectedPointsProperty, binding);
						Heatmap = heatmap;
						_chartPlotter.Children.Add(heatmap);
					}
					else
					{
						// Had a memory leak, was using this code to check where the problem was

						_chartPlotter.Children.Remove(Heatmap);
						Heatmap = null;
						GC.Collect();
						GC.WaitForPendingFinalizers();
						GC.Collect();
						Console.WriteLine($"Heatmp is alive {_heatmapControl.IsAlive}");
					}
				}
			}

		}

		private IEnumerable _selectedPoints = Enumerable.Empty<object>();
		public IEnumerable SelectedPoints
		{
			get => _selectedPoints;
			set => SetProperty(ref _selectedPoints, value);
		}
	}
}
