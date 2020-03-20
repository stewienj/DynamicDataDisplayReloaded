using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace DynamicDataDisplay.RadioBand
{
  public class RadioBandGroupAxis : RadioBandAxis
  {
    private List<object> _groups = new List<object>();

    public RadioBandGroupAxis() : base(AxisPlacement.Left)
    {
    }

    internal void UpdateGroups(IEnumerable<object> groups)
    {
      // Check if update required
      List<object> newGroups = groups.ToList();
      if (newGroups.Count == _groups.Count)
      {
        if (newGroups.Zip(_groups, (a,b)=>a==b).All(x=>x))
        {
          // No update required
          return;
        }
      }

      _groups = newGroups;
      Dispatcher.Invoke(() =>
      {
        _scalableGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
        _scalableGrid.VerticalAlignment = VerticalAlignment.Stretch;
        _scalableGrid.Background = new SolidColorBrush(Color.FromRgb(240,248,255));
        _scalableGrid.Children.Clear();
        _scalableGrid.RowDefinitions.Clear();
        _scalableGrid.RowDefinitions.AddMany(_groups.Select(g => new RowDefinition()));
        for(int i=0; i < newGroups.Count; ++i)
        {
          RadioBandGroupLabel groupLabel = new RadioBandGroupLabel();
          groupLabel.DataContext = _groups[i];
          groupLabel.BorderThickness = new Thickness(0, 1, 0, i == (newGroups.Count-1) ? 1 : 0);
          groupLabel.SetValue(Grid.RowProperty, i);
          if (_groups[i] != null)
          {
            groupLabel.Text = _groups[i].ToString();
          }
          _scalableGrid.Children.Add(groupLabel);
        }
      });
    }


    /// <summary>
    /// The RadioBandChartPlotter has some labels in the bottom left corner. These make the left axis wider,
    /// unfortunately the normal left axis panel is a stack panel, which means this control won't fill the
    /// axis, so we have to parent this control with the main plotter grid instead.
    /// </summary>
    /// <param name="plotter"></param>
    protected override void OnPlotterAttached(Plotter2D plotter)
    {
      plotter.Viewport.PropertyChanged += OnViewportPropertyChanged;
      SetValue(Grid.ColumnProperty, 0);
      SetValue(Grid.RowProperty, 1);

      plotter.MainGrid.Children.Add(this);

      TicksProvider = ((HorizontalAxis)(((RadioBandChartPlotter)plotter).MainHorizontalAxis)).TicksProvider;

      UpdateAxisControl(plotter.Viewport);
    }

    protected override void OnPlotterDetaching(Plotter2D plotter)
    {
      if (plotter == null)
        return;

      plotter.MainGrid.Children.Remove(this);

      plotter.Viewport.PropertyChanged -= OnViewportPropertyChanged;
    }

  }
}
