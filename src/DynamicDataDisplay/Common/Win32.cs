using System;
using System.Runtime.InteropServices;

namespace Microsoft.Research.DynamicDataDisplay.Common
{
	public class Win32Stuff
	{
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern int GetDoubleClickTime();

		[DllImport("User32.Dll")]
		public static extern bool SetCursorPos(int x, int y);

		[DllImport("User32.Dll")]
		public static extern bool GetCursorPos(out POINT lpPoint);

		[DllImport("User32.Dll")]
		public static extern bool ClientToScreen(IntPtr hWnd, ref POINT point);

		[DllImport("User32.Dll")]
		public static extern bool ScreenToClient(IntPtr hWnd, ref POINT point);

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int x;
			public int y;
		}

	}
}
