using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace DynamicDataDisplay.Charts
{
	public static class LiveToolTipService
	{

		#region Properties

		public static object GetToolTip(DependencyObject obj)
		{
			return (object)obj.GetValue(ToolTipProperty);
		}

		public static void SetToolTip(DependencyObject obj, object value)
		{
			obj.SetValue(ToolTipProperty, value);
		}

		public static readonly DependencyProperty ToolTipProperty = DependencyProperty.RegisterAttached(
		  "ToolTip",
		  typeof(object),
		  typeof(LiveToolTipService),
		  new FrameworkPropertyMetadata(null, OnToolTipChanged));

		private static LiveToolTip GetLiveToolTip(DependencyObject obj)
		{
			return (LiveToolTip)obj.GetValue(LiveToolTipProperty);
		}

		private static void SetLiveToolTip(DependencyObject obj, LiveToolTip value)
		{
			obj.SetValue(LiveToolTipProperty, value);
		}

		private static readonly DependencyProperty LiveToolTipProperty = DependencyProperty.RegisterAttached(
		  "LiveToolTip",
		  typeof(LiveToolTip),
		  typeof(LiveToolTipService),
		  new FrameworkPropertyMetadata(null));

		#region Opacity

		public static double GetTooltipOpacity(DependencyObject obj)
		{
			return (double)obj.GetValue(TooltipOpacityProperty);
		}

		public static void SetTooltipOpacity(DependencyObject obj, double value)
		{
			obj.SetValue(TooltipOpacityProperty, value);
		}

		public static readonly DependencyProperty TooltipOpacityProperty = DependencyProperty.RegisterAttached(
		  "TooltipOpacity",
		  typeof(double),
		  typeof(LiveToolTipService),
		  new FrameworkPropertyMetadata(1.0, OnTooltipOpacityChanged));

		private static void OnTooltipOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			LiveToolTip liveTooltip = GetLiveToolTip(d);
			if (liveTooltip != null)
			{
				liveTooltip.Opacity = (double)e.NewValue;
			}
		}

		#endregion // end of Opacity

		#region IsPropertyProxy property

		public static bool GetIsPropertyProxy(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsPropertyProxyProperty);
		}

		public static void SetIsPropertyProxy(DependencyObject obj, bool value)
		{
			obj.SetValue(IsPropertyProxyProperty, value);
		}

		public static readonly DependencyProperty IsPropertyProxyProperty = DependencyProperty.RegisterAttached(
		  "IsPropertyProxy",
		  typeof(bool),
		  typeof(LiveToolTipService),
		  new FrameworkPropertyMetadata(false));

		#endregion // end of IsPropertyProxy property


		#region FollowMouseCursor

		public static bool GetFollowMouseCursor(DependencyObject obj)
		{
			return (bool)obj.GetValue(FollowMouseCursorProperty);
		}

		public static void SetFollowMouseCursor(DependencyObject obj, bool value)
		{
			obj.SetValue(FollowMouseCursorProperty, value);
		}

		// Using a DependencyProperty as the backing store for FollowMouseCursor.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty FollowMouseCursorProperty =
			DependencyProperty.RegisterAttached("FollowMouseCursor", typeof(bool), typeof(LiveToolTipService), new PropertyMetadata(true));


		#endregion


		#region ToolTipOffset X / Y

		public static int GetToolTipOffsetX(DependencyObject obj)
		{
			return (int)obj.GetValue(ToolTipOffsetXProperty);
		}

		public static void SetToolTipOffsetX(DependencyObject obj, int value)
		{
			obj.SetValue(ToolTipOffsetXProperty, value);
		}

		// Using a DependencyProperty as the backing store for ToolTipOffsetX.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ToolTipOffsetXProperty =
			DependencyProperty.RegisterAttached("ToolTipOffsetX", typeof(int), typeof(LiveToolTipService), new PropertyMetadata(0));



		public static int GetToolTipOffsetY(DependencyObject obj)
		{
			return (int)obj.GetValue(ToolTipOffsetYProperty);
		}

		public static void SetToolTipOffsetY(DependencyObject obj, int value)
		{
			obj.SetValue(ToolTipOffsetYProperty, value);
		}

		// Using a DependencyProperty as the backing store for ToolTipOffsetY.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ToolTipOffsetYProperty =
			DependencyProperty.RegisterAttached("ToolTipOffsetY", typeof(int), typeof(LiveToolTipService), new PropertyMetadata(0));


		#endregion


		#endregion

		private static void OnToolTipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FrameworkElement source = (FrameworkElement)d;

			if (e.NewValue == null)
			{
				source.Loaded -= source_Loaded;
				source.ClearValue(LiveToolTipProperty);
			}

			if (GetIsPropertyProxy(source)) return;

			var content = e.NewValue;

			DataTemplate template = content as DataTemplate;
			if (template != null)
			{
				content = template.LoadContent();
			}

			LiveToolTip tooltip = null;
			if (e.NewValue is LiveToolTip)
			{
				tooltip = e.NewValue as LiveToolTip;
			}
			else
			{
				tooltip = new LiveToolTip { Content = content };
			}

			if (tooltip == null && e.OldValue == null)
			{
				tooltip = new LiveToolTip { Content = content };
			}

			if (tooltip != null)
			{
				SetLiveToolTip(source, tooltip);
				if (!source.IsVisible)
				{
					source.IsVisibleChanged += source_Visible;
				}
				else if (!source.IsLoaded)
				{
					source.Loaded += source_Loaded;
				}
				else
				{
					AddTooltip(source);
				}
			}
		}

		private static void source_Visible(object sender, DependencyPropertyChangedEventArgs e)
		{
			var source = sender as FrameworkElement;
			if (source != null)
			{
				source.IsVisibleChanged -= source_Visible;
				if (!source.IsLoaded)
				{
					source.Loaded += source_Loaded;
				}
				else
				{
					AddTooltip(source);
				}
			}
		}

		private static void AddTooltipForElement(FrameworkElement source, LiveToolTip tooltip)
		{
			AdornerLayer layer = AdornerLayer.GetAdornerLayer(source);

			if (layer != null)
			{
				LiveToolTipAdorner adorner = new LiveToolTipAdorner(source, tooltip);
				layer.Add(adorner);
			}
		}

		private static void source_Loaded(object sender, RoutedEventArgs e)
		{
			FrameworkElement source = (FrameworkElement)sender;

			if (source.IsLoaded)
			{
				AddTooltip(source);
			}
		}

		private static void AddTooltip(FrameworkElement source)
		{
			if (DesignerProperties.GetIsInDesignMode(source)) return;

			LiveToolTip tooltip = GetLiveToolTip(source);

			Window window = Window.GetWindow(source);
			FrameworkElement child = source;
			FrameworkElement parent = null;
			if (window != null)
			{
				while (parent != window)
				{
					parent = (FrameworkElement)VisualTreeHelper.GetParent(child);
					child = parent;
					var nameScope = NameScope.GetNameScope(parent);
					if (nameScope != null)
					{
						string nameScopeName = nameScope.ToString();
						if (nameScopeName != "System.Windows.TemplateNameScope")
						{
							NameScope.SetNameScope(tooltip, nameScope);
							break;
						}
					}
				}
			}

			var binding = BindingOperations.GetBinding(tooltip, LiveToolTip.ContentProperty);
			if (binding != null)
			{
				BindingOperations.ClearBinding(tooltip, LiveToolTip.ContentProperty);
				BindingOperations.SetBinding(tooltip, LiveToolTip.ContentProperty, binding);
			}

			Binding dataContextBinding = new Binding { Path = new PropertyPath("DataContext"), Source = source };
			tooltip.SetBinding(LiveToolTip.DataContextProperty, dataContextBinding);

			tooltip.Owner = source;
			if (GetTooltipOpacity(source) != (double)LiveToolTipService.TooltipOpacityProperty.DefaultMetadata.DefaultValue)
			{
				tooltip.Opacity = LiveToolTipService.GetTooltipOpacity(source);
			}

			AddTooltipForElement(source, tooltip);
		}
	}
}
