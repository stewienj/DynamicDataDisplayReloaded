using DynamicDataDisplay.Charts.Axes.Numeric;

namespace DynamicDataDisplay.Charts
{
	/// <summary>
	/// Represents a numeric axis with values of <see cref="double"/> type.
	/// </summary>
	public class NumericAxis : AxisBase<double>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NumericAxis"/> class.
		/// </summary>
		public NumericAxis()
			: base(new NumericAxisControl(),
				d => d,
				d => d)
		{
		}

		/// <summary>
		/// Sets conversions of axis - functions used to convert values of axis type to and from double values of viewport.
		/// Sets both ConvertToDouble and ConvertFromDouble properties.
		/// </summary>
		/// <param name="min">The minimal viewport value.</param>
		/// <param name="minValue">The value of axis type, corresponding to minimal viewport value.</param>
		/// <param name="max">The maximal viewport value.</param>
		/// <param name="maxValue">The value of axis type, corresponding to maximal viewport value.</param>
		public override void SetConversion(double min, double minValue, double max, double maxValue)
		{
			var conversion = new NumericConversion(min, minValue, max, maxValue);

			this.ConvertFromDouble = conversion.FromDouble;
			this.ConvertToDouble = conversion.ToDouble;
		}
	}
}
