using System;
using System.Linq;
using System.Windows;

namespace DynamicDataDisplay
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
		/// Rounds up a number to the next highest power of 2
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public static int CeilingPow2(int x)
		{
			x--;
			x |= x >> 1;
			x |= x >> 2;
			x |= x >> 4;
			x |= x >> 8;
			x |= x >> 16;
			x++;
			return x;
		}
	}
}
