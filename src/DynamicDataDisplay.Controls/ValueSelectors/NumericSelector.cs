using DynamicDataDisplay.Charts;

namespace DynamicDataDisplay.Controls
{
	/// <summary>
	/// Represents a control for precise selecting <see cref="T:System.Int32"/> values.
	/// </summary>
	public class NumericSelector : GenericValueSelector<double>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NumericSelector"/> class.
		/// </summary>
		public NumericSelector()
		{
			var axis = new HorizontalAxis();
			Children.Add(axis);
			ValueConversion = axis;
		}
	}
}
