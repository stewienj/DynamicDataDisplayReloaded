using System;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Navigation
{
	/// <summary>Helper class to parse Windows messages</summary>
	public static class MessagesHelper
	{
		public static int GetXFromLParam(IntPtr lParam)
		{
			return LOWORD(lParam.ToInt32());
		}

		public static int GetYFromLParam(IntPtr lParam)
		{
			return HIWORD(lParam.ToInt32());
		}

		public static Point GetMousePosFromLParam(IntPtr lParam)
		{
			return new Point(GetXFromLParam(lParam), GetYFromLParam(lParam));
		}

		public static int GetWheelDataFromWParam(IntPtr wParam)
		{
			return HIWORD(wParam.ToInt32());
		}

		public static short HIWORD(int i)
		{
			return (short)((i & 0xFFFF0000) >> 16);
		}

		public static short LOWORD(int i)
		{
			return (short)(i & 0x0000FFFF);
		}
	}
}
