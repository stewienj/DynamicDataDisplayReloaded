using System;

namespace Microsoft.Research.DynamicDataDisplay.Common
{
	public interface INotifyingPanel
	{
		NotifyingUIElementCollection NotifyingChildren { get; }
		event EventHandler ChildrenCreated;
	}
}
