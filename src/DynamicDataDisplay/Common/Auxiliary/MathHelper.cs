using System;
using System.Linq;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay
{
	public static class MathHelper
	{
		public static long Clamp(long value, long min, long max)
		{
			return Math.Max(min, Math.Min(value, max));
		}

		public static double Clamp(double value, double min, double max)
		{
			return Math.Max(min, Math.Min(value, max));
		}

		/// <summary>Clamps specified value to [0,1]</summary>
		/// <param name="d">Value to clamp</param>
		/// <returns>Value in range [0,1]</returns>
		public static double Clamp(double value)
		{
			return Math.Max(0, Math.Min(value, 1));
		}

		public static int Clamp(int value, int min, int max)
		{
			return Math.Max(min, Math.Min(value, max));
		}

		public static Rect CreateRectByPoints(double xMin, double yMin, double xMax, double yMax)
		{
			return new Rect(new Point(xMin, yMin), new Point(xMax, yMax));
		}

		public static double Interpolate(double start, double end, double ratio)
		{
			return start * (1 - ratio) + end * ratio;
		}

		public static double RadiansToDegrees(this double radians)
		{
			return radians * 180 / Math.PI;
		}

		public static double DegreesToRadians(this double degrees)
		{
			return degrees / 180 * Math.PI;
		}

		/// <summary>
		/// Converts vector into angle.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <returns>Angle in degrees.</returns>
		public static double ToAngle(this Vector vector)
		{
			return Math.Atan2(-vector.Y, vector.X).RadiansToDegrees();
		}

		public static Point ToPoint(this Vector v)
		{
			return new Point(v.X, v.Y);
		}

		public static bool IsNaN(this double d)
		{
			return double.IsNaN(d);
		}

		public static bool IsNotNaN(this double d)
		{
			return !double.IsNaN(d);
		}

		public static bool IsFinite(this double d)
		{
			return !double.IsNaN(d) && !double.IsInfinity(d);
		}

		public static bool IsInfinite(this double d)
		{
			return double.IsInfinity(d);
		}

		public static bool AreClose(double d1, double d2, double diffRatio)
		{
			return Math.Abs(d1 / d2 - 1) < diffRatio;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="P">Point</param>
		/// <param name="A">Segment Start</param>
		/// <param name="B">Segment End</param>
		/// <returns></returns>
		public static (double distance, Point nearestPoint) DistancePointToSegment(Point P, (Point A, Point B) line)
		{
			var AB = line.B - line.A;
			var AP = P - line.A;

			// If no length or it dAB <=0
			Point closestPoint = line.A;
			if (AB.LengthSquared > 0)
			{
				double dAB = Vector.Multiply(AP, AB) / Vector.Multiply(AB, AB);
				if (dAB > 1)
				{
					closestPoint = line.B;
				}
				else if (dAB > 0)
				{
					closestPoint = new Point(line.A.X + dAB * AB.X, line.A.Y + dAB * AB.Y);
				}
			}
			return ((closestPoint - P).Length, closestPoint);
		}


		/// <summary>
		/// Converts a double to Engineering Notation
		/// </summary>
		/// <example>
		/// class Program {
		///   static void Main(string[] args) {
		///     for (int i = -18; i &lt;= 28; ++i) {
		///       double a = Math.Pow(10, i) * 1.234567890123;
		///       Console.WriteLine("{0} -> {1}Hz",a,a.ToEngineeringNotation());
		///     }
		///     Console.ReadLine();
		///   }
		/// }
		/// 
		/// Gives the following output
		/// 
		/// 1.234567890123E-18 -> 0.00123aHz
		/// 1.234567890123E-17 -> 0.0123aHz
		/// 1.234567890123E-16 -> 0.123aHz
		/// 1.234567890123E-15 -> 1.23fHz
		/// 1.234567890123E-14 -> 12.3fHz
		/// 1.234567890123E-13 -> 123fHz
		/// 1.234567890123E-12 -> 1.23pHz
		/// 1.234567890123E-11 -> 12.3pHz
		/// 1.234567890123E-10 -> 123pHz
		/// 1.234567890123E-09 -> 1.23nHz
		/// 1.234567890123E-08 -> 12.3nHz
		/// 1.234567890123E-07 -> 123nHz
		/// 1.234567890123E-06 -> 1.23µHz
		/// 1.234567890123E-05 -> 12.3µHz
		/// 0.0001234567890123 -> 123µHz
		/// 0.001234567890123 -> 1.23mHz
		/// 0.01234567890123 -> 12.3mHz
		/// 0.1234567890123 -> 123mHz
		/// 1.234567890123 -> 1.23Hz
		/// 12.34567890123 -> 12.3Hz
		/// 123.4567890123 -> 123Hz
		/// 1234.567890123 -> 1.23kHz
		/// 12345.67890123 -> 12.3kHz
		/// 123456.7890123 -> 123kHz
		/// 1234567.890123 -> 1.23MHz
		/// 12345678.90123 -> 12.3MHz
		/// 123456789.0123 -> 123MHz
		/// 1234567890.123 -> 1.23GHz
		/// 12345678901.23 -> 12.3GHz
		/// 123456789012.3 -> 123GHz
		/// 1234567890123 -> 1.23THz
		/// 12345678901230 -> 12.3THz
		/// 123456789012300 -> 123THz
		/// 1.234567890123E+15 -> 1.23PHz
		/// 1.234567890123E+16 -> 12.3PHz
		/// 1.234567890123E+17 -> 123PHz
		/// 1.234567890123E+18 -> 1.23EHz
		/// 1.234567890123E+19 -> 12.3EHz
		/// 1.234567890123E+20 -> 123EHz
		/// 1.234567890123E+21 -> 1.23ZHz
		/// 1.234567890123E+22 -> 12.3ZHz
		/// 1.234567890123E+23 -> 123ZHz
		/// 1.234567890123E+24 -> 1.23YHz
		/// 1.234567890123E+25 -> 12.3YHz
		/// 1.234567890123E+26 -> 123YHz
		/// 1.234567890123E+27 -> 1235YHz
		/// 1.234567890123E+28 -> 12346YHz
		/// </example>
		/// <param name="d">the double to convert</param>
		/// <param name="significantFigures">The number of significant figures</param>
		/// <returns>A string</returns>
		public static string ToEngineeringNotation(this double d, int significantFigures = 3)
		{

			// Here's a lambda funtion for formatting a number that ranges between 1 and 999
			Func<double, string> format = (x) =>
			{
				int decimalPlaces = significantFigures - ((int)Math.Floor(Math.Log10(Math.Abs(x))) + 1);
				if (decimalPlaces >= 0)
				{
					return x.ToString("F" + decimalPlaces.ToString());
				}
				// Need to start zeroing out figures that come to the left of the decimal place. Divide by powers of 10
				// and pad out with zeros
				var retVal = (x * Math.Pow(10, decimalPlaces)).ToString("F0").ToCharArray().Concat(Enumerable.Repeat('0', -decimalPlaces));
				return new string(retVal.ToArray());
			};

			// Convert the double to a number between 1 and 999
			// Format it with the above function
			// Add the appropriate suffix
			double exponent = Math.Log10(Math.Abs(d));
			if (Math.Abs(d) >= 1)
			{
				switch ((int)Math.Floor(exponent))
				{
					case 0:
					case 1:
					case 2:
						return format(d);
					case 3:
					case 4:
					case 5:
						return format(d / 1e3) + "k";
					case 6:
					case 7:
					case 8:
						return format(d / 1e6) + "M";
					case 9:
					case 10:
					case 11:
						return format(d / 1e9) + "G";
					case 12:
					case 13:
					case 14:
						return format(d / 1e12) + "T";
					case 15:
					case 16:
					case 17:
						return format(d / 1e15) + "P";
					case 18:
					case 19:
					case 20:
						return format(d / 1e18) + "E";
					case 21:
					case 22:
					case 23:
						return format(d / 1e21) + "Z";
					default:
						return format(d / 1e24) + "Y";
				}
			}
			else if (Math.Abs(d) > 0)
			{
				switch ((int)Math.Floor(exponent))
				{
					case -1:
					case -2:
					case -3:
						return format(d * 1e3) + "m";
					case -4:
					case -5:
					case -6:
						return format(d * 1e6) + "μ";
					case -7:
					case -8:
					case -9:
						return format(d * 1e9) + "n";
					case -10:
					case -11:
					case -12:
						return format(d * 1e12) + "p";
					case -13:
					case -14:
					case -15:
						return format(d * 1e15) + "f";
					case -16:
					case -17:
					case -18:
						return format(d * 1e18) + "a";
					case -19:
					case -20:
					case -21:
						return format(d * 1e21) + "z";
					default:
						return format(d * 1e24) + "y";
				}
			}
			else
			{
				return "0";
			}
		}

		/// <summary>
		/// Rounds up a number to the next highest power of 2
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public static int CeilingPow2(int x)
		{
			if (x > 2)
			{
				x--;
				x |= x >> 1;
				x |= x >> 2;
				x |= x >> 4;
				x |= x >> 8;
				x |= x >> 16;
				x++;
			}
			return x;
		}
	}
}
