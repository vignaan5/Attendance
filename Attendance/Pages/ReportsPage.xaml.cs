using Attendance.Data;

namespace Attendance.Pages;

public partial class ReportsPage : ContentPage
{
	public DataClass dt = new DataClass();
	public ReportsPage()
	{
		InitializeComponent();
	}


	private async void showempreportbtn_Clicked(object sender, EventArgs e)
	{

		List<string> states = new List<string>();

		string emp_id = await SecureStorage.GetAsync("employee_id");
		await dt.get_emp_id();
		dt.start_connection();
		bool is_zonal_manager = dt.is_zonal_manager();
		if (is_zonal_manager)
		{
			string zone = dt.get_current_employee_zone();
			states = dt.get_all_states_from_zone_with_employee_id();
			states.Add(zone);
		}
		string state = dt.get_current_employee_state(emp_id);
		dt.close_connection();
		if (states.Count > 0)
		{

			Navigation.PushAsync(new Attendance2(states));
			return;


		}
		Navigation.PushAsync(new Attendance2(state));

	}

	private async void showteamreportbtn_Clicked(object sender, EventArgs e)
	{
		List<string> states = new List<string>();

		string emp_id = await SecureStorage.GetAsync("employee_id");
		await dt.get_emp_id();
		dt.start_connection();
		bool is_zonal_manager = dt.is_zonal_manager();
		if (is_zonal_manager)
		{
			string zone = dt.get_current_employee_zone();
			states = dt.get_all_states_from_zone_with_employee_id();
			states.Add(zone);
		}
		string state = dt.get_current_employee_state(emp_id);
		dt.close_connection();
		if (states.Count > 0)
		{
			Navigation.PushAsync(new ViewTeamReport(states));
			return;


		}
		Navigation.PushAsync(new ViewTeamReport(state));

	}

	private async void showproducts_Clicked(object sender, EventArgs e)
	{
		List<string> states = new List<string>();
		
		string emp_id = await SecureStorage.GetAsync("employee_id");
		await dt.get_emp_id();
		dt.start_connection();
		bool is_zonal_manager = dt.is_zonal_manager();
		if(is_zonal_manager)
		{
			string zone = dt.get_current_employee_zone();
			states = dt.get_all_states_from_zone_with_employee_id();
			states.Add(zone);
		}
	     string state = dt.get_current_employee_state(emp_id);
		dt.close_connection();
		if(states.Count > 0) 
		{
			Navigation.PushAsync(new DataGrid(states));
			return;


		}
		Navigation.PushAsync(new DataGrid(state));

	}

	private async void empstorestock_Clicked(object sender, EventArgs e)
	{
		string emp_id = await SecureStorage.GetAsync("employee_id");
		await dt.get_emp_id();
		dt.start_connection();
		string state = dt.get_current_employee_state(emp_id);
		bool is_zonal_manager= dt.is_zonal_manager();
		if(is_zonal_manager) 
		{
			string zone = dt.get_current_employee_zone();
			dt.close_connection();

			Navigation.PushAsync(new ViewStoreStock(true, state,zone));
			return;
		}
	
		
		dt.close_connection();
		Navigation.PushAsync(new ViewStoreStock(true, state));
	}
}