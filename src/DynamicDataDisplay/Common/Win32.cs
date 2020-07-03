using System;
using System.Runtime.InteropServices;
using System.Security;

namespace DynamicDataDisplay.Common
{
	public class Win32Stuff
	{
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern int GetDoubleClickTime();

		[SecurityCritical]
		[DllImport("User32.Dll", EntryPoint = "SetCursorPos")]
		private static extern bool SetCursorPosPInvoke(int x, int y);

		[SecuritySafeCritical]
		public static bool SetCursorPos(int x, int y)
		{
			return SetCursorPosPInvoke(x, y);
		}


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
