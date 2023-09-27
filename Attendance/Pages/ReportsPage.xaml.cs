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
		string emp_id = await SecureStorage.GetAsync("employee_id");
		dt.start_connection();
		string state = dt.get_current_employee_state(emp_id);
		dt.close_connection();
		Navigation.PushAsync(new EmployeeAttendance(state));

	}

	private async void showteamreportbtn_Clicked(object sender, EventArgs e)
	{
		string emp_id = await SecureStorage.GetAsync("employee_id");
		dt.start_connection();
		string state = dt.get_current_employee_state(emp_id);
		dt.close_connection();
		Navigation.PushAsync(new ViewTeamReport(state));

	}
}