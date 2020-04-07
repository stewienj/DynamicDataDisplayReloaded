using System;

namespace DynamicDataDisplay.Common
{
	public interface INotifyingPanel
	{
		NotifyingUIElementCollection NotifyingChildren { get; }
		event EventHandler ChildrenCreated;
	}
}
