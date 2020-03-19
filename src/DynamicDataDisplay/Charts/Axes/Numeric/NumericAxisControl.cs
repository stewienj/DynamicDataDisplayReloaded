namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public class NumericAxisControl : AxisControl<double>
	{
		public NumericAxisControl()
		{
			LabelProvider = new ExponentialLabelProvider();
			TicksProvider = new NumericTicksProvider();
			ConvertToDouble = d => d;
			Range = new Range<double>(0, 10);
		}
	}
}
