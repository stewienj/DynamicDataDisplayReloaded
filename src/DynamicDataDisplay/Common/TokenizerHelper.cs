using System;
using System.Globalization;

namespace Microsoft.Research.DynamicDataDisplay.Common
{
	public static class TokenizerHelper
	{
		public static char GetNumericListSeparator(IFormatProvider provider)
		{
			char separator = ',';

			NumberFormatInfo numberInfo = NumberFormatInfo.GetInstance(provider);
			if (numberInfo.NumberDecimalSeparator.Length > 0 && separator == numberInfo.NumberDecimalSeparator[0])
			{
				separator = ';';
			}

			return separator;
		}
	}
}
