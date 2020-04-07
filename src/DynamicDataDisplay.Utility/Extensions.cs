using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DynamicDataDisplay.Utility
{
	public static class Extensions
	{
		public static IEnumerable<List<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
		{
			if (batchSize == 0)
			{
				yield break;
			}
			List<T> retVal = new List<T>();
			int count = 0;
			foreach (var item in source)
			{
				if (count >= batchSize)
				{
					yield return retVal;
					count = 0;
					retVal = new List<T>();
				}
				retVal.Add(item);
				count++;
			}
			yield return retVal;
		}

		public static IEnumerable<List<T>> BatchConditionalSplit<T>(this IEnumerable<T> source, int batchSize, Func<T, List<T>, bool> condition)
		{
			if (batchSize == 0)
			{
				yield break;
			}
			List<T> retVal = new List<T>();
			int count = 0;
			foreach (var item in source)
			{
				if (count >= batchSize)
				{
					if (condition(item, retVal))
					{
						yield return retVal;
						count = 0;
						retVal = new List<T>();
					}
				}
				retVal.Add(item);
				count++;
			}
			yield return retVal;
		}

		public static void dg_AutoGeneratingColumnOneWay(this UserControl control, object sender, DataGridAutoGeneratingColumnEventArgs e)
		{
			dg_AutoGeneratingColumn(e, BindingMode.OneWay);
		}

		public static void dg_AutoGeneratingColumnOneWay(this System.Windows.Window control, object sender, DataGridAutoGeneratingColumnEventArgs e)
		{
			dg_AutoGeneratingColumn(e, BindingMode.OneWay);
		}

		/// <summary>
		/// Callback for accessing and updating columns during autogeneration, in case you want to filter or change the headers.
		/// DisplayNameAttribute meta data is read off the properties, if the DisplayName == "" then cancel creating the column.
		/// </summary>
		private static void dg_AutoGeneratingColumn(DataGridAutoGeneratingColumnEventArgs e, BindingMode bindingMode = BindingMode.TwoWay)
		{
			// Need to make sure we aren't trying to do two way binding on a read only property
			var prop = e.PropertyDescriptor as System.ComponentModel.PropertyDescriptor;
			if (prop?.IsReadOnly == true)
			{
				bindingMode = BindingMode.OneWay;
			}

			string oldHeader = e.Column.Header.ToString();
			// If the display name is "" then we don't show this property
			string displayName = oldHeader;//PropertyHelper.GetPropertyDisplayName(e.PropertyDescriptor);
			if (!string.IsNullOrEmpty(displayName))
			{
				e.Column.Header = displayName;
			}
			else if (displayName == "")
			{
				e.Cancel = true;
				return;
			}

			// This is so we can toggle checkBoxes with one click, rather than the default behaviour of
			// DataGridCheckBoxColumn where you have to select the row, then toggle the checkbox
			if (e.Column is DataGridCheckBoxColumn)
			{
				//Set up the CheckBox Factory
				FrameworkElementFactory cbFactory = new FrameworkElementFactory(typeof(CheckBox));
				Binding binding = new Binding(oldHeader);
				binding.Mode = bindingMode;
				binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				cbFactory.SetBinding(CheckBox.IsCheckedProperty, binding);
				cbFactory.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);

				// Set up the DataTemplate for the column
				var checkBoxTemplate = new System.Windows.DataTemplate();
				checkBoxTemplate.VisualTree = cbFactory;

				// Now create a new column and assign it
				DataGridTemplateColumn newColumn = new DataGridTemplateColumn();
				newColumn.CellTemplate = checkBoxTemplate;
				newColumn.Header = e.Column.Header;
				e.Column = newColumn;
			}
			else if (e.PropertyType == typeof(System.Windows.Input.ICommand))
			{
				//Set up the CheckBox Factory
				FrameworkElementFactory cbFactory = new FrameworkElementFactory(typeof(Button));
				Binding binding = new Binding(oldHeader);
				binding.Mode = bindingMode;
				binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				cbFactory.SetBinding(Button.CommandProperty, binding);
				cbFactory.SetValue(Button.HorizontalAlignmentProperty, HorizontalAlignment.Center);
				cbFactory.SetValue(Button.ContentProperty, e.Column.Header);

				// Set up the DataTemplate for the column
				var buttonTemplate = new System.Windows.DataTemplate();
				buttonTemplate.VisualTree = cbFactory;

				// Now create a new column and assign it
				DataGridTemplateColumn newColumn = new DataGridTemplateColumn();
				newColumn.CellTemplate = buttonTemplate;
				newColumn.Header = e.Column.Header;
				e.Column = newColumn;
			}
			else if (e.PropertyType == typeof(System.Windows.Media.Imaging.BitmapSource))
			{
				//Set up the CheckBox Factory
				FrameworkElementFactory cbFactory = new FrameworkElementFactory(typeof(Image));
				Binding binding = new Binding(oldHeader);
				binding.Mode = bindingMode;
				binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				cbFactory.SetBinding(Image.SourceProperty, binding);
				cbFactory.SetValue(Image.HorizontalAlignmentProperty, HorizontalAlignment.Center);
				cbFactory.SetValue(Image.SnapsToDevicePixelsProperty, true);

				// Set up the DataTemplate for the column
				var imageTemplate = new System.Windows.DataTemplate();
				imageTemplate.VisualTree = cbFactory;

				// Now create a new column and assign it
				DataGridTemplateColumn newColumn = new DataGridTemplateColumn();
				newColumn.CellTemplate = imageTemplate;
				newColumn.Header = e.Column.Header;
				e.Column = newColumn;
			}

			DataGridBoundColumn boundColumn = e.Column as DataGridBoundColumn;
			if (boundColumn != null)
			{
				Binding binding = boundColumn.Binding as Binding;
				if (binding != null)
				{
					binding.Mode = bindingMode;
				}
			}

		}
	}
}
