﻿using System.Windows.Controls;

namespace Microsoft.Research.DynamicDataDisplay.Samples.Demos.Custom
{
	/// <summary>
	/// Interaction logic for HeatMapOnMapSample.xaml
	/// </summary>
	public partial class GroupedMarkersOnMapSample : Page
	{
		public GroupedMarkersOnMapSample()
		{
			InitializeComponent();
			groupedMarkers.CreateRandomMapData();
		}
	}
}
