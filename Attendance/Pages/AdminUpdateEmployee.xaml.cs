using Attendance.Data;

namespace Attendance.Pages;

public partial class AdminUpdateEmployee : ContentPage
{

	public DataClass dt = new DataClass();
	public Dictionary<string, string> employee_details = new Dictionary<string, string>();
	public List<string> placeholder_list = new List<string>();
	public List<string> nothing_found_temp = new List<string> { "Nothing Found" };
	public string emp_id = String.Empty;
	public AdminUpdateEmployee()
	{
		InitializeComponent();
		
}

	private void update_details_btn_Clicked(object sender, EventArgs e)
	{
		if (update_details_btn.Text == "Update Employee Details (+)")
		{
			update_details_btn.Text = "Update Employee Details (-)";
			for (int i = 1; i < vs.Count; i++)
			{
				if (vs[i].GetType() == typeof(HorizontalStackLayout))
				{

					HorizontalStackLayout temp = (HorizontalStackLayout)vs[i];

					foreach (Entry ent in temp)
					{
						ent.IsVisible = true;
						ent.IsEnabled = true;

					}


				}
				else if (vs[i].GetType() == typeof(Button))
				{
					Button button = (Button)vs[i];
					button.IsEnabled = true;
					button.IsVisible = true;
				}
			}
		}
		else if (update_details_btn.Text == "Update Employee Details (-)")
		{
			update_details_btn.Text = "Update Employee Details (+)";
			for (int i = 1; i < vs.Count; i++)
			{
				if (vs[i].GetType() == typeof(HorizontalStackLayout))
				{

					HorizontalStackLayout temp = (HorizontalStackLayout)vs[i];

					foreach (Entry ent in temp)
					{
						ent.IsVisible = false;
						ent.IsEnabled = false;

					}


				}
				else if (vs[i].GetType() == typeof(Button))
				{
					Button button = (Button)vs[i];
					button.IsEnabled = false;
					button.IsVisible = false;
				}
			}
		}

	}

	private async void update_details2_btn_Clicked(object sender, EventArgs e)
	{
		Task.Run(() => {

			MainThread.InvokeOnMainThreadAsync(async () => { actind.IsVisible = true; });

		});
		update_details_btn_Clicked(null, null);


		for (int i = 1; i < vs.Count - 1; i++)
		{
			if (vs[i].GetType() == typeof(HorizontalStackLayout))
			{
				HorizontalStackLayout temphs = (HorizontalStackLayout)vs[i];
				Entry temp = temphs[0] as Entry;
				if (temp != null && temp.Text != null && temp.Text.Trim() != "")
				{
					placeholder_list[i - 1] = temp.Text.Trim();
				}
			}
		}
		
		dt.start_connection();
		
		//bool zsmID = dt.employee_id_exists(placeholder_list[13]);
		//bool ASMID = dt.employee_id_exists(asmid.Text.Trim());
		//bool svisorID = dt.employee_id_exists(visorid.Text.Trim());
		//if(!zsmID || !ASMID || !svisorID)
		//{
		//	dt.close_connection();
		//	DisplayAlert("ID not Found", "ZSM ID or ASM ID or SuperVisor ID Does not Exist, Please Check!", "OK!");
		//	return;
		//}
		int rows_affected = dt.update_employee_details(placeholder_list,this.emp_id,true);
		dt.close_connection();

		if (rows_affected == 0)
		{
			DisplayAlert("Not Updated !", "Sorry ! Something went wrong", "Ok!");
		}
		else
		{
			dt.start_connection();
			employee_details = dt.get_employee_details_with_column_names(this.emp_id);
			dt.close_connection();

			placeholder_list = new List<string> { employee_details["firstname"], employee_details["lastname"], employee_details["age"], employee_details["bank_account_name"], employee_details["bank_account_number"], employee_details["ifsc_code"] };


			DisplayAlert("Updated !", "Yours Details has been updated", "Ok!");
			actind.IsVisible = false;

		}

		Navigation.PopAsync();


	}

	private void emplist_ItemSelected(object sender, SelectedItemChangedEventArgs e)
	{
		this.emp_id = (string)emplist.SelectedItem;
		this.emp_id = this.emp_id.Split(' ')[0];

		emp_vs.Clear();

		

		Task.Run(async () => {
			
			dt.start_connection();
			employee_details = dt.get_employee_details_with_column_names(this.emp_id);
			dt.close_connection();

			MainThread.InvokeOnMainThreadAsync(async () =>
			{
				try
				{
					placeholder_list = new List<string> { employee_details["firstname"], employee_details["lastname"], employee_details["age"], employee_details["bank_account_name"], employee_details["bank_account_number"], employee_details["ifsc_code"], employee_details["store_name"], employee_details["area"], employee_details["city"], employee_details["state"], employee_details["monthly_target"], employee_details["emp_zonal_mgr_id"], employee_details["emp_asm_id"],employee_details["emp_supervisor_id"] };
				}
				catch(Exception ex) 
				{ 
				
				}
					int k = 0;
				foreach (var ele in vs)
				{
					if (ele.GetType() == typeof(HorizontalStackLayout))
					{

						HorizontalStackLayout temphs = ele as HorizontalStackLayout;
						Entry ent = null;
						try
						{
							 ent = (Entry)temphs[0];
						}
						catch(Exception ex) 
						{ 
						
						}
							ent.Placeholder += placeholder_list[k++];

					}
				}

				actind.IsVisible = false;
				
				
				update_details_btn.IsEnabled = true;
				update_details_btn.IsVisible = true;
				actind.IsVisible = false;

			});


		});













	}


	private void search_emp_TextChanged(object sender, TextChangedEventArgs e)
	{
		emplist.ItemsSource = nothing_found_temp;
		emplist.IsVisible = true;
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
					temp.Add(list[0] + "  " + list[1] + " " + list[2]);

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