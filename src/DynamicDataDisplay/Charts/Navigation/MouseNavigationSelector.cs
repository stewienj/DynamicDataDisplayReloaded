using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

#nullable enable

namespace DynamicDataDisplay.Charts.Navigation
{
    /// <summary>
    /// This allows for the mouse mode to be selected from a bound view model
    /// </summary>
    [ContentProperty("Children")]
    public class MouseNavigationSelector : FrameworkElement, IPlotterElement
    {
        private Plotter2D? _plotter;
        private IPlotterElement? _lastSelectedElement = null;

        public void OnPlotterAttached(Plotter plotter)
        {
            _plotter = (Plotter2D)plotter;
            if (_plotter is ChartPlotter chartPlotter)
            {
                _plotter.Children.Remove(chartPlotter.MouseNavigation);
            }
            DataContext = plotter.DataContext;
            if (_lastSelectedElement != null)
            {
                plotter.Children.Add(_lastSelectedElement);
            }
        }

        public void OnPlotterDetaching(Plotter plotter)
        {
            if (_lastSelectedElement != null)
            {
                plotter.Children.Remove(_lastSelectedElement);
                _lastSelectedElement = null;
            }
            DataContext = null;
            _plotter = null;
        }

        public Plotter? Plotter => _plotter;

        public ObservableCollection<IPlotterElement> Children { get; } = new ObservableCollection<IPlotterElement>();

        public object SelectedKey
        {
            get { return (object)GetValue(SelectedKeyProperty); }
            set { SetValue(SelectedKeyProperty, value); }
        }

        /// <summary>
        /// The selected key that determines which one of the child naviation elements is in use
        /// </summary>
        public static readonly DependencyProperty SelectedKeyProperty =
            DependencyProperty.Register("SelectedKey", typeof(object), typeof(MouseNavigationSelector), new PropertyMetadata(null, (s, e) =>
            {
                try
                {
                    if (s is MouseNavigationSelector control)
                    {
                        IPlotterElement? selectedElement = null;

                        // Find if a valid child element has been selected

                        if (e.NewValue != null)
                        {
                            // Find the matching element
                            selectedElement = control
                          .Children
                          .OfType<DependencyObject>()
                          .Where(child => GetKey(child)?.ToString() == e.NewValue.ToString())
                          .FirstOrDefault() as IPlotterElement;
                        }

                        // Check that the selected element has actually changed, if so then
                        // remove the old element, and add the newly selected element to
                        // the plotter children

                        if (selectedElement != control._lastSelectedElement)
                        {
                            if (control._plotter != null && control._lastSelectedElement != null)
                            {
                                control._plotter.Children.Remove(control._lastSelectedElement);
                            }

                            control.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                if (control._plotter != null && selectedElement != null)
                                {
                                    control._plotter.Children.Add(selectedElement);
                                }
                                control._lastSelectedElement = selectedElement;
                            }));
                        }
                    }
                }
                catch (Exception) { }
            }));

        public static object GetKey(DependencyObject obj)
        {
            return (object)obj.GetValue(KeyProperty);
        }

        public static void SetKey(DependencyObject obj, object value)
        {
            obj.SetValue(KeyProperty, value);
        }
        
        /// <summary>
        /// An attached property for the child elements of this that identify them by a key
        /// </summary>
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.RegisterAttached("Key", typeof(object), typeof(MouseNavigationSelector), new PropertyMetadata(null));
    }
}
