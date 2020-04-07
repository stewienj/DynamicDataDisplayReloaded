using DynamicDataDisplay.Common;
using System;
using System.Windows;
using System.Windows.Input;

namespace DynamicDataDisplay.Charts.Shapes
{

	/// <summary>
	/// Represents a simple draggable point with position bound to point in viewport coordinates, which allows to drag iself by mouse.
	/// </summary>
	public partial class DraggablePoint : PositionalViewportUIContainer
	{
		private Vector shiftInViewport;
		private bool ignoreNextMouseMove;

		/// <summary>
		/// Initializes a new instance of the <see cref="DraggablePoint"/> class.
		/// </summary>
		public DraggablePoint()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DraggablePoint"/> class.
		/// </summary>
		/// <param name="position">The position of DraggablePoint.</param>
		public DraggablePoint(Point position) : this() { Position = position; }

		protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			if (Plotter == null)
				return;

			var requiredMousePos = Position.DataToScreen(Plotter.Viewport.Transform);
			var actualMousePos = e.GetPosition(Plotter.ViewportPanel);
			var offset = requiredMousePos - actualMousePos;

			bool success = true;
			ignoreNextMouseMove = false;

			if (offset.LengthSquared > 0)
			{
				success = false;
				Win32Stuff.POINT p = new Win32Stuff.POINT();
				if (Win32Stuff.GetCursorPos(out p))
				{
					p.x = Convert.ToInt16(p.x + offset.X);
					p.y = Convert.ToInt16(p.y + offset.Y);
					if (Win32Stuff.SetCursorPos(p.x, p.y))
					{
						success = true;
						ignoreNextMouseMove = true;
					}
				}
			}
			if (!success)
			{
				Point dragStartInViewport = e.GetPosition(Plotter.ViewportPanel).ScreenToData(Plotter.Viewport.Transform);
				shiftInViewport = Position - dragStartInViewport;
			}
			else
			{
				shiftInViewport = new Vector(0, 0);
			}

			CaptureMouse();
			e.Handled = true;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (Plotter == null)
				return;

			if (Mouse.LeftButton == MouseButtonState.Pressed)
			{
				Point mouseInViewport = e.GetPosition(Plotter.ViewportPanel).ScreenToData(Plotter.Viewport.Transform);
				if (!ignoreNextMouseMove)
				{
					Position = mouseInViewport + shiftInViewport;
				}
			}
			else if (IsMouseCaptured)
			{
				ReleaseMouseCapture();
			}
			ignoreNextMouseMove = false;
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			ReleaseMouseCapture();
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			return base.ArrangeOverride(arrangeBounds);
		}
	}
}
