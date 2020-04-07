using System.Linq;
using System.Windows.Controls;

namespace DynamicDataDisplay.Common.Auxiliary
{
	public static class MenuItemExtensions
	{
		public static MenuItem FindChildByHeader(this MenuItem parent, string header)
		{
			return parent.Items.OfType<MenuItem>().Where(subMenu => (subMenu.Header as string) == header).FirstOrDefault();
		}
	}
}
