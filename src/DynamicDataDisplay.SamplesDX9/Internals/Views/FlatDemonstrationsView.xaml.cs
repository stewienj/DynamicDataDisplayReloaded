using DynamicDataDisplay.SamplesDX9.Internals.Models;
using DynamicDataDisplay.SamplesDX9.Internals.ViewModels;
using System.Linq;
using System.Windows;

namespace DynamicDataDisplay.SamplesDX9.Internals.Views
{
	/// <summary>
	/// Interaction logic for FlatDemonstrationsView.xaml
	/// </summary>
	public partial class FlatDemonstrationsView : ViewBase
	{
		public FlatDemonstrationsView()
		{
			InitializeComponent();
		}

		private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != null)
			{
				SamplesCollection collection = (SamplesCollection)e.NewValue;

				var query = from r in collection.Releases
							from d in r.Demonstrations
							select new DemonstrationViewModel { Demonstration = d, Version = r.Version };
				FlatViewModel viewModel = new FlatViewModel(query.ToList());

				itemsControl.DataContext = viewModel.Demonstrations;
			}
		}
	}
}
