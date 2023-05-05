using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using DynamicDataDisplay.Navigation;

namespace DynamicDataDisplay.Navigation
{
    public class MouseNavigationPolygonSelectArea : MouseNavigationSelectionBase
    {

        protected override bool TryStopSelection()
        {
            if (SelectionInProgress)
            {
                if (SelectedAreaPath.Count < 2)
                {
                    SelectedAreaPath.Clear();
                }
                else
                {
                    SelectedAreaPath.RemoveAt(SelectedAreaPath.Count - 1);
                }
                base.TryStopSelection();
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && Keyboard.Modifiers == ModifierKeys.None)
            {
                var mousePoint = e.GetPosition(this);
                LastDataPoint = Viewport.Transform.ScreenToData(mousePoint);
                if (!SelectionInProgress)
                {
                    SelectionInProgress = true;

                    SelectedAreaPath?.Clear();
                    SelectedAreaPath?.Add(LastDataPoint);
                    SelectedAreaPath?.Add(LastDataPoint);

                    // not capturing mouse because this made some tools like PointSelector not
                    // receive MouseUp events on markers;
                    // Mouse will be captured later, in the first MouseMove handler call.
                    // CaptureMouse();
                    // e.Handled = true;
                }
                else
                {
                    SelectedAreaPath[SelectedAreaPath.Count - 1] = LastDataPoint;
                    SelectedAreaPath?.Add(LastDataPoint);
                    e.Handled = true;
                }
            }

            if (!Plotter.IsFocused)
            {
                Plotter.Focus();

                // this is done to prevent other tools like PointSelector from getting mouse click event when clicking on plotter
                // to activate window it's contained within
                e.Handled = true;
            }
        }

        protected override void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!SelectionInProgress)
                return;

            var mousePoint = e.GetPosition(this);
            Point endPoint = Viewport.Transform.ScreenToData(mousePoint);
            Vector shift = LastDataPoint - endPoint;

            // preventing unnecessary changes, if actually visible hasn't change.
            if (shift.X != 0 || shift.Y != 0)
            {
                LastDataPoint = endPoint;
                SelectedAreaPath[SelectedAreaPath.Count - 1] = LastDataPoint;
            }

            e.Handled = true;
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            TryStopSelection();
            base.OnLostFocus(e);
        }
    }
}
