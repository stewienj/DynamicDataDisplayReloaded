using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.ViewportRestrictions
{
	/// <summary>
	/// Represents a viewport restriction which allows the target plotter's Vertical axis
	/// to follow the Vertical axis (zoom/pan) of the source plotter.
	/// Add to the plotter.Viewport.Restrictions collection.
	/// </summary>
	public class FollowVerticalAxisRestriction : FollowHorizontalAxisRestriction
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FollowWidthRestriction"/> class.
		/// </summary>
		public FollowVerticalAxisRestriction(Plotter2D sourcePlotter) : base(sourcePlotter) { }

		/// <summary>
		/// Applies the restriction.
		/// </summary>
		/// <param name="previousDataRect">Previous data rectangle.</param>
		/// <param name="proposedDataRect">Proposed data rectangle.</param>
		/// <param name="viewport">The viewport, to which current restriction is being applied.</param>
		/// <returns>New changed visible rectangle.</returns>
		public override DataRect Apply(DataRect previousDataRect, DataRect proposedDataRect, Viewport2D viewport)
		{
			if (proposedDataRect.IsEmpty)
				return proposedDataRect;

			Rect visible = new Rect(proposedDataRect.XMin, DataRect.YMin, proposedDataRect.Width, DataRect.Height);

			return visible;
		}
	}
}
