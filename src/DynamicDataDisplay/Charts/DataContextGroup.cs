using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	/// <summary>
	/// This class hosts child controls and adds them to the parent plotter, setting the data context on the child controls.
	/// Because this class isn't a visual if you want to set the binding as a parent, you have to use a binding property.
	/// e.g.
	/// <UserControl.Resources>
	///  <sf:BindingProxy x:Key="TerrainProfileBinding" Data="{Binding RelativeSource={RelativeSource AncestorType=UserControl}, Path=LineOfSightData.TerrainProfile}" />
	/// </UserControl.Resources>
	///    <d3:DataContextGroup ChildContext = "{Binding Source={StaticResource TerrainProfileBinding}, Path=Data}" >
	///      < d3:LineGraph PointsSource = "{Binding LonLatDegSourceToIntersect}" Stroke="Yellow" StrokeThickness="3" Panel.ZIndex="10001"/>
	///      <d3:LineGraph PointsSource = "{Binding LonLatDegIntersectToIntersect}" Stroke="Yellow" StrokeThickness="3" Panel.ZIndex="10001"/>
	///      <d3:LineGraph PointsSource = "{Binding LonLatDegIntersectToTarget}" Stroke="Yellow" StrokeThickness="3" Panel.ZIndex="10001"/>
	///      <d3:LineGraph PointsSource = "{Binding LonLatDegSourceToIntersect}" Stroke="Green" StrokeThickness="1" Panel.ZIndex="10002"/>
	///      <d3:LineGraph PointsSource = "{Binding LonLatDegIntersectToIntersect}" Stroke="Red" StrokeThickness="1" Panel.ZIndex="10002"/>
	///      <d3:LineGraph PointsSource = "{Binding LonLatDegIntersectToTarget}" Stroke="Green" StrokeThickness="1" Panel.ZIndex="10002"/>
	///    </d3:DataContextGroup>
	/// </summary>
	[ContentProperty("Items")]
	public class DataContextGroup : DependencyObject, IPlotterElement
	{
		public DataContextGroup()
		{
			Items.CollectionChanged += Items_CollectionChanged;
		}


		private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (_plotter != null)
			{
				foreach (IPlotterElement oldItem in e.OldItems ?? new object[0])
				{
					((FrameworkElement)oldItem).DataContext = null;
					Plotter2D.Children.Remove(oldItem);
				}
				foreach (FrameworkElement newItem in e.NewItems ?? new object[0])
				{
					newItem.SetBinding(FrameworkElement.DataContextProperty, new Binding { Path = new PropertyPath("ChildContext"), Source = this });
					Plotter2D.Children.Add(newItem);
				}
			}
		}

		protected virtual Panel HostPanel
		{
			get { return _plotter.CentralGrid; }
		}


		private Plotter2D _plotter;
		protected Plotter2D Plotter2D
		{
			get { return _plotter; }
		}

		Plotter IPlotterElement.Plotter
		{
			get { return _plotter; }
		}

		public void OnPlotterAttached(Plotter plotter)
		{
			_plotter = (Plotter2D)plotter;
			Task.Factory.StartNew(() =>
			{
				Dispatcher.Invoke(() =>
		  {
				  foreach (FrameworkElement newItem in Items)
				  {
					  newItem.SetBinding(FrameworkElement.DataContextProperty, new Binding { Path = new PropertyPath("ChildContext"), Source = this });
					  Plotter2D.Children.Add(newItem);
				  }
			  });
			});
		}

		public void OnPlotterDetaching(Plotter plotter)
		{
			//throw new NotImplementedException();
		}

		public ObservableCollection<object> Items
		{
			get { return (ObservableCollection<object>)GetValue(ItemsProperty); }
			set { SetValue(ItemsProperty, value); }
		}

		public static readonly DependencyProperty ItemsProperty =
			DependencyProperty.Register("Items", typeof(ObservableCollection<object>), typeof(DataContextGroup), new UIPropertyMetadata(new ObservableCollection<object>()));



		public object ChildContext
		{
			get { return (object)GetValue(ChildContextProperty); }
			set { SetValue(ChildContextProperty, value); }
		}

		// Using a DependencyProperty as the backing store for ChildContenxt.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ChildContextProperty =
			DependencyProperty.Register("ChildContext", typeof(object), typeof(DataContextGroup), new PropertyMetadata(null));


	}
}
