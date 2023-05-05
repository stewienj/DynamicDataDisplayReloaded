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
    public class MouseNavigationPolygonSelectArea : NavigationBase
    {
        private bool _isDrawing = false;
        private Point _lastDrawingDataPoint;

        protected override void OnPlotterAttached(Plotter plotter)
        {
            base.OnPlotterAttached(plotter);

            Mouse.AddMouseDownHandler(Parent, OnMouseDown);
            Mouse.AddMouseMoveHandler(Parent, OnMouseMove);
            Mouse.AddMouseUpHandler(Parent, OnMouseUp);
            Mouse.AddMouseWheelHandler(Parent, OnMouseWheel);

            plotter.KeyDown += new KeyEventHandler(OnParentKeyDown);
        }

        protected override void OnPlotterDetaching(Plotter plotter)
        {
            plotter.KeyDown -= new KeyEventHandler(OnParentKeyDown);

            Mouse.RemoveMouseDownHandler(Parent, OnMouseDown);
            Mouse.RemoveMouseMoveHandler(Parent, OnMouseMove);
            Mouse.RemoveMouseUpHandler(Parent, OnMouseUp);
            Mouse.RemoveMouseWheelHandler(Parent, OnMouseWheel);

            if (CheckIsDrawing())
            {
                _isDrawing = false;
                SelectedAreaPath.RemoveAt(SelectedAreaPath.Count - 1);
            }

            base.OnPlotterDetaching(plotter);
        }

        private bool CheckIsDrawing()
        {
            if (_isDrawing && SelectedAreaPath.Count < 2)
            {
                SelectedAreaPath.Clear();
                _isDrawing = false;
            }

            return _isDrawing;
        }

        private void OnParentKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Back)
            {
                if (CheckIsDrawing())
                {
                    _isDrawing = false;
                    SelectedAreaPath.RemoveAt(SelectedAreaPath.Count - 1);
                    e.Handled = true;
                }
            }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && Keyboard.Modifiers == ModifierKeys.None)
            {
                var mousePoint = e.GetPosition(this);
                _lastDrawingDataPoint = Viewport.Transform.ScreenToData(mousePoint);
                if (!CheckIsDrawing())
                {
                    _isDrawing = true;

                    SelectedAreaPath?.Clear();
                    SelectedAreaPath?.Add(_lastDrawingDataPoint);
                    SelectedAreaPath?.Add(_lastDrawingDataPoint);

                    // not capturing mouse because this made some tools like PointSelector not
                    // receive MouseUp events on markers;
                    // Mouse will be captured later, in the first MouseMove handler call.
                    // CaptureMouse();
                    // e.Handled = true;
                }
                else
                {
                    SelectedAreaPath[SelectedAreaPath.Count - 1] = _lastDrawingDataPoint;
                    SelectedAreaPath?.Add(_lastDrawingDataPoint);
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

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!CheckIsDrawing())
                return;

            var mousePoint = e.GetPosition(this);
            Point endPoint = Viewport.Transform.ScreenToData(mousePoint);
            Vector shift = _lastDrawingDataPoint - endPoint;

            // preventing unnecessary changes, if actually visible hasn't change.
            if (shift.X != 0 || shift.Y != 0)
            {
                _lastDrawingDataPoint = endPoint;
                SelectedAreaPath[SelectedAreaPath.Count - 1] = _lastDrawingDataPoint;
            }

            e.Handled = true;
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (CheckIsDrawing())
            {
                _isDrawing = false;
            }
            base.OnLostFocus(e);
        }

        public IList<Point> SelectedAreaPath
        {
            get { return (IList<Point>)GetValue(SelectedAreaPathProperty); }
            set { SetValue(SelectedAreaPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UserDrawnPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedAreaPathProperty =
            DependencyProperty.Register("SelectedAreaPath", typeof(IList<Point>), typeof(MouseNavigationPolygonSelectArea), new PropertyMetadata(new List<Point>()));
    }
}
