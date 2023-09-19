using Attendance.Data;


namespace Attendance.Pages;

public partial class AccountSettings : ContentPage
{
	public DataClass dt = new DataClass();
	public AccountSettings()
	{
		InitializeComponent();
		manage_event_handling();
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

					bool ps = SecureStorage.Remove("password");
					bool un = SecureStorage.Remove("username");
					bool ad = SecureStorage.Remove("admin");



				}
				else if(rows_affected==0) 
				{
					DisplayAlert("Passwords Update Failed !", "Your Password has not been updated, May be your Entered current password is incorrect  !", "Ok");
					return;
				}

			});
		
		
		
		});
	}
}