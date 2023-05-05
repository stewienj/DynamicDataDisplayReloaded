using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DynamicDataDisplay.Navigation
{
    public class MouseNavigationSelectionBase : NavigationBase
    {
        protected override void OnPlotterAttached(Plotter plotter)
        {
            base.OnPlotterAttached(plotter);

            Mouse.AddMouseDownHandler(Parent, OnMouseDown);
            Mouse.AddMouseMoveHandler(Parent, OnMouseMove);
            Mouse.AddMouseUpHandler(Parent, OnMouseUp);

            plotter.KeyDown += new KeyEventHandler(OnParentKeyDown);
        }

        protected override void OnPlotterDetaching(Plotter plotter)
        {
            TryStopSelection();

            Mouse.RemoveMouseDownHandler(Parent, OnMouseDown);
            Mouse.RemoveMouseMoveHandler(Parent, OnMouseMove);
            Mouse.RemoveMouseUpHandler(Parent, OnMouseUp);

            plotter.KeyDown -= new KeyEventHandler(OnParentKeyDown);
            
            base.OnPlotterDetaching(plotter);
        }

        protected virtual void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
        }

        protected virtual void OnMouseMove(object sender, MouseEventArgs e)
        {
        }

        protected virtual void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        protected virtual void OnParentKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Back)
            {
                if (TryStopSelection())
                {
                    e.Handled = true;
                }
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            TryStopSelection();
            base.OnLostFocus(e);
        }

        protected virtual bool TryStopSelection()
        {
            if (SelectionInProgress)
            {
                SelectionInProgress = false;
                ReleaseMouseCapture();
                return true;
            }
            else
            {
                return false;
            }
        }

        protected Point FirstDataPoint { get; set; }

        protected Point LastDataPoint { get; set; }

        private bool _selectionInProgress = false;
        protected bool SelectionInProgress
        {
            get => _selectionInProgress;
            set
            {
                if (_selectionInProgress != value)
                {
                    _selectionInProgress = value;
                    if (value)
                    {
                        SelectionStarted?.Execute(FirstDataPoint);
                    }
                    else
                    {
                        SelectionStopped?.Execute(LastDataPoint);
                    }
                }
            }
        }

        public ICommand SelectionStarted
        {
            get { return (ICommand)GetValue(SelectionStartedProperty); }
            set { SetValue(SelectionStartedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartedMoving.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionStartedProperty =
            DependencyProperty.Register("SelectionStarted", typeof(ICommand), typeof(MouseNavigationSelectionBase), new PropertyMetadata(null));

        public ICommand SelectionStopped
        {
            get { return (ICommand)GetValue(SelectionStoppedProperty); }
            set { SetValue(SelectionStoppedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartedMoving.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionStoppedProperty =
            DependencyProperty.Register("SelectionStopped", typeof(ICommand), typeof(MouseNavigationSelectionBase), new PropertyMetadata(null));

        public IList<Point> SelectedAreaPath
        {
            get { return (IList<Point>)GetValue(SelectedAreaPathProperty); }
            set { SetValue(SelectedAreaPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UserDrawnPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedAreaPathProperty =
            DependencyProperty.Register("SelectedAreaPath", typeof(IList<Point>), typeof(MouseNavigationSelectionBase), new PropertyMetadata(new List<Point>()));

    }
}
