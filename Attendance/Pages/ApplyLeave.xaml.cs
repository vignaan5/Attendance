using Attendance.Data;
using System.Runtime.CompilerServices;

namespace Attendance.Pages;

public partial class ApplyLeave : ContentPage
{
	public DataClass dt = new DataClass();
	public List<List<string>> pending_requests = new List<List<string>>();	
	public ApplyLeave()
	{

		InitializeComponent();
	    sdt.MinimumDate = DateTime.Now;
		edt.MinimumDate = DateTime.Now;
		applyleavebtn.IsEnabled = false;
		Task.Run(async () => 
		 {
			 dt.start_connection();
			 await dt.get_emp_id();
			pending_requests= dt.get_employee_pending_leave_requests();
			 dt.close_connection();

			 if(pending_requests!=null && pending_requests.Count>0)
			 {
				 MainThread.InvokeOnMainThreadAsync(() => { hs.Clear(); vs.Remove(Leavedur); vs.Remove(applyleavebtn); hs.Add(new Label { Text = String.Format("You Already have a pending leave request from {0} to {1} in progess ", pending_requests[0][1].Substring(0,10), pending_requests[0][2].Substring(0,10)) }) ;

					 Button canclerequestbtn = new Button { Text="Cancel Leave Request" , HorizontalOptions=LayoutOptions.Center };

					 canclerequestbtn.Clicked+=(async (object sender, EventArgs e) => 
					 {
						 dt.start_connection();
						 await dt.get_emp_id();
						 int rowsaffected = dt.cancel_employee_leave_request(pending_requests[0][6]);
						 dt.close_connection();

						 DisplayAlert("Leave request Cancled", "Your leave request has been cancled", "Ok");
						 Navigation.PopAsync();
					 
					 });

					 hs.Add(new VerticalStackLayout { canclerequestbtn });
				 
				 });
				
			 }
			 else
			 {

				 MainThread.InvokeOnMainThreadAsync(async() => { 
					 applyleavebtn.IsEnabled = true; });
				 
			 }

		 
		 });

	}

	private async void applyleavebtn_Clicked(object sender, EventArgs e)
	{
		if(sdt.Date>edt.Date)
		{
			DisplayAlert("Invalid Date", "Please Select valid date", "Ok");
			return;
		}

		TimeSpan date_dif =edt.Date.Subtract(sdt.Date);
		int days = Convert.ToInt32( date_dif.TotalDays.ToString() )+1;

		int request_approved = 0;

		dt.start_connection();
		await dt.get_emp_id();
		request_approved = dt.create_leave_request(sdt.Date.ToString("yyyy-MM-dd"), edt.Date.ToString("yyyy-MM-dd"), days.ToString());
		dt.close_connection();

		if (request_approved == 1)
		{
			DisplayAlert("Leave Request Assigned", "We will get back to you shortly !", "Ok");

		}
		else
		{
			DisplayAlert("Leave Request Not Assigned", "Something went wrong !", "Ok");

		}

		Navigation.PopAsync();
	}

	private void sdt_DateSelected(object sender, DateChangedEventArgs e)
	{
		if (sdt.Date > edt.Date)
		{
			
			return;
		}

		TimeSpan date_dif = edt.Date.Subtract(sdt.Date);
		int days = Convert.ToInt32(date_dif.TotalDays.ToString()) + 1;

		
	}

	private void edt_DateSelected(object sender, DateChangedEventArgs e)
	{
		if (sdt.Date > edt.Date)
		{

			return;
		}

		TimeSpan date_dif = edt.Date.Subtract(sdt.Date);
		int days = Convert.ToInt32(date_dif.TotalDays.ToString()) + 1;

		if (days == 1)
		{
			Leavedur.Text = "Apply leave for 1 day";
		}
		else
		{
			Leavedur.Text = String.Format("Apply Leave for {0} days ", days);
		}

		

		

	}

	private void viewrecentreqbtn_Clicked(object sender, EventArgs e)
	{
		Navigation.PushAsync(new LeaveRequests(true));
	}
}