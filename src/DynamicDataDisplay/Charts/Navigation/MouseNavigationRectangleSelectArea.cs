using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace DynamicDataDisplay.Navigation
{
    public class MouseNavigationRectangleSelectArea : MouseNavigationSelectionBase
    {
        protected override void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && Keyboard.Modifiers == ModifierKeys.None)
            {
                var mousePoint = e.GetPosition(this);
                LastDataPoint = Viewport.Transform.ScreenToData(mousePoint);

                SelectionInProgress = true;

                SelectedAreaPath?.Clear();
                // Add 4 corners to define rectangle
                SelectedAreaPath?.Add(LastDataPoint);
                SelectedAreaPath?.Add(LastDataPoint);
                SelectedAreaPath?.Add(LastDataPoint);
                SelectedAreaPath?.Add(LastDataPoint);

                // not capturing mouse because this made some tools like PointSelector not
                // receive MouseUp events on markers;
                // Mouse will be captured later, in the first MouseMove handler call.
                // CaptureMouse();
                // e.Handled = true;
            }

            if (!Plotter.IsFocused)
            {
                Plotter.Focus();

                // this is done to prevent other tools like PointSelector from getting mouse click event when clicking on plotter
                // to activate window it's contained within
                e.Handled = true;
            }
        }

        protected override void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (SelectionInProgress && e.ChangedButton == MouseButton.Left)
            {
                if (!Plotter.IsFocused)
                {
                    Plotter2D.Focus();
                }

                TryStopSelection();
            }
        }

        protected override void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!SelectionInProgress)
                return;

            // dragging
            if (SelectionInProgress && e.LeftButton == MouseButtonState.Pressed)
            {
                if (!IsMouseCaptured)
                {
                    CaptureMouse();
                }

                var mousePoint = e.GetPosition(this);
                Point endPoint = Viewport.Transform.ScreenToData(mousePoint);
                Vector shift = LastDataPoint - endPoint;

                // preventing unnecessary changes, if actually visible hasn't change.
                if (shift.X != 0 || shift.Y != 0)
                {
                    LastDataPoint = endPoint;
                    var firstPoint = SelectedAreaPath.First();
                    SelectedAreaPath[1] = new Point(LastDataPoint.X, firstPoint.Y);
                    SelectedAreaPath[2] = LastDataPoint;
                    SelectedAreaPath[3] = new Point(firstPoint.X, LastDataPoint.Y);
                }

                e.Handled = true;
            }
        }
    }
}
