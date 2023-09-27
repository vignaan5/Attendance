using Attendance.Data;

namespace Attendance.Pages;

public partial class EmployeeAccountAccess : ContentPage
{
	public string emp_id { get; set; }
	public List<string> nothing_found_temp = new List<string> { "Nothing Found" };
	public DataClass dt = new DataClass();
	public EmployeeAccountAccess()
	{
		InitializeComponent();
	}

	private void emplis_ItemSelected(object sender, SelectedItemChangedEventArgs e)
	{
		this.emp_id = (string)emplist.SelectedItem;
		this.emp_id = this.emp_id.Split(' ')[0];
		searchvs.Clear();
		vs.Remove(searchvs);
		dt.start_connection();
	bool active=	 dt.is_employee_account_active(this.emp_id);
		dt.close_connection();

		if(active)
		{
			vs.Add( new Label { Text = String.Format("{0} account is active", this.emp_id),HorizontalOptions=LayoutOptions.Center }); 
		   
			Button accessbtn= new Button { Text="Revoke Account",HorizontalOptions=LayoutOptions.Center};

			accessbtn.Clicked += (async (object sender, EventArgs e) => 
			{
				dt.start_connection();
				dt.revoke_employee_account_access(this.emp_id);
				dt.close_connection();

				DisplayAlert("Account Revoked !", "Account access has been revoked", "ok");
				return;
			
			
			});

			vs.Add(accessbtn);
		}
		else if(!active)
		{

			vs.Add(new Label { Text = String.Format("{0} account is NOT active", this.emp_id), HorizontalOptions = LayoutOptions.Center });

			Button accessbtn = new Button { Text = "Re-Activate Account", HorizontalOptions = LayoutOptions.Center };

			accessbtn.Clicked += (async (object sender, EventArgs e) =>
			{
				dt.start_connection();
				dt.reactivate_employee_account_access(this.emp_id);
				dt.close_connection();

				DisplayAlert("Account Reactivated !", "Account access has been Re-Activated", "ok");
				return;


			});

			vs.Add(accessbtn);

		}


	
	}

	private void search_emp_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (search_emp.Text.Trim() == "")
		{

			emplist.ItemsSource = nothing_found_temp;
			return;
		}

		Task.Run(async () => {

			while (dt.is_conn_open)
			{

			}

			dt.start_connection();
			List<List<string>> result = dt.search_employee_in_db(search_emp.Text.Trim(), search_emp.Text.Trim(), search_emp.Text.Trim());
			dt.close_connection();

			MainThread.InvokeOnMainThreadAsync(() => {
				List<string> temp = new List<string>();
				foreach (List<string> list in result)
				{
					temp.Add(list[0] + " " + list[1] + " " + list[2]);

				}
				if (temp.Count == 0)
				{
					emplist.ItemsSource = nothing_found_temp;
				}
				else
				{
					emplist.ItemsSource = temp;
				}

				emplist.IsVisible = true;
			});


		});




	}



}