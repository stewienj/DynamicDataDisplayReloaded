using System;

namespace Microsoft.Research.DynamicDataDisplay.ViewportRestrictions
{
	/// <summary>
	/// Represents a restriction, which limits the maximal size of <see cref="Viewport"/>'s Visible property.
	/// </summary>
	public class MaximalRectRestriction : ViewportRestriction
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MaximalRectRestriction"/> class.
		/// </summary>
		public MaximalRectRestriction() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="MaximalRectRestriction"/> class with the given maximal size of Viewport's Visible.
		/// </summary>
		/// <param name="maxRect">Maximal size of Viewport's Visible.</param>
		public MaximalRectRestriction(DataRect maxRect)
		{
			MaxRect = maxRect;
		}

		protected DataRect internalMaxRect = new DataRect(-500, -500, 1000, 1000);

		/// <summary>
		/// Gets or sets the maximal size of Viewport's Visible.
		/// The default value is 1000.0.
		/// </summary>
		/// <value>The size of the max.</value>
		public DataRect MaxRect
		{
			get { return internalMaxRect; }
			set
			{
				if (internalMaxRect != value)
				{
					internalMaxRect = value;
					RaiseChanged();
				}
			}
		}

		/// <summary>
		/// Applies the specified old data rect.
		/// </summary>
		/// <param name="oldDataRect">The old data rect.</param>
		/// <param name="newDataRect">The new data rect.</param>
		/// <param name="viewport">The viewport.</param>
		/// <returns></returns>
		public override DataRect Apply(DataRect oldDataRect, DataRect newDataRect, Viewport2D viewport)
		{
			if (this.internalMaxRect.Width <= 0 || this.internalMaxRect.Height <= 0)
			{
				return newDataRect;
			}
			// Need to do a scale-xy and offset-x and offset-y to bring the newDataRect within the bounds

			DataRect returnRect = newDataRect;
			DataRect maxRect = internalMaxRect;

			// Restrict Zoom X/Y
			if (maxRect.Width < newDataRect.Width && maxRect.Height < newDataRect.Height)
			{

				double scalerX = maxRect.Width / newDataRect.Width;
				double scalerY = maxRect.Height / newDataRect.Height;
				double scaler = Math.Max(scalerX, scalerY);

				returnRect.Width *= scaler;
				returnRect.Height *= scaler;

				returnRect.CenterPoint = oldDataRect.CenterPoint;
			}

			// Restrict Panning X
			if (returnRect.Width >= maxRect.Width)
			{
				returnRect.XMax = Math.Max(maxRect.XMax, returnRect.XMax);
				returnRect.XMin = Math.Min(maxRect.XMin, returnRect.XMin);
			}
			else
			{
				returnRect.XMax = Math.Min(maxRect.XMax, returnRect.XMax);
				returnRect.XMin = Math.Max(maxRect.XMin, returnRect.XMin);
			}

			// Restrict Panning Y
			if (returnRect.Height >= maxRect.Height)
			{
				returnRect.YMax = Math.Max(maxRect.YMax, returnRect.YMax);
				returnRect.YMin = Math.Min(maxRect.YMin, returnRect.YMin);
			}
			else
			{
				returnRect.YMax = Math.Min(maxRect.YMax, returnRect.YMax);
				returnRect.YMin = Math.Max(maxRect.YMin, returnRect.YMin);
			}

			return returnRect;
		}
	}
}
