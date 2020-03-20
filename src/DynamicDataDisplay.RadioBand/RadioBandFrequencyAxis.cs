using DynamicDataDisplay.RadioBand.ConfigLoader;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace DynamicDataDisplay.RadioBand
{
  public class RadioBandFrequencyAxis : RadioBandAxis
  {
    private List<RowDefinition> _addedRowDefinitions = new List<RowDefinition>();
    private List<Grid> _addedGrids = new List<Grid>();
    private RadioBandPlotConfig _config;
    private Grid _mainGrid;

    public RadioBandFrequencyAxis() : this(RadioBandPlotConfig.SpectrumDisplayDefault)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimeAxis"/> class.
    /// </summary>
    public RadioBandFrequencyAxis(RadioBandPlotConfig config) : base(AxisPlacement.Bottom)
    {
      _config = config;
    }

    protected override void OnPlotterAttached(Plotter2D plotter)
    {
      base.OnPlotterAttached(plotter);
      MainGrid = plotter.MainGrid as Grid;
    }

    protected override void OnPlotterDetaching(Plotter2D plotter)
    {
      base.OnPlotterDetaching(plotter);
      MainGrid = null;
    }



    public Grid MainGrid
    {
      get { return (Grid)GetValue(MainGridProperty); }
      set { SetValue(MainGridProperty, value); }
    }

    // Using a DependencyProperty as the backing store for MainGrid.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty MainGridProperty =
        DependencyProperty.Register("MainGrid", typeof(Grid), typeof(RadioBandFrequencyAxis), new PropertyMetadata(null, (s,e)=>
        {
          if (s is RadioBandFrequencyAxis axis)
          {
            if (e.NewValue is Grid mainGrid)
            {
              axis._mainGrid = mainGrid;
              axis.PopulateGridFromConfig(axis._config);
            }
            else
            {
              axis.PopulateGridFromConfig(null);
              axis._mainGrid = null;
            }
          }
        }));

    /// <summary>
    /// It all got a bit complicated with trying to get the labels on the left, and the screlling labels
    /// under the plot. Now we create extra rows in the top level grid in the plotter, and put stuff in there.
    /// </summary>
    /// <param name="config"></param>
    private void PopulateGridFromConfig(RadioBandPlotConfig config)
    {
      _scalableGrid.Children.Clear();
      foreach(var rowDefinition in _addedRowDefinitions)
      {
        _mainGrid.RowDefinitions.Remove(rowDefinition);
      }
      _addedRowDefinitions.Clear();
      foreach (var addedGrid in _addedGrids)
      {
        _mainGrid.Children.Remove(addedGrid);
      }
      _addedGrids.Clear();

      if (config == null || !config.Ticks.Any())
      {
        return;
      }

      foreach (var (labels, rowNo) in config.FrequencyLabels.Select((labels, rowNo) => (labels, rowNo)))
      {
        (var labelGrid, var scrollableGrid) = CreateAndAddLabelAndFrequencyGrid();
        switch (labels)
        {
          case FrequencyRangeLabels rangeLabels:
            var rangeGridBuilder = new FrequencyRangeLabelsGridBuilder(config, rangeLabels);
            var rangeLabelGrid = rangeGridBuilder.BuildLabelsGridControl(rowNo);
            scrollableGrid.Children.Add(rangeLabelGrid);

            break;
          case FrequencyPointLabels pointLabels:
            var pointGridBuilder = new FrequencyPointLabelsGridBuilder(config, pointLabels);
            var pointLabelGrid = pointGridBuilder.BuildLabelsGridControl(rowNo);
            scrollableGrid.Children.Add(pointLabelGrid);
            break;
        }

        // Border top lines
        var topThickness = rowNo == 0 ? 0 : 1;

        Border labelBorder = new Border();
        labelBorder.BorderThickness = new Thickness(0, topThickness, 0, 0);
        labelBorder.BorderBrush = new SolidColorBrush(Colors.Black);
        labelBorder.SetValue(Panel.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);

        TextBlock textBlock = new TextBlock();
        textBlock.VerticalAlignment = VerticalAlignment.Center;
        textBlock.TextAlignment = TextAlignment.Center;
        textBlock.Margin = new Thickness(4, 1, 4, 1);
        textBlock.Text = labels.Description.Aggregate((a, b) => $"{a}\n{b}");
        labelBorder.Child = textBlock;
        labelGrid.Children.Add(labelBorder);

        // Add a border over the top
        Border border = new Border();
        border.BorderThickness = new Thickness(1, topThickness, 1, 0);
        border.BorderBrush = new SolidColorBrush(Colors.Black);
        border.SetValue(Panel.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
        scrollableGrid.Children.Add(border);
      }

    }

    private (Grid,Grid) CreateAndAddLabelAndFrequencyGrid()
    {
      int rowNo = _mainGrid.RowDefinitions.Count;

      var rowDefinition = new RowDefinition { Height = new GridLength(0, GridUnitType.Auto) };
      _addedRowDefinitions.Add(rowDefinition);
      _mainGrid.RowDefinitions.Add(rowDefinition);

      Grid gridOuter = new Grid();
      gridOuter.VerticalAlignment = VerticalAlignment.Stretch;
      gridOuter.SetValue(Grid.RowProperty,rowNo);
      gridOuter.SetValue(Grid.ColumnProperty, 1);

      Grid scrollableGrid = new Grid();
      scrollableGrid.VerticalAlignment = VerticalAlignment.Stretch;
      Binding binding = new Binding("Margin");
      binding.Source = _scalableGrid;
      binding.Mode = BindingMode.OneWay;
      scrollableGrid.SetBinding(Grid.MarginProperty, binding);

      gridOuter.ClipToBounds = true;
      gridOuter.Children.Add(scrollableGrid);

      _addedGrids.Add(gridOuter);
      _mainGrid.Children.Add(gridOuter);

      Grid labelGrid = new Grid();
      labelGrid.Height = double.NaN; // Set Height to Auto
      labelGrid.SetValue(Grid.RowProperty, rowNo);
      labelGrid.SetValue(Grid.ColumnProperty, 0);
      _addedGrids.Add(labelGrid);
      _mainGrid.Children.Add(labelGrid);

      return (labelGrid,scrollableGrid);
    }

  }
}
