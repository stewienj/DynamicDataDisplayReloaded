using System.Windows;
using System.Windows.Controls;

namespace DynamicDataDisplay.SamplesDX9.Internals.Views
{
	/// <summary>
	/// Interaction logic for ViewBase.xaml
	/// </summary>
	public class ViewBase : UserControl
	{
		public ViewBase()
		{
		}

		public object SelectedValue
		{
			get { return (object)GetValue(SelectedValueProperty); }
			set { SetValue(SelectedValueProperty, value); }
		}

		public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.Register(
		  "SelectedValue",
		  typeof(object),
		  typeof(ViewBase),
		  new FrameworkPropertyMetadata(null, OnSelectedValueChanged));

		private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{

		}
	}
}
