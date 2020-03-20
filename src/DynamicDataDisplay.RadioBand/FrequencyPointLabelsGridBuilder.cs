using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DynamicDataDisplay.RadioBand.ConfigLoader;
using System.Windows.Media;

namespace DynamicDataDisplay.RadioBand
{
  public class FrequencyPointLabelsGridBuilder : FrequencyLabelsGridBuilder
  {
    private RadioBandPlotConfig _config;
    private FrequencyPointLabels _pointLabels;
    protected RadioBandTransform _transform;

    public FrequencyPointLabelsGridBuilder(RadioBandPlotConfig config, FrequencyPointLabels pointLabels)
    {
      _config = config;
      _pointLabels = pointLabels;
      _transform = new RadioBandTransform(config);
    }

    protected override IEnumerable<UIElement> CreateGridChildControls()
    {
      // Add the grid lines over the top of the colors
      // Add the grid labels
      return CreateLableBorders().Concat(CreateLabelTextBoxes());
    }

    /// <summary>
    /// So we do something tricky, each alternate column is zero width, then we place a
    /// text block inside it with negative left/right margins.
    /// </summary>
    /// <returns></returns>
    protected override IEnumerable<double> GetSizesForColumns()
    {
      if (_pointLabels.Count() == 0)
      {
        yield break;
      }

      double firstTick = _config.Ticks.First();
      double lastTick = _config.Ticks.Last();

      double startPos = _transform.FrequencyToViewport(firstTick);
      double widthsTotal = startPos;
      int skipCount = _pointLabels.First().Position <= firstTick ? 1 : 0;

      foreach (var pointLabel in _pointLabels.Skip(skipCount))
      {
        double width = _transform.FrequencyToViewport(pointLabel.Position) - widthsTotal;
        widthsTotal += width;
        yield return width;
        yield return 0;
      }

      // If the last tick is after the labels, then we need an extra column
      if (lastTick > _pointLabels.Last().Position)
      {
        double width = _transform.FrequencyToViewport(lastTick) - widthsTotal;
        widthsTotal += width;
        yield return width;
      }
    }

    protected override IEnumerable<double> GetSizesForRows()
    {
      yield return 1.0;
    }

    public override Grid BuildLabelsGridControl(int rowNo)
    {
      var pointLabelGrid = base.BuildLabelsGridControl(rowNo);

      // Add in the background brush
      double startPos = _transform.FrequencyToViewport(_config.Ticks.First());
      var backgroundGradientStops =  _pointLabels
        .Where(pl => pl.ColorStop.HasValue)
        .Select(pl => new GradientStop(pl.ColorStop.Value, _transform.FrequencyToViewport(pl.Position) - startPos));

      // Set the background gradient
      pointLabelGrid.Background = new LinearGradientBrush(new GradientStopCollection(backgroundGradientStops));

      return pointLabelGrid;

    }

    private IEnumerable<UIElement> CreateLableBorders()
    {
      if (_pointLabels.Count() == 0 || _pointLabels.LineMarkerThickness == 0 || _pointLabels.LineMarkerColor == Colors.Transparent)
      {
        yield break;
      }

      int columns = GetSizesForColumns().Count();

      // Odd Columns are zero width, this is where we place our border edges
      for (int i = 1; i < columns; i+=2)
      {
        var border = new Border();
        border.BorderThickness = new Thickness(0, 0, _pointLabels.LineMarkerThickness, 0);
        border.BorderBrush = new SolidColorBrush(_pointLabels.LineMarkerColor);
        border.Margin = new Thickness(-10, 0, 0, 0);
        border.SetValue(Grid.ColumnProperty, i);
        border.SetValue(Panel.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
        yield return border;
      }
    }


    private IEnumerable<UIElement> CreateLabelTextBoxes()
    {
      if (_pointLabels.Count() == 0)
      {
        yield break;
      }

      RadioBandTransform transform = new RadioBandTransform(_config);

      double firstTick = _config.Ticks.First();
      double lastTick = _config.Ticks.Last();

      double startPos = transform.FrequencyToViewport(firstTick);
      double widthsTotal = startPos;

      int skip = _pointLabels.First().Position <= firstTick ? 1 : 0;
      int skipEnd = lastTick <= _pointLabels.Last().Position ? 1 : 0;

      int columnNo = 0;
      if (skip == 1)
      {
        var textBlock = CreateTextBlock(columnNo, _pointLabels.First().LabelText);
        textBlock.SetValue(Grid.ColumnSpanProperty, 1);
        textBlock.TextAlignment = TextAlignment.Left;
        textBlock.HorizontalAlignment = HorizontalAlignment.Left;
        textBlock.Margin = new Thickness(0);
        yield return textBlock;
      }
      ++columnNo;
      foreach (var pointLabel in _pointLabels.Skip(skip).Take(_pointLabels.Count() - skip - skipEnd))
      {
        var textBlock = CreateTextBlock(columnNo, pointLabel.LabelText);
        yield return textBlock;
        columnNo+=2;
      }
      --columnNo;
      // If the last tick is after the labels, then we need an extra column
      if (skipEnd == 1)
      {
        var textBlock = CreateTextBlock(columnNo, _pointLabels.Last().LabelText);
        textBlock.SetValue(Grid.ColumnSpanProperty, 1);
        textBlock.TextAlignment = TextAlignment.Right;
        textBlock.HorizontalAlignment = HorizontalAlignment.Right;
        textBlock.Margin = new Thickness(0);
        yield return textBlock;
        ++columnNo;
      }

    }

    private TextBlock CreateTextBlock(int columnNo, string[] labels)
    {
      var textBlock = new TextBlock();
      textBlock.SetValue(Grid.ColumnProperty, columnNo);
      textBlock.TextAlignment = TextAlignment.Center;
      textBlock.VerticalAlignment = VerticalAlignment.Center;
      textBlock.HorizontalAlignment = HorizontalAlignment.Center;
      textBlock.Text = labels.Aggregate((a, b) => $"{a}\n{b}");
      textBlock.Margin = new Thickness(-30,0,-30,0);
      textBlock.Foreground = new SolidColorBrush(Colors.White);
      return textBlock;
    }

  }
}
