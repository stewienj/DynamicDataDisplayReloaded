using DynamicDataDisplay.BitmapGraphs;
using DynamicDataDisplay.Charts.Maps;
using DynamicDataDisplay.Common.Auxiliary;
using System;
using System.Windows.Controls;

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

	public class HeatMapOnMapSampleViewModel : NotifyPropertyChanged
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
						Heatmap = heatmap;
						_chartPlotter.Children.Add(heatmap);
					}
					else
					{
						_chartPlotter.Children.Remove(Heatmap);
						Heatmap = null;
						GC.Collect();
						GC.WaitForPendingFinalizers();
						GC.Collect();
						Console.WriteLine($"Heatmp is alive {_heatmapControl.IsAlive}");
					}
				}
			}
		
				/*
				if (SetProperty(ref _heatmapVisible, value))
				{
					if (_heatmapVisible && _heatmap==null)
					{
						_heatmap = new HeatmapChartView();
						_heatmap.CreateRandomMapData();
						_chartPlotter.Children.Add(_heatmap);
					}
					else
					{
						var heatmap = _heatmap;
						Heatmap = null;
						_chartPlotter.Children.Remove(heatmap);
						heatmap.DataContext = null;
						GC.Collect();
						GC.WaitForPendingFinalizers();
					}
				}
				*/
		}
	}
}
