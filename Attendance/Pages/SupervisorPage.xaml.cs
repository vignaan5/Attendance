using Microsoft.Maui.Graphics;

namespace Attendance.Pages;

public partial class SupervisorPage : ContentPage
{
	

	public SupervisorPage()
	{
		InitializeComponent();
		pname.Title = "Reports Page";
	}

	public SupervisorPage(string role)
	{
		InitializeComponent();

		if(role=="asm"||role=="AREASTATEMANAGER")
		{
			pname.Title = "Area State Manager";
		}

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
