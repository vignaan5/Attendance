using System.Linq.Expressions;
using The_Attendance.Interfaces;

namespace Attendance.Pages;

public partial class LogoutPage : ContentPage
{
	public LogoutPage()
	{
		InitializeComponent();
	
	}

	private void remove_credentials(object sender, EventArgs e)
	{
		bool ps = SecureStorage.Remove("password");
		bool un = SecureStorage.Remove("username");
		bool ad = SecureStorage.Remove("admin");
#if ANDROID
		if(DependencyService.Resolve<IAndroid>().IsForeGroundServiceRunning()) 
		     DependencyService.Resolve<IAndroid>().StopMyService();
#endif   
		App.Current.MainPage= new MainPage();
		App.Current.MainPage=new LoginPage();
	}

	
}