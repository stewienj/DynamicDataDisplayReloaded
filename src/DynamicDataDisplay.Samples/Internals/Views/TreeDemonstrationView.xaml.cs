using System.Windows;
using System.Windows.Data;

namespace Microsoft.Research.DynamicDataDisplay.Samples.Internals.Views
{
	/// <summary>
	/// Interaction logic for FlatDemonstrationView.xaml
	/// </summary>
	public partial class TreeDemonstrationView : ViewBase
	{
		public TreeDemonstrationView()
		{
			InitializeComponent();
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			//tree.SetBinding(TreeView.SelectedValueProperty, new Binding { Source = ViewState.State, Path = new PropertyPath("SelectedValue") });
			SetBinding(SelectedValueProperty, new Binding { Source = tree, Path = new PropertyPath("SelectedValue") });
		}

	}
}
