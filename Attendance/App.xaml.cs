namespace Attendance;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new Pages.LoginPage();
		//  MainPage = new Pages.Test();
	}
}
