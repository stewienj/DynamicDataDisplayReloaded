using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Markup;

namespace Microsoft.Research.DynamicDataDisplay.Utility
{
	// EnumToItemsSource, used to make ComboBoxes from Enums
	// Needs on the enumerated type an EnumToDescriptionConverter,
	// e.g:
	// [TypeConverter(typeof(EnumToDescriptionConverter))]
	// public enum TerrainCacheDataType
	// {
	//   [Description("DEM Tile Source")]
	//   DEM,
	//   [Description("DTED Tile Source")]
	//   DTED
	// }
	//
	// xmlns:d3="http://research.microsoft.com/DynamicDataDisplay/1.0"
	//
	// <ComboBox ItemsSource="{d3:EnumToItemsSource {x:Type enums:<YOUR ENUM HERE>}}" />
	//
	public class EnumToItemsSource : MarkupExtension
	{
		private readonly Type _type;

		public EnumToItemsSource(Type type)
		{
			_type = type;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			TypeConverter converter = TypeDescriptor.GetConverter(_type);

			return Enum.GetValues(_type).Cast<Enum>().Where(x => converter.ConvertToString(x) != "");
		}
	}

	public class EnumToItemsSourceWithNull : MarkupExtension
	{
		private readonly Type _type;

		public EnumToItemsSourceWithNull(Type type)
		{
			_type = type;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			TypeConverter converter = TypeDescriptor.GetConverter(_type);
			var empty = new object[] { "" };

			return empty.Concat(Enum.GetValues(_type).Cast<Enum>().Where(x => converter.ConvertToString(x) != "")).ToList();
		}
	}

	// Converts an enumerated type to a string where a binding can't be used because it's not a DependencyProperty
	//
	// Example:
	//
	// <avalonDock:LayoutAnchorablePane DockHeight="*" Name="{sf:EnumToString {x:Static local:DockLocationEnum.TopLeftPane}}">
	//
	public class EnumToString : MarkupExtension
	{
		private readonly object _enum;

		public EnumToString(object value)
		{
			_enum = value;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return _enum.ToString();
		}
	}
}
