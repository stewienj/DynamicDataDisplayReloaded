using DynamicDataDisplay.Common;
using System;
using System.Windows;
using System.Windows.Markup;

namespace DynamicDataDisplay.Charts.Markers
{
	[ContentProperty("Template")]
	public class TemplateMarkerGenerator2 : OldMarkerGenerator
	{
		private DataTemplate template;
		[NotNull]
		public DataTemplate Template
		{
			get { return template; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				if (template != value)
				{
					template = value;
					pool.Clear();
					RaiseChanged();
				}
			}
		}

		private readonly ResourcePool<FrameworkElement> pool = new ResourcePool<FrameworkElement>();

		protected override FrameworkElement CreateMarkerCore(object dataItem)
		{
			if (template == null)
				throw new InvalidOperationException(Strings.Exceptions.TemplateShouldNotBeNull);

			FrameworkElement marker = pool.Get();
			if (marker == null)
			{
				marker = (FrameworkElement)template.LoadContent();
			}

			return marker;
		}

		public override void ReleaseMarker(FrameworkElement element)
		{
			pool.Put(element);
		}
	}
}
