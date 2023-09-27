using Microsoft.Maui.Graphics;

namespace Attendance.Pages;

public partial class SupervisorPage : ContentPage
{
	

	public SupervisorPage()
	{
		InitializeComponent();
	}

	









	private async void manageempbtn_Clicked(object sender, EventArgs e)
	{
		Navigation.PushAsync(new ManageEmployee());
	}

	private void reportsbtn_Clicked(object sender, EventArgs e)
	{
		Navigation.PushAsync(new ReportsPage());	
	}
}
