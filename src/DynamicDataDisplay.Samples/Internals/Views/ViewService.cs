using System;
using System.Diagnostics;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Samples.Internals.Views
{
	public static class ViewService
	{
		public static Uri GetSelectedValue(DependencyObject obj)
		{
			return (Uri)obj.GetValue(SelectedValueProperty);
		}

		public static void SetSelectedValue(DependencyObject obj, Uri value)
		{
			obj.SetValue(SelectedValueProperty, value);
		}

		public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.RegisterAttached(
		  "SelectedValue",
		  typeof(Uri),
		  typeof(ViewService),
		  new FrameworkPropertyMetadata(null, OnSelectedValueChanged));

		private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Debug.WriteLine(e.OldValue + " -> " + e.NewValue);
		}


	}
}
