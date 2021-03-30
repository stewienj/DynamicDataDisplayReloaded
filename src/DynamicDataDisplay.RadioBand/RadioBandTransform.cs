using DynamicDataDisplay.RadioBand.ConfigLoader;
using DynamicDataDisplay;
using System;
using System.Linq;
using System.Windows;

namespace DynamicDataDisplay.RadioBand
{
	public class RadioBandTransform : DataTransform
	{
		//  private static double[] _ticks = new[] {
		//      0.0, // at position 0
		//      0.1, // at 0.0625
		//      0.3, // at 0.125
		//      0.5, // at 0.1875
		//      1.0, // at 0.25
		//      2.0, // at 0.3125
		//      3.0, // at 0.375
		//      4.0, // at 0.4375
		//      6.0, // at 0.5
		//      8.0, // at 0.5625
		//      10.0,  // at 0.625
		//      20.0,  // at 0.6875
		//      30.0,  // at 0.75
		//      40.0,  // at 0.8125
		//      60.0,  // at 0.875
		//      100.0, // at 0.9375
		//      1000.0 // at 1.0
		//  }; // Length = 17

		private double[] _ticks;

        public RadioBandTransform(RadioBandPlotConfig config) => _ticks = config.Ticks;

        public double FrequencyToViewport(double frequency)
		{
			double viewportCoord = frequency;

			if (_ticks.Any())
			{
				viewportCoord = Math.Max(viewportCoord, _ticks.First());
				viewportCoord = Math.Min(viewportCoord, _ticks.Last());

				for (int i = 0; i < _ticks.Length - 1; ++i)
				{
					if (viewportCoord >= _ticks[i] && viewportCoord <= _ticks[i + 1])
					{
						viewportCoord = ((viewportCoord - _ticks[i]) / (_ticks[i + 1] - _ticks[i]) + i) / (_ticks.Length - 1);
						break;
					}
				}
			}
			return viewportCoord;
		}

        public double GroupNoToViewport(double groupNo) => groupNo / _groupCount;

        public double ViewportToGroupNo(double viewPort) => viewPort * _groupCount;

        public override Point DataToViewport(Point pt)
		{
			if (_ticks.Length == 0)
			{
				return pt;
			}
			return new Point(FrequencyToViewport(pt.X), GroupNoToViewport(pt.Y));
		}

		public double ViewportToFrequency(double viewportCoord)
		{
			double frequency = viewportCoord;
			frequency = Math.Max(frequency, 0);
			frequency = Math.Min(frequency, 1);

			if (_ticks.Any())
			{
				// Interpolate between tick points
				double indexReal = frequency * (_ticks.Length - 1);
				int index = (int)Math.Floor(indexReal);
				if (index < (_ticks.Length - 1))
				{
					frequency = _ticks[index] + (indexReal - index) * (_ticks[index + 1] - _ticks[index]);
				}
				else
				{
					frequency = _ticks.Last();
				}
			}
			return frequency;
		}

		public override Point ViewportToData(Point pt)
		{
			if (_ticks.Length == 0)
			{
				return pt;
			}
			return new Point(ViewportToFrequency(pt.X), ViewportToGroupNo(pt.Y));
		}

		private double _groupCount = 1.0;
		public double GroupCount
		{
			get => _groupCount;
			set => _groupCount = Math.Max(value, 1.0);
		}

		private DataRect _dataDomain = new DataRect(0, 0, double.MaxValue, double.MaxValue);
        public override DataRect DataDomain => _dataDomain;
    }
}
