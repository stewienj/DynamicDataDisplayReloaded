using System.ComponentModel;
using System.Windows;

namespace DynamicDataDisplay.ViewportRestrictions
{
	/// <summary>
	/// Represents a viewport restriction which allows the target plotter's Horizontal axis
	/// to follow the horizontal axis (zoom/pan) of the source plotter.
	/// Add to the plotter.Viewport.Restrictions collection.
	/// </summary>
	public class FollowHorizontalAxisRestriction : ViewportRestriction
	{
		/// <summary>
		/// The viewport rect of the source plotter
		/// </summary>
		private DataRect dataRect = new DataRect(0, 0, 1, 1);

		/// <summary>
		/// Initializes a new instance of the <see cref="FollowWidthRestriction"/> class.
		/// </summary>
		public FollowHorizontalAxisRestriction(Plotter2D sourcePlotter)
		{
			DataRect = sourcePlotter.Viewport.Visible;
			var descr = DependencyPropertyDescriptor.FromProperty(Viewport2D.VisibleProperty, typeof(Viewport2D));
			descr.AddValueChanged(sourcePlotter.Viewport, (s, e) =>
			{
				// Update when the source plotter changes
				DataRect = sourcePlotter.Viewport.Visible;
				RaiseChanged();
			});
		}

		protected DataRect DataRect { get => dataRect; set => dataRect = value; }

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

			Rect visible = new Rect(DataRect.XMin, proposedDataRect.YMin, DataRect.Width, proposedDataRect.Height);

			return visible;
		}
	}
}
