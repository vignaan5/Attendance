
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


	private void SearchEmployeeClicked(object sender, EventArgs e)
	{
		Navigation.PushAsync(new ViewEmployeeLocations());

	}


	private void EmployeeAttendance(object sender, EventArgs e)
	{
		//Navigation.PushAsync(new EmployeeAttendance());
		Navigation.PushAsync(new Attendance2());
	}

	private void manageempclicked(object sender, EventArgs e)
	{
		Navigation.PushAsync(new EmployeeAccountAccess());

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

	private void Button_Clicked(object sender, EventArgs e)
	{

	}

	private void overrideempsalesbtn_Clicked(object sender, EventArgs e)
	{
		Navigation.PushAsync(new OverrideEmployeeSales());
    }

	private void leavereqbtn_Clicked(object sender, EventArgs e)
	{
		Navigation.PushAsync(new LeaveRequests());
    }

	private void manage_accounts_Clicked(object sender, EventArgs e)
	{
		Navigation.PushAsync(new AdminUpdateEmployee());
    }

	private void store_rankings_btn_Clicked(object sender, EventArgs e)
	{
		Navigation.PushAsync(new ViewStoreRankings());
	}

	private void products_rankings_btn_Clicked(object sender, EventArgs e)
	{
		Navigation.PushAsync(new ViewProductRankings());
	}

	private void storestock_btn_Clicked(object sender, EventArgs e)
	{
		Navigation.PushAsync(new ViewStoreStock(true));
	}

	private void devicechanges_Clicked(object sender, EventArgs e)
	{
		Navigation.PushAsync(new DeviceChanges());
    }

	private void bills_Clicked(object sender, EventArgs e)
	{
		Navigation.PushAsync(new UploadClayvedaNewVersion());

	}

	private void geosettingsbtn_Clicked(object sender, EventArgs e)
	{
		Navigation.PushAsync(new GeoLocationSettings());

	}
}