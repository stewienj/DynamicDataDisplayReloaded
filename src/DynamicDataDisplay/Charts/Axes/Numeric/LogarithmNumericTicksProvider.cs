using DynamicDataDisplay.Common.Auxiliary;
using System;

namespace DynamicDataDisplay.Charts.Axes.Numeric
{
	/// <summary>
	/// Represents a ticks provider for logarithmically transfomed axis - returns ticks which are a power of specified logarithm base.
	/// </summary>
	public class LogarithmNumericTicksProvider : ITicksProvider<double>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LogarithmNumericTicksProvider"/> class.
		/// </summary>
		public LogarithmNumericTicksProvider()
		{
			minorProvider = new MinorNumericTicksProvider(this);
			minorProvider.Changed += ticksProvider_Changed;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LogarithmNumericTicksProvider"/> class.
		/// </summary>
		/// <param name="logarithmBase">The logarithm base.</param>
		public LogarithmNumericTicksProvider(double logarithmBase)
				: this()
		{
			LogarithmBase = logarithmBase;
		}

		private void ticksProvider_Changed(object sender, EventArgs e)
		{
			Changed.Raise(this);
		}

		private double logarithmBase = 10;
		public double LogarithmBase
		{
			get { return logarithmBase; }
			set
			{
				if (value <= 0)
					throw new ArgumentOutOfRangeException(Strings.Exceptions.LogarithmBaseShouldBePositive);

				logarithmBase = value;
			}
		}

		private double LogByBase(double d)
		{
			return Math.Log10(d) / Math.Log10(logarithmBase);
		}

		#region ITicksProvider<double> Members

		private double[] ticks;
		public ITicksInfo<double> GetTicks(Range<double> range, int ticksCount)
		{
			double min = LogByBase(range.Min);
			double max = LogByBase(range.Max);

			double minDown = Math.Floor(min);
			double maxUp = Math.Ceiling(max);

			double logLength = LogByBase(range.GetLength());

			// JCS TODO Create a range that hold the number of tick counts. The range created is always
			// powers of logBase,could pad it out
			ticks = CreateTicks(range);//.Concat(new double[] { 0.02, 0.04, 0.06, 0.08, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9 }).OrderBy(x => x).ToArray();

			int log = RoundingHelper.GetDifferenceLog(range.Min, range.Max);
			TicksInfo<double> result = new TicksInfo<double> { Ticks = ticks, TickSizes = ArrayExtensions.CreateArray(ticks.Length, 1.0), Info = log };
			return result;
		}

		private double[] CreateTicks(Range<double> range)
		{
			double min = LogByBase(range.Min);
			double max = LogByBase(range.Max);

			double minDown = Math.Floor(min);
			double maxUp = Math.Ceiling(max);

			int intStart = (int)Math.Floor(minDown);
			int count = (int)(Math.Min(256, maxUp - minDown + 1));

			var ticks = new double[count];
			for (int i = 0; i < count; i++)
			{
				ticks[i] = intStart + i;
			}

			for (int i = 0; i < ticks.Length; i++)
			{
				ticks[i] = Math.Pow(logarithmBase, ticks[i]);
			}

			return ticks;
		}

		public int DecreaseTickCount(int ticksCount)
		{
			return ticksCount;
		}

		public int IncreaseTickCount(int ticksCount)
		{
			return ticksCount;
		}

		private MinorNumericTicksProvider minorProvider;
		public ITicksProvider<double> MinorProvider
		{
			get
			{
				minorProvider.SetRanges(ArrayExtensions.GetPairs(ticks));
				return minorProvider;
			}
		}

		public ITicksProvider<double> MajorProvider
		{
			get { return null; }
		}

		public event EventHandler Changed;

		#endregion
	}
}
