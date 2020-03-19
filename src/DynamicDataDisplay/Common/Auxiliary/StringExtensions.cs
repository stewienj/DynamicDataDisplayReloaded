namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	public static class StringExtensions
	{
		public static string Format(this string formatString, object param)
		{
			return string.Format(formatString, param);
		}

		public static string Format(this string formatString, object param1, object param2)
		{
			return string.Format(formatString, param1, param2);
		}
	}
}
