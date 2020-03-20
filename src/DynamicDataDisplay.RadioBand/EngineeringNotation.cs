using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDataDisplay.RadioBand
{
  public static class EngineeringNotation
  {
    /// <summary>
    /// Converts a string to Engineering Notation, used by the methods above
    /// </summary>
    /// <example>
    /// class Program
    /// {
    ///   static void Main(string[] args)
    ///   {
    ///     foreach(var a in SampleNumbers)
    ///     {
    ///        var leftPad = a < 0 ? "" : " ";
    ///        var rightPad = a < 0 ? "--> " : "-> ";
    ///        var original = $"{leftPad}{a.ToString().PadRight(22)}{rightPad}";
    ///        var engineering = $"{leftPad}{a.ToEngineeringNotation(256).PadRight(22)}{rightPad}";
    ///        var engineering3Figures = $"{leftPad}{a.ToEngineeringNotation(3)}";
    ///        Console.WriteLine($"/// {original}{engineering}{engineering3Figures}");
    ///     }
    ///     Console.ReadLine();
    ///   }
    ///     
    ///   private static IEnumerable<double> SampleNumbers
    ///   {
    ///     get
    ///     {
    ///       var testValues = new[]
    ///       {
    ///         Double.NaN,
    ///         Double.Epsilon,
    ///         Double.MinValue,
    ///         Double.MaxValue,
    ///         Double.NegativeInfinity,
    ///         Double.PositiveInfinity,
    ///         -300,
    ///         -30,
    ///         -1.1,
    ///         -1,
    ///         -0.1,
    ///         -0.01,
    ///         -0.001,
    ///         -0.0001,
    ///         0,
    ///         0.0001,
    ///         0.001,
    ///         0.01,
    ///         0.1,
    ///         1,
    ///         1.1,
    ///         30,
    ///         300
    ///       };
    ///  
    ///       foreach (double a in testValues)
    ///       {
    ///         yield return a;
    ///       }
    ///       for (int i = 28; i >= -28; --i)
    ///       {
    ///         yield return Math.Pow(10, i) * -1.234567890123;
    ///       }
    ///       for (int i = -28; i <= 28; ++i)
    ///       {
    ///         yield return Math.Pow(10, i) * 1.234567890123;
    ///       }
    ///     }
    ///   }
    /// }
    /// Gives the following output
    /// 
    ///  NaN                   ->  NaN                   ->  NaN
    ///  4.94065645841247E-324 ->  4.94065645841247E-324 ->  4.94E-324
    /// -1.79769313486232E+308--> -179.769313486232E+306--> -179E+306
    ///  1.79769313486232E+308 ->  179.769313486232E+306 ->  179E+306
    /// -Infinity             --> -Infinity             --> -Infinity
    ///  Infinity              ->  Infinity              ->  Infinity
    /// -30000                --> -30k                  --> -30k
    /// -3000                 --> -3k                   --> -3k
    /// -300                  --> -300                  --> -300
    /// -30                   --> -30                   --> -30
    /// -1.1                  --> -1.1                  --> -1.1
    /// -1                    --> -1                    --> -1
    /// -0.1                  --> -100m                 --> -100m
    /// -0.01                 --> -10m                  --> -10m
    /// -0.001                --> -1m                   --> -1m
    /// -0.0001               --> -100μ                 --> -100μ
    ///  0                     ->  0                     ->  0
    ///  0.0001                ->  100μ                  ->  100μ
    ///  0.001                 ->  1m                    ->  1m
    ///  0.01                  ->  10m                   ->  10m
    ///  0.1                   ->  100m                  ->  100m
    ///  1                     ->  1                     ->  1
    ///  1.1                   ->  1.1                   ->  1.1
    ///  30                    ->  30                    ->  30
    ///  300                   ->  300                   ->  300
    ///  3000                  ->  3k                    ->  3k
    ///  30000                 ->  30k                   ->  30k
    /// -1.234567890123E+28   --> -12.34567890123E+27   --> -12.3E+27
    /// -1.234567890123E+27   --> -1.234567890123E+27   --> -1.23E+27
    /// -1.234567890123E+26   --> -123.4567890123Y      --> -123Y
    /// -1.234567890123E+25   --> -12.34567890123Y      --> -12.3Y
    /// -1.234567890123E+24   --> -1.234567890123Y      --> -1.23Y
    /// -1.234567890123E+23   --> -123.4567890123Z      --> -123Z
    /// -1.234567890123E+22   --> -12.34567890123Z      --> -12.3Z
    /// -1.234567890123E+21   --> -1.234567890123Z      --> -1.23Z
    /// -1.234567890123E+20   --> -123.4567890123E      --> -123E
    /// -1.234567890123E+19   --> -12.34567890123E      --> -12.3E
    /// -1.234567890123E+18   --> -1.234567890123E      --> -1.23E
    /// -1.234567890123E+17   --> -123.4567890123P      --> -123P
    /// -1.234567890123E+16   --> -12.34567890123P      --> -12.3P
    /// -1.234567890123E+15   --> -1.234567890123P      --> -1.23P
    /// -123456789012300      --> -123.4567890123T      --> -123T
    /// -12345678901230       --> -12.34567890123T      --> -12.3T
    /// -1234567890123        --> -1.234567890123T      --> -1.23T
    /// -123456789012.3       --> -123.4567890123G      --> -123G
    /// -12345678901.23       --> -12.34567890123G      --> -12.3G
    /// -1234567890.123       --> -1.234567890123G      --> -1.23G
    /// -123456789.0123       --> -123.4567890123M      --> -123M
    /// -12345678.90123       --> -12.34567890123M      --> -12.3M
    /// -1234567.890123       --> -1.234567890123M      --> -1.23M
    /// -123456.7890123       --> -123.4567890123k      --> -123k
    /// -12345.67890123       --> -12.34567890123k      --> -12.3k
    /// -1234.567890123       --> -1.234567890123k      --> -1.23k
    /// -123.4567890123       --> -123.4567890123       --> -123
    /// -12.34567890123       --> -12.34567890123       --> -12.3
    /// -1.234567890123       --> -1.234567890123       --> -1.23
    /// -0.1234567890123      --> -123.4567890123m      --> -123m
    /// -0.01234567890123     --> -12.34567890123m      --> -12.3m
    /// -0.001234567890123    --> -1.234567890123m      --> -1.23m
    /// -0.0001234567890123   --> -123.4567890123μ      --> -123μ
    /// -1.234567890123E-05   --> -12.34567890123μ      --> -12.3μ
    /// -1.234567890123E-06   --> -1.234567890123μ      --> -1.23μ
    /// -1.234567890123E-07   --> -123.4567890123n      --> -123n
    /// -1.234567890123E-08   --> -12.34567890123n      --> -12.3n
    /// -1.234567890123E-09   --> -1.234567890123n      --> -1.23n
    /// -1.234567890123E-10   --> -123.4567890123p      --> -123p
    /// -1.234567890123E-11   --> -12.34567890123p      --> -12.3p
    /// -1.234567890123E-12   --> -1.234567890123p      --> -1.23p
    /// -1.234567890123E-13   --> -123.4567890123f      --> -123f
    /// -1.234567890123E-14   --> -12.34567890123f      --> -12.3f
    /// -1.234567890123E-15   --> -1.234567890123f      --> -1.23f
    /// -1.234567890123E-16   --> -123.4567890123a      --> -123a
    /// -1.234567890123E-17   --> -12.34567890123a      --> -12.3a
    /// -1.234567890123E-18   --> -1.234567890123a      --> -1.23a
    /// -1.234567890123E-19   --> -123.4567890123z      --> -123z
    /// -1.234567890123E-20   --> -12.34567890123z      --> -12.3z
    /// -1.234567890123E-21   --> -1.234567890123z      --> -1.23z
    /// -1.234567890123E-22   --> -123.4567890123y      --> -123y
    /// -1.234567890123E-23   --> -12.34567890123y      --> -12.3y
    /// -1.234567890123E-24   --> -1.234567890123y      --> -1.23y
    /// -1.234567890123E-25   --> -123.4567890123E-27   --> -123E-27
    /// -1.234567890123E-26   --> -12.34567890123E-27   --> -12.3E-27
    /// -1.234567890123E-27   --> -1.234567890123E-27   --> -1.23E-27
    /// -1.234567890123E-28   --> -123.4567890123E-30   --> -123E-30
    ///  1.234567890123E-28    ->  123.4567890123E-30    ->  123E-30
    ///  1.234567890123E-27    ->  1.234567890123E-27    ->  1.23E-27
    ///  1.234567890123E-26    ->  12.34567890123E-27    ->  12.3E-27
    ///  1.234567890123E-25    ->  123.4567890123E-27    ->  123E-27
    ///  1.234567890123E-24    ->  1.234567890123y       ->  1.23y
    ///  1.234567890123E-23    ->  12.34567890123y       ->  12.3y
    ///  1.234567890123E-22    ->  123.4567890123y       ->  123y
    ///  1.234567890123E-21    ->  1.234567890123z       ->  1.23z
    ///  1.234567890123E-20    ->  12.34567890123z       ->  12.3z
    ///  1.234567890123E-19    ->  123.4567890123z       ->  123z
    ///  1.234567890123E-18    ->  1.234567890123a       ->  1.23a
    ///  1.234567890123E-17    ->  12.34567890123a       ->  12.3a
    ///  1.234567890123E-16    ->  123.4567890123a       ->  123a
    ///  1.234567890123E-15    ->  1.234567890123f       ->  1.23f
    ///  1.234567890123E-14    ->  12.34567890123f       ->  12.3f
    ///  1.234567890123E-13    ->  123.4567890123f       ->  123f
    ///  1.234567890123E-12    ->  1.234567890123p       ->  1.23p
    ///  1.234567890123E-11    ->  12.34567890123p       ->  12.3p
    ///  1.234567890123E-10    ->  123.4567890123p       ->  123p
    ///  1.234567890123E-09    ->  1.234567890123n       ->  1.23n
    ///  1.234567890123E-08    ->  12.34567890123n       ->  12.3n
    ///  1.234567890123E-07    ->  123.4567890123n       ->  123n
    ///  1.234567890123E-06    ->  1.234567890123μ       ->  1.23μ
    ///  1.234567890123E-05    ->  12.34567890123μ       ->  12.3μ
    ///  0.0001234567890123    ->  123.4567890123μ       ->  123μ
    ///  0.001234567890123     ->  1.234567890123m       ->  1.23m
    ///  0.01234567890123      ->  12.34567890123m       ->  12.3m
    ///  0.1234567890123       ->  123.4567890123m       ->  123m
    ///  1.234567890123        ->  1.234567890123        ->  1.23
    ///  12.34567890123        ->  12.34567890123        ->  12.3
    ///  123.4567890123        ->  123.4567890123        ->  123
    ///  1234.567890123        ->  1.234567890123k       ->  1.23k
    ///  12345.67890123        ->  12.34567890123k       ->  12.3k
    ///  123456.7890123        ->  123.4567890123k       ->  123k
    ///  1234567.890123        ->  1.234567890123M       ->  1.23M
    ///  12345678.90123        ->  12.34567890123M       ->  12.3M
    ///  123456789.0123        ->  123.4567890123M       ->  123M
    ///  1234567890.123        ->  1.234567890123G       ->  1.23G
    ///  12345678901.23        ->  12.34567890123G       ->  12.3G
    ///  123456789012.3        ->  123.4567890123G       ->  123G
    ///  1234567890123         ->  1.234567890123T       ->  1.23T
    ///  12345678901230        ->  12.34567890123T       ->  12.3T
    ///  123456789012300       ->  123.4567890123T       ->  123T
    ///  1.234567890123E+15    ->  1.234567890123P       ->  1.23P
    ///  1.234567890123E+16    ->  12.34567890123P       ->  12.3P
    ///  1.234567890123E+17    ->  123.4567890123P       ->  123P
    ///  1.234567890123E+18    ->  1.234567890123E       ->  1.23E
    ///  1.234567890123E+19    ->  12.34567890123E       ->  12.3E
    ///  1.234567890123E+20    ->  123.4567890123E       ->  123E
    ///  1.234567890123E+21    ->  1.234567890123Z       ->  1.23Z
    ///  1.234567890123E+22    ->  12.34567890123Z       ->  12.3Z
    ///  1.234567890123E+23    ->  123.4567890123Z       ->  123Z
    ///  1.234567890123E+24    ->  1.234567890123Y       ->  1.23Y
    ///  1.234567890123E+25    ->  12.34567890123Y       ->  12.3Y
    ///  1.234567890123E+26    ->  123.4567890123Y       ->  123Y
    ///  1.234567890123E+27    ->  1.234567890123E+27    ->  1.23E+27
    ///  1.234567890123E+28    ->  12.34567890123E+27    ->  12.3E+27
    /// </example>
    /// <param name="d">the double to convert</param>
    /// <param name="significantFigures">The number of significant figures</param>
    /// <returns>A string</returns>
    public static string ToEngineeringNotation(this string originalString, int? significantFigures = null)
    {
      var str = originalString;

      // remove spaces and negative sign

      str.Replace(" ", "");
      string prefix = "";
      if (str[0] == '-')
      {
        str = str.Substring(1);
        prefix = "-";
      }

      // Get the exponent, remove the exponent nomenclature

      int exponent = 0;
      int exponentStrIndex = 0;
      if ((exponentStrIndex = str.IndexOfAny("Ee".ToArray())) >= 0)
      {
        string exponentStr = str.Substring(exponentStrIndex + 1);
        str = str.Substring(0, exponentStrIndex);
        Int32.TryParse(exponentStr, out exponent);
      }

      // remove the decimal point, and adjust the exponent so the decimal point
      // should go just after the first digit, and trim trailing zeros

      int currentDecimalPosition = str.IndexOf('.');
      if (currentDecimalPosition >= 0)
      {
        exponent += currentDecimalPosition - 1;
        str = str.Replace(".", "");
      }
      else
      {
        exponent += str.Length - 1;
      }
      str = str.TrimEnd('0');

      // At this point we should only have digits, just return the original string if we don't

      if (!str.All(char.IsDigit))
      {
        return originalString;
      }

      // Trim leading zeros, the decimal point is effectively moved as it's
      // just after the first digit, so adjust the exponent

      int lengthBefore = str.Length;
      str = str.TrimStart('0');
      exponent += str.Length - lengthBefore;

      // work out how much we need to shift the decimal point to get
      // engineering notation

      var decimalShiftRequired = exponent % 3;
      if (decimalShiftRequired < 0)
        decimalShiftRequired += 3;

      // Put the decimal point back in, but move the decimal point right
      // according to the shift worked out above.

      if (significantFigures.HasValue && significantFigures.Value < str.Length)
      {
        str = str.Substring(0, significantFigures.Value);
      }
      if (exponent == 0)
      {
        decimalShiftRequired = 0;
      }
      str = str.PadRight(1 + decimalShiftRequired, '0');
      str = $"{str.Substring(0, 1 + decimalShiftRequired)}.{str.Substring(1 + decimalShiftRequired)}";
      exponent -= decimalShiftRequired;

      // Remove the decimal point if there are no digits after it
      str = str.TrimEnd('.');

      // Create a default suffix consisting of the exponent
      string suffix = exponent != 0 ? $"E{(exponent < 0 ? "" : "+")}{exponent}" : "";

      // Work out which letter to put on the end, if any. If no letter is found,
      // then the Exponent suffix above will be added without modification

      switch (exponent)
      {
        case 3:
          suffix = "k"; break;
        case 6:
          suffix = "M"; break;
        case 9:
          suffix = "G"; break;
        case 12:
          suffix = "T"; break;
        case 15:
          suffix = "P"; break;
        case 18:
          suffix = "E"; break;
        case 21:
          suffix = "Z"; break;
        case 24:
          suffix = "Y"; break;
        case -3:
          suffix = "m"; break;
        case -6:
          suffix = "μ"; break;
        case -9:
          suffix = "n"; break;
        case -12:
          suffix = "p"; break;
        case -15:
          suffix = "f"; break;
        case -18:
          suffix = "a"; break;
        case -21:
          suffix = "z"; break;
        case -24:
          suffix = "y"; break;
      }
      return $"{prefix}{str}{suffix}";
    }
  }
}
