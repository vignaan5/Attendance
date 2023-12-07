using Attendance.Data;

namespace Attendance.Pages;

public partial class ManageEmployee : ContentPage
{
	public DataClass dt = new DataClass();	
	public ManageEmployee()
	{
		InitializeComponent();
	}





	

	private async void showempbtn_Clicked(object sender, EventArgs e)
	{
		string emp_id = await SecureStorage.GetAsync("employee_id");
		await dt.get_emp_id();
		dt.start_connection();
		string storeName = dt.get_current_employee_storename(emp_id);
		string zone = dt.get_current_employee_zone();
		dt.close_connection();

		if(storeName.Contains("ZONAL")) 
		{
			Navigation.PushAsync(new ViewEmployees(emp_id,zone));
			return;

		}


		Navigation.PushAsync(new ViewEmployees(emp_id));
	}

	private async void showempreportbtn_Clicked(object sender, EventArgs e)
	{
		string emp_id = await SecureStorage.GetAsync("employee_id");
		dt.start_connection();
		string state= dt.get_current_employee_state(emp_id);
		dt.close_connection();
		Navigation.PushAsync(new EmployeeAttendance(state));

	}

	private async void leavreqbtn_Clicked(object sender, EventArgs e)
	{
		string emp_id = await SecureStorage.GetAsync("employee_id");
		dt.start_connection();
		string state = dt.get_current_employee_state(emp_id);
		dt.close_connection();
		Navigation.PushAsync(new LeaveRequests(state));
	}
}