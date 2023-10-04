using Attendance.Data;
using The_Attendance.Interfaces;

namespace Attendance.Pages;

public partial class AccountSettings : ContentPage
{
	public DataClass dt = new DataClass();
	public Dictionary<string, string> employee_details=new Dictionary<string, string>();
	public List<string> placeholder_list = new List<string>();	
	public AccountSettings()
	{
		InitializeComponent();
		Task.Run(async() => {
			await dt.get_emp_id();
			dt.start_connection();
			employee_details = dt.get_employee_details_with_column_names();
			dt.close_connection();

			MainThread.InvokeOnMainThreadAsync(async() => 
			{
				placeholder_list = new List<string> { employee_details["firstname"], employee_details["lastname"], employee_details["age"],employee_details["bank_account_name"], employee_details["bank_account_number"], employee_details["ifsc_code"] };
				int k = 0;
			   foreach(var ele in vs)
				{
					if(ele.GetType()==typeof(HorizontalStackLayout))
					{
						HorizontalStackLayout temphs = ele as HorizontalStackLayout;
						Entry ent = (Entry)temphs[0];
						ent.Placeholder += placeholder_list[k++];
						
					}
				}

				actind.IsVisible = false;
				manage_event_handling();
				changepassbtn.IsEnabled = true;
				changepassbtn.IsVisible = true;
				update_details_btn.IsEnabled = true;
				update_details_btn.IsVisible = true;
				actind.IsVisible = false;
			
			});
			
		
		});
	}

	private void remove_credentials(object sender, EventArgs e)
	{
		bool ps = SecureStorage.Remove("password");
		bool un = SecureStorage.Remove("username");
		bool ad = SecureStorage.Remove("admin");
#if ANDROID
		if (DependencyService.Resolve<IAndroid>().IsForeGroundServiceRunning())
			DependencyService.Resolve<IAndroid>().StopMyService();
#endif
		App.Current.MainPage = new MainPage();
		App.Current.MainPage = new LoginPage();
	}







	public void manage_event_handling()
	{
		changepassbtn.Clicked += ((object sender, EventArgs e) => 
		{ 
		   currentpasswordentry.IsVisible = true;
			currentpasswordentry.IsEnabled = true;

			new_password_entry.IsEnabled = true;
			new_password_entry.IsVisible = true;

			confirm_password.IsVisible = true;
			confirm_password.IsEnabled = true;

			updatepassbtn.IsEnabled = true;
			updatepassbtn.IsVisible = true;

			updatepassbtn.Clicked += (async(object sender, EventArgs e) =>
			{ 
			     if(confirm_password.Text==null|| confirm_password.Text.Trim().Length<=0 || currentpasswordentry.Text==null || currentpasswordentry.Text.Trim().Length<=0 || new_password_entry.Text==null | new_password_entry.Text.Trim().Length<=0) 
				{
					DisplayAlert("Not Updated !", "No Empty Feils allowed", "Ok");
					return;
				}

				 if(confirm_password.Text.Trim()!=new_password_entry.Text.Trim())
				{
					DisplayAlert("Passwords do not match!", "Please enter the same password in both the feilds", "Ok");
					return;

				}

				dt.start_connection();
				await dt.get_emp_id();
				int rows_affected= dt.update_account_password(currentpasswordentry.Text.Trim(), new_password_entry.Text.Trim());
				dt.close_connection();

				if(rows_affected==1)
				{
					currentpasswordentry.IsVisible = false;
					currentpasswordentry.IsEnabled = false;

					new_password_entry.IsEnabled = false;
					new_password_entry.IsVisible = false;

					confirm_password.IsVisible = false;
					confirm_password.IsEnabled = false;

					updatepassbtn.IsEnabled = false;
					updatepassbtn.IsVisible = false;

				     

 					DisplayAlert("Passwords Updated!", "Your Password has been updated next time login with your new password !", "Ok");

					remove_credentials(null,null);




				}
				else if(rows_affected==0) 
				{
					DisplayAlert("Passwords Update Failed !", "Your Password has not been updated, May be your Entered current password is incorrect  !", "Ok");
					return;
				}

			});
		
		
		
		});
	}

	private void update_details_btn_Clicked(object sender, EventArgs e)
	{
		if(update_details_btn.Text== "Update Your Details (+)")
		{
			update_details_btn.Text = "Update Your Details (-)";
			for(int i=1;i<vs.Count;i++)
			{
				if (vs[i].GetType()==typeof(HorizontalStackLayout))
				{
					
					 HorizontalStackLayout temp = (HorizontalStackLayout)vs[i];	

					foreach(Entry ent in temp)
					{
						ent.IsVisible=true;
						ent.IsEnabled= true;	
				
					}
					

		        }
				else if (vs[i].GetType()==typeof(Button))
				{
					Button button = (Button)vs[i];
					button.IsEnabled = true;
					button.IsVisible = true;
				}
			}
		}
		else if (update_details_btn.Text == "Update Your Details (-)")
		{
			update_details_btn.Text = "Update Your Details (+)";
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
		update_details_btn_Clicked(null,null);
		
		
		for(int i=1;i<vs.Count-1;i++)
		{
			if (vs[i].GetType()== typeof(HorizontalStackLayout)) 
			{ 
			  HorizontalStackLayout temphs = (HorizontalStackLayout)vs[i];
				Entry temp = temphs[0] as Entry;
				if(temp!=null && temp.Text!=null && temp.Text.Trim()!="")
				{
					placeholder_list[i-1] = temp.Text.Trim();
				}
			}
		}
		await dt.get_emp_id();
		dt.start_connection();
		int rows_affected = dt.update_employee_details(placeholder_list);
		dt.close_connection();

		if(rows_affected==0)
		{
			DisplayAlert("Not Updated !", "Sorry ! Something went wrong", "Ok!");
		}
		else
		{
			dt.start_connection();
			employee_details = dt.get_employee_details_with_column_names();
			dt.close_connection();

			placeholder_list = new List<string> { employee_details["firstname"], employee_details["lastname"], employee_details["age"], employee_details["bank_account_name"], employee_details["bank_account_number"], employee_details["ifsc_code"] };
			

			DisplayAlert("Updated !", "Yours Details has been updated", "Ok!");
			actind.IsVisible = false;

		}


	}

}