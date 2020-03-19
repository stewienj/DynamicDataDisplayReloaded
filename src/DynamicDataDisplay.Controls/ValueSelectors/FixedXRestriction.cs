using Microsoft.Research.DynamicDataDisplay.ViewportRestrictions;

namespace Microsoft.Research.DynamicDataDisplay.Controls
{
	internal sealed class FixedXRestriction : ViewportRestriction
	{
		public override DataRect Apply(DataRect previousDataRect, DataRect proposedDataRect, Viewport2D viewport)
		{
			return DataRect.Create(proposedDataRect.XMin, 0, proposedDataRect.XMax, 1);
		}
	}
}
