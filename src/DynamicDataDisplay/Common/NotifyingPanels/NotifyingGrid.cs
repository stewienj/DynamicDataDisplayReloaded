using System;
using System.Windows;
using System.Windows.Controls;

namespace DynamicDataDisplay.Common
{
	public sealed class NotifyingGrid : Grid, INotifyingPanel
	{
		#region INotifyingPanel Members

		private NotifyingUIElementCollection notifyingChildren;
		public NotifyingUIElementCollection NotifyingChildren
		{
			get { return notifyingChildren; }
		}

		protected override UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent)
		{
			notifyingChildren = new NotifyingUIElementCollection(this, logicalParent);
			ChildrenCreated.Raise(this);

			return notifyingChildren;
		}

		public event EventHandler ChildrenCreated;

		#endregion
	}
}
