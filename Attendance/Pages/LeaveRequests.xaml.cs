using Attendance.Data;
using System.Runtime.InteropServices;

namespace Attendance.Pages;

public partial class LeaveRequests : ContentPage
{
	public string state = String.Empty;
	public DataClass dt = new DataClass();
	public List<List<string>> requests = new List<List<string>>();
	public LeaveRequests()
	{
		InitializeComponent();

		Task.Run(() => {
			dt.start_connection();
			requests = dt.get_all_employees_pending_leave_requests();
			dt.close_connection();
			MainThread.InvokeOnMainThreadAsync(async () => { await create_ui(); });
		});

	}



	public LeaveRequests(string state)
	{
		InitializeComponent();

		Task.Run(() => {
			this.state = state;
			dt.start_connection();
			requests=dt.get_all_employees_pending_leave_requests(state);
		    dt.close_connection();
			
			MainThread.InvokeOnMainThreadAsync(async() => { await create_ui();  });
		});
	}

	public LeaveRequests(bool mine)
	{
		InitializeComponent();

		Task.Run(async() => {
			
			dt.start_connection();
			await dt.get_emp_id();
			requests = dt.get_employee_processed_leave_requests();
			dt.close_connection();

			MainThread.InvokeOnMainThreadAsync(async () => { await create_ui(true);return; });
		});

	}

	public async Task create_ui()
	{
		if(requests.Count == 0) 
		{
			DisplayAlert("No Leave Requests", "There are no leave request Currently", "Ok");
			Navigation.PopAsync();
			return;
		}

		for (int i = 0; i < requests.Count; i++)
		{

			Label[] l = new Label[9];

			l[0] = new Label { Text = "First Name : " + requests[i][8],HorizontalOptions=LayoutOptions.Center,VerticalOptions=LayoutOptions.Center };
			l[1] = new Label { Text = "Last Name :" + requests[i][9], HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
			l[2] = new Label { Text="EmployeeId :" + requests[i][0] };
			l[3] = new Label { Text="Store Name : " + requests[i][14] };
			l[4] =  new Label { Text="Area :" + requests[i][22] };
			l[5] = new Label { Text="City : " + requests[i][12] };
			l[6] = new Label { Text="State : " + requests[i][13] };
			l[8] = new Label { Text = String.Format("Leave From {0} to {1} Total : {2} Day(s)", requests[i][1].Substring(0,10), requests[i][2].Substring(0,10), requests[i][3]) };
			l[7] = new Label { Text = String.Format("Contact : {0} ", requests[i][16]) };
			VerticalStackLayout tempvs= new VerticalStackLayout {Spacing=10 };

			foreach(Label label in l) { tempvs.Add(label); }


			Button approvebtn = new Button { Text = "Approve:" + requests[i][0] + ":" + requests[i][6], HorizontalOptions = LayoutOptions.Center };
			
			approvebtn.Clicked += (async (object sender,EventArgs e) => 
			 {
				 string empid = approvebtn.Text.Split(':')[1];
				 string reqid = approvebtn.Text.Split(":")[2];
				 dt.start_connection();
				 await dt.get_emp_id();
				 dt.approve_leave(empid,reqid);
				 if(state!=String.Empty)
				 {
					 requests.Clear();
					 requests = dt.get_all_employees_pending_leave_requests(state);
					
				 }
				 else
				 {
					 requests.Clear();
					 requests = dt.get_all_employees_pending_leave_requests();

				 }

				 dt.close_connection();
				 innervs.Clear();
				 await create_ui();


			 });

			 tempvs.Add(approvebtn);


			Button declinebtn = new Button { Text = "Decline:" + requests[i][0] + ":" + requests[i][6], HorizontalOptions = LayoutOptions.Center };

			declinebtn.Clicked += (async (object sender, EventArgs e) =>
			{
				string empid = declinebtn.Text.Split(':')[1];
				string reqid = approvebtn.Text.Split(":")[2];
				dt.start_connection();
				await dt.get_emp_id();
				dt.decline_leave(empid,reqid);
				if (state != String.Empty)
				{
					requests.Clear();
					requests = dt.get_all_employees_pending_leave_requests(state);

				}
				else
				{
					requests.Clear();
					requests = dt.get_all_employees_pending_leave_requests();

				}

				dt.close_connection();
				innervs.Clear();
				await create_ui();


			});

			tempvs.Add(declinebtn);





			Frame f = new Frame { HorizontalOptions=LayoutOptions.Center, VerticalOptions=LayoutOptions.Center , WidthRequest=400,HeightRequest=500,Content=tempvs };
			
			innervs.Add(f);
		}

		innervs.Remove(actind);

	}


	public async Task create_ui(bool mine)
	{
		if (requests.Count == 0)
		{
			DisplayAlert("No Leave Requests", "There are no leave request Currently", "Ok");
			Navigation.PopAsync();
			Navigation.PopAsync();
			return;
		}

		for (int i = 0; i < requests.Count; i++)
		{

			Label[] l = new Label[10];

			l[0] = new Label { Text = "First Name : " + requests[i][8], HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
			l[1] = new Label { Text = "Last Name :" + requests[i][9], HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
			l[2] = new Label { Text = "EmployeeId :" + requests[i][0] };
			l[3] = new Label { Text = "Store Name : " + requests[i][14] };
			l[4] = new Label { Text = "Area :" + requests[i][22] };
			l[5] = new Label { Text = "City : " + requests[i][12] };
			l[6] = new Label { Text = "State : " + requests[i][13] };
			l[8] = new Label { Text = String.Format("Leave From {0} to {1} Total : {2} Day(s)", requests[i][1].Substring(0, 10), requests[i][2].Substring(0, 10), requests[i][3]) };
			l[7] = new Label { Text = String.Format("Contact : {0} ", requests[i][16]) };
			if (requests[i][4]=="True")
			l[9]= new Label { Text = String.Format("Status : {0} ", "Approved !"),FontSize=20 };
			if (requests[i][4] == "False")
				l[9] = new Label { Text = String.Format("Status : {0} ", "Rejected !"), FontSize = 20 };

			VerticalStackLayout tempvs = new VerticalStackLayout { Spacing = 10 };

			foreach (Label label in l) { tempvs.Add(label); }


			

		


			

			





			Frame f = new Frame { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, WidthRequest = 400, HeightRequest = 500, Content = tempvs };

			innervs.Add(f);
		}

		innervs.Remove(actind);

	}

	private void homebtn_Clicked(object sender, EventArgs e)
	{
		Navigation.PopToRootAsync();
	}
}