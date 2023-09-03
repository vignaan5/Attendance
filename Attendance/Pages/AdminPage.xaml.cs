
using Attendance.Data;

namespace Attendance.Pages;

public partial class AdminPage : ContentPage
{
	public bool is_admin_ { get; set; }	
	public DataClass db= new DataClass("");
	public AdminPage()
	{

	       
			InitializeComponent();
		check_and_del_ui();


	}

	public async Task<bool> is_admin()
	{
		string adm = await SecureStorage.GetAsync("admin");

		if(adm != null && adm=="yes") { return true; }

		return false ;
	 
	}

	private async void check_and_del_ui()
	{

		bool y = await is_admin();
		if (!y)

		{
			DisplayAlert("Access Denied", "Only Admin can access this page", "Ok");
			vs.Clear();
			Label lb = new Label();
			lb.Text = "Only Admin can access this page";
			vs.Add(lb);
		}
	}
	private async void Addempbtn(object sender, EventArgs e)
	{
        
	
			Navigation.PushAsync(new AddEmployee());

	
	}

	private void UpdateStoreTarget(object sender, EventArgs e)
	{
		Navigation.PushAsync(new UpdateTargets());
	}

	private void Statereports(object sender,EventArgs eventArgs)
	{
		Navigation.PushAsync(new ViewTeamReport());
	}



	private void DailyReports(object sender,EventArgs e)
	{

		Navigation.PushAsync(new DataGrid());	


	}


	private void remove_credentials(object sender, EventArgs e)
	{
		bool ps = SecureStorage.Remove("password");
		bool un = SecureStorage.Remove("username");

		

	}

     private void View_Employees(object sender, EventArgs e)
	{
		Navigation.PushAsync(new ViewEmployees());
	}
}