using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace DynamicDataDisplay.Navigation
{
    public class MouseNavigationMoveSelectedArea : NavigationBase
    {
        private IList<Point> _originalPoints = new List<Point>();
        private Point _firstDataPoint;
        private Point _lastDataPoint;

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

            IsMoving = false;

            base.OnPlotterDetaching(plotter);
        }

        private void OnParentKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Back)
            {
                if (IsMoving)
                {
                    IsMoving = false;
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
                _firstDataPoint = Viewport.Transform.ScreenToData(mousePoint);
                _lastDataPoint = _firstDataPoint;

                IsMoving = true;

                _originalPoints = SelectedAreaPath.ToList();

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
            if (IsMoving && e.ChangedButton == MouseButton.Left)
            {
                IsMoving = false;
                if (!Plotter.IsFocused)
                {
                    Plotter2D.Focus();
                }

                ReleaseMouseCapture();
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!IsMoving)
                return;

            if (IsMoving && e.LeftButton == MouseButtonState.Pressed)
            {
                if (!IsMouseCaptured)
                {
                    CaptureMouse();
                }

                var mousePoint = e.GetPosition(this);
                _lastDataPoint = Viewport.Transform.ScreenToData(mousePoint);
                Vector shift = _lastDataPoint - _firstDataPoint;

                SelectedAreaPath.Clear();
                SelectedAreaPath.AddMany(_originalPoints.Select(p => p + shift));

                e.Handled = true;
            }
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (IsMoving)
            {
                IsMoving = false;
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
            DependencyProperty.Register("SelectedAreaPath", typeof(IList<Point>), typeof(MouseNavigationMoveSelectedArea), new PropertyMetadata(new List<Point>()));

        public bool IsMoving
        {
            get { return (bool)GetValue(IsMovingProperty); }
            set { SetValue(IsMovingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMoving.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMovingProperty =
            DependencyProperty.Register("IsMoving", typeof(bool), typeof(MouseNavigationMoveSelectedArea), new PropertyMetadata(false, (s, e) =>
            {
                if (s is MouseNavigationMoveSelectedArea nav)
                {
                    if (e.NewValue is bool newValue && e.OldValue is bool oldValue)
                    {
                        if (newValue && !oldValue)
                        {
                            nav.StartedMoving?.Execute(nav._firstDataPoint);
                        }
                        if (!newValue && oldValue)
                        {
                            nav.StoppedMoving?.Execute(nav._lastDataPoint);
                        }
                    }
                }
            }));


        public ICommand StartedMoving
        {
            get { return (ICommand)GetValue(StartedMovingProperty); }
            set { SetValue(StartedMovingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartedMoving.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartedMovingProperty =
            DependencyProperty.Register("StartedMoving", typeof(ICommand), typeof(MouseNavigationMoveSelectedArea), new PropertyMetadata(null));

        public ICommand StoppedMoving
        {
            get { return (ICommand)GetValue(StoppedMovingProperty); }
            set { SetValue(StoppedMovingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartedMoving.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StoppedMovingProperty =
            DependencyProperty.Register("StoppedMoving", typeof(ICommand), typeof(MouseNavigationMoveSelectedArea), new PropertyMetadata(null));


    }
}
