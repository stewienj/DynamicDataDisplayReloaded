using DynamicDataDisplay.RadioBand.ConfigLoader;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System;

namespace DynamicDataDisplay.RadioBand
{
  public class RadioBandFrequencyTicksProvider : ITicksProvider<double>
  {
    private double[] _ticks;

    public RadioBandFrequencyTicksProvider(RadioBandPlotConfig config)
    {
      _ticks = config.Ticks;
      _minorProvider = new MinorNumericTicksProvider(this);
      _minorProvider.Changed += ticksProvider_Changed;
    }

    private void ticksProvider_Changed(object sender, EventArgs e)
    {
      Changed.Raise(this);
    }

    public ITicksProvider<double> MajorProvider
    {
      get
      {
        return null;
      }
    }

    private MinorNumericTicksProvider _minorProvider;
    public ITicksProvider<double> MinorProvider
    {
      get
      {
        _minorProvider.SetRanges(_ticks.GetPairs());
        return _minorProvider;
      }
    }

    public event EventHandler Changed;

    public int DecreaseTickCount(int ticksCount)
    {
      return _ticks.Length;
    }

    public ITicksInfo<double> GetTicks(Range<double> range, int ticksCount)
    {
      // Tried using a subset, but using the whole lot gives consistent shading on the axis grid, otherwise it jumps around
      var ticks = _ticks;
      int log = 0;
      TicksInfo<double> result = new TicksInfo<double> { Ticks = ticks, TickSizes = ArrayExtensions.CreateArray(ticks.Length, 1.0), Info = log };
      return result;
    }

    public int IncreaseTickCount(int ticksCount)
    {
      return _ticks.Length;
    }
  }
}
