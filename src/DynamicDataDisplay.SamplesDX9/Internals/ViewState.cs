using System.ComponentModel;
using System.Windows;

namespace DynamicDataDisplay.SamplesDX9.Internals
{
	public class ViewState : DependencyObject, INotifyPropertyChanged
	{
		private static readonly ViewState state = new ViewState();

		public static ViewState State { get { return state; } }

		private object value;
		public object SelectedValue
		{
			get { return value; }
			set
			{
				this.value = value;
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("SelectedValue"));
				}
			}
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}
}
