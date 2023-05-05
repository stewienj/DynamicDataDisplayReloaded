using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace DynamicDataDisplay.Navigation
{
    public class MouseNavigationFreehandSelectArea : NavigationBase
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

            _isDrawing = false;

            base.OnPlotterDetaching(plotter);
        }

        private void OnParentKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Back)
            {
                if (_isDrawing)
                {
                    _isDrawing = false;
                    ReleaseMouseCapture();
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

                _isDrawing = true;

                SelectedAreaPath?.Clear();
                SelectedAreaPath?.Add(_lastDrawingDataPoint);

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

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDrawing && e.ChangedButton == MouseButton.Left)
            {
                _isDrawing = false;
                if (!Plotter.IsFocused)
                {
                    Plotter2D.Focus();
                }

                ReleaseMouseCapture();
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDrawing)
                return;

            // dragging
            if (_isDrawing && e.LeftButton == MouseButtonState.Pressed)
            {
                if (!IsMouseCaptured)
                {
                    CaptureMouse();
                }

                var mousePoint = e.GetPosition(this);
                Point endPoint = Viewport.Transform.ScreenToData(mousePoint);
                Vector shift = _lastDrawingDataPoint - endPoint;

                // preventing unnecessary changes, if actually visible hasn't change.
                if (shift.X != 0 || shift.Y != 0)
                {
                    _lastDrawingDataPoint = endPoint;
                    SelectedAreaPath?.Add(_lastDrawingDataPoint);
                }

                e.Handled = true;
            }
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (_isDrawing)
            {
                _isDrawing = false;
                ReleaseMouseCapture();
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
            DependencyProperty.Register("SelectedAreaPath", typeof(IList<Point>), typeof(MouseNavigationFreehandSelectArea), new PropertyMetadata(new List<Point>()));
    }
}
