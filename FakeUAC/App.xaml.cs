using System.Windows;

namespace FakeUAC
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			Window w = Environment.OSVersion.Version.Build >= 22000 ? new Windows11() : new Windows10();
			w.Show();
		}
	}
}