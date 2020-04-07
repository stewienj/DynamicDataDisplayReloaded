namespace DynamicDataDisplay
{
	public static class DataDomains
	{
		private static readonly DataRect xPositive = DataRect.FromPoints(double.Epsilon, double.MinValue / 2, double.MaxValue, double.MaxValue / 2);
		public static DataRect XPositive
		{
			get { return xPositive; }
		}

		private static readonly DataRect yPositive = DataRect.FromPoints(double.MinValue / 2, double.Epsilon, double.MaxValue / 2, double.MaxValue);
		public static DataRect YPositive
		{
			get { return yPositive; }
		}

		private static readonly DataRect xyPositive = DataRect.FromPoints(double.Epsilon, double.Epsilon, double.MaxValue, double.MaxValue);
		public static DataRect XYPositive
		{
			get { return xyPositive; }
		}
	}
}
