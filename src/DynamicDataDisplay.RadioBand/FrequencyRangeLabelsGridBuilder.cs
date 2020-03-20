using DynamicDataDisplay.RadioBand.ConfigLoader;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DynamicDataDisplay.RadioBand
{
  public class FrequencyRangeLabelsGridBuilder : FrequencyLabelsGridBuilder
  {
    public class RangeAndLabels
    {
      public Range<double> ColumnRange { get; set; }
      public List<RangeLabel> Labels { get; set; } = new List<RangeLabel>();

      public int ColumnIndex {get; set;}

      public bool IsRowEmpty(int rowNo)
      {
        while (Labels.Count <= rowNo)
        {
          Labels.Add(null);
        }
        return Labels[rowNo] == null;
      }
    }

    public class LabelAndGridCoord
    {
      public RangeLabel Label { get; set; }
      public int ColumnNo { get; set; }
      public int ColumnSpan { get; set; } = 1;
      public int RowNo { get; set; }
      public int RowSpan { get; set; } = 1;
    }

    public class GapFiller
    {
      public Color TopColor { get; set; } = Colors.Transparent;
      public Color BottomColor { get; set; } = Colors.Transparent;
      public int ColumnNo { get; set; }
      public int RowNo { get; set; }
      public int RowSpan { get; set; }
    }


    private SortedSet<double> _frequencyDividers = new SortedSet<double>();
    private List<RangeAndLabels> _columns = new List<RangeAndLabels>();
    private RadioBandPlotConfig _config = null;
    private FrequencyLabelSet<RangeLabel> _rangeLabels = null;
    private List<LabelAndGridCoord> _labelsAndGridCoords = new List<LabelAndGridCoord>();
    private List<GapFiller> _gapFillers = new List<GapFiller>();
    private RadioBandTransform _transform;

    public FrequencyRangeLabelsGridBuilder(RadioBandPlotConfig config, FrequencyLabelSet<RangeLabel> rangeLabels)
    {
      _config = config;
      _rangeLabels = rangeLabels;
      _transform = new RadioBandTransform(config);

      BuildFrequencyDividers();
      BuildColumns();
      MatchRangeLabelsToColumns();
      ExpandLabelsAcrossRows();
      FillInTheGaps();
    }

    protected override IEnumerable<double> GetSizesForColumns()
    {
      return _columns
        .Select(c => _transform.FrequencyToViewport(c.ColumnRange.Max) - _transform.FrequencyToViewport(c.ColumnRange.Min));
    }

    protected override IEnumerable<double> GetSizesForRows()
    {
      return Enumerable.Range(0, _columns.Max(c => c.Labels.Count)).Select(l => 1.0);
    }

    protected override IEnumerable<UIElement> CreateGridChildControls()
    {
      return CreateGridLabels().Concat(CreateGapFillerControls());
    }


    /// <summary>
    /// Go over all the labels and set their Row Span, possibly repositioning which row they are in
    /// </summary>
    private void ExpandLabelsAcrossRows()
    {
      int rowCount = _columns.Max(c => c.Labels.Count);

      // Each of these labels will currently have a rowspan of 1
      foreach(LabelAndGridCoord labelAndGridCoord in _labelsAndGridCoords)
      {
        for(int rowNo=labelAndGridCoord.RowNo+1; rowNo<rowCount; ++rowNo)
        {
          var columnRange = Enumerable.Range(labelAndGridCoord.ColumnNo, labelAndGridCoord.ColumnSpan);
          if(columnRange.All(cr=>_columns[cr].IsRowEmpty(rowNo)))
          {
            labelAndGridCoord.RowSpan += 1;
            foreach(int columNo in columnRange)
            {
              _columns[columNo].Labels[rowNo] = labelAndGridCoord.Label;
            }
          }
          else
          {
            break;
          }
        }
      }
    }

    private void FillInTheGaps()
    {
      _gapFillers.Clear();
      foreach (var column in _columns)
      {
        // Variables used to build gap fillers
        int? gapStart = null;
        Color? topColor = null;
        Color? bottomColor = null;
        int rowNo = 0;

        // Factory to create gap fillers from the above variables
        Func<GapFiller> newGapFiller = () => new GapFiller
        {
          TopColor = topColor ?? bottomColor ?? Colors.Transparent,
          BottomColor = bottomColor ?? topColor ?? Colors.Transparent,
          ColumnNo = column.ColumnIndex,
          RowNo = gapStart.Value,
          RowSpan = rowNo - gapStart.Value
        };

        foreach (var row in column.Labels)
        {
          // Check if row gap is starting
          if (row == null && !gapStart.HasValue)
          {
            gapStart = rowNo;
          }
          // Check if row gap is ended
          else if (row != null && gapStart.HasValue)
          {
            // Build the gap filler
            bottomColor = row.Color;
            _gapFillers.Add(newGapFiller());

            // reset the variables in case there is more than one gap
            gapStart = null;
            topColor = null;
            bottomColor = null;
          }
          // Else just set the top color to the current row color
          else if (row!=null)
          {
            topColor = row.Color;
          }
          ++rowNo;
        }
        if (gapStart!=null)
        {
          _gapFillers.Add(newGapFiller());
        }
      }
    }

    /// <summary>
    /// Builds a list of frequencies that will define the columns
    /// </summary>
    private void BuildFrequencyDividers()
    {
      // first of all need to build a SortedSet of segments
      // Add the first tick, last tick, and all the frequency starts and ends of the range labels
      _frequencyDividers.Clear();
      if (_config.Ticks.Any())
      {
        _frequencyDividers.Add(_config.Ticks.First());
        _frequencyDividers.Add(_config.Ticks.Last());
      }
      foreach(var label in _rangeLabels)
      {
        _frequencyDividers.Add(label.Start);
        _frequencyDividers.Add(label.End);
      }
    }

    private void BuildColumns()
    {
      _columns = _frequencyDividers
        .GetPairs()
        .Select((range, i) => new RangeAndLabels { ColumnRange = range, ColumnIndex = i })
        .ToList();
    }

    private void MatchRangeLabelsToColumns()
    {
      _labelsAndGridCoords.Clear();
      foreach (var label in _rangeLabels)
      {
        var matchingColumns = GetMatchingColumns(label).ToList();
        _labelsAndGridCoords.Add(new LabelAndGridCoord
        {
          Label = label,
          RowNo = GetFirstEmptyRow(matchingColumns, label),
          ColumnNo = matchingColumns.Min(c => c.ColumnIndex),
          ColumnSpan = matchingColumns.Count
        });
      }
    }

    private IEnumerable<RangeAndLabels> GetMatchingColumns(RangeLabel label)
    {
      // Get all columns where either the label start or end is contained or the label completely overlaps column
      return _columns.Where(c => 
        (c.ColumnRange.Min <= label.Start && label.Start < c.ColumnRange.Max) 
        || (c.ColumnRange.Min < label.End && label.End <= c.ColumnRange.Max)
        || (label.Start <= c.ColumnRange.Min && c.ColumnRange.Max <= label.End));
    }

    private int GetFirstEmptyRow(List<RangeAndLabels> matchingColumns, RangeLabel label)
    {
      int rowNo = 0;
      while (!matchingColumns.All(c => c.IsRowEmpty(rowNo)))
        ++rowNo;

      if (label!=null)
      {
        foreach(var c in matchingColumns)
        {
          c.Labels[rowNo] = label;
        }
      }

      return rowNo;
    }


    private IEnumerable<UIElement> CreateGridLabels()
    {
      foreach (var label in _labelsAndGridCoords)
      {
        Border tbBorder = new Border();
        tbBorder.BorderThickness = new Thickness(0);
        tbBorder.SetValue(Grid.ColumnProperty, label.ColumnNo);
        tbBorder.SetValue(Grid.ColumnSpanProperty, label.ColumnSpan);
        tbBorder.SetValue(Grid.RowProperty, label.RowNo);
        tbBorder.SetValue(Grid.RowSpanProperty, label.RowSpan);
        tbBorder.Padding = new Thickness(0, 2, 0, 1);
        if (label.Label.Color.HasValue)
        {
          tbBorder.Background = new SolidColorBrush(label.Label.Color.Value);
        }
        TextBlock tb = new TextBlock();
        tb.Margin = new Thickness(1);
        tb.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
        tb.LineHeight = 10;
        tb.FontSize = 11;
        tb.Text = label.Label.LabelText.Aggregate((a, b) => $"{a}\n{b}");
        tb.TextAlignment = TextAlignment.Center;
        tb.VerticalAlignment = VerticalAlignment.Center;
        tbBorder.Child = tb;
        yield return tbBorder;
      }
    }

    private IEnumerable<UIElement> CreateGapFillerControls()
    {
      foreach (var gapFiller in _gapFillers)
      {
        Grid grid = new Grid();
        // Some tiny gaps between labels, this plugs the gaps
        grid.Margin = new Thickness(-0.25);
        grid.SetValue(Grid.ColumnProperty, gapFiller.ColumnNo);
        grid.SetValue(Grid.RowProperty, gapFiller.RowNo);
        grid.SetValue(Grid.RowSpanProperty, gapFiller.RowSpan);
        grid.Background = new LinearGradientBrush(gapFiller.TopColor, gapFiller.BottomColor, 90.0);
        yield return grid;
      }
    }


  }
}
