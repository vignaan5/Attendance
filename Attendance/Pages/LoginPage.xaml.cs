namespace Attendance.Pages;
using MySqlConnector;
public partial class LoginPage : ContentPage
{
	public string emp_id { get; set; }
	public bool is_admin=false;
	public bool nothing_found=false;
	public  LoginPage()
	{
		InitializeComponent();
		check_already_signed_in();
	}

	public async void check_already_signed_in()
	{
		string pass = await SecureStorage.GetAsync("password");
		string usernm = await SecureStorage.GetAsync("username");

		if(pass != null && usernm != null)
		{
			usrname.Text = usernm;
			passcode.Text = pass;
			login_btn.IsEnabled = false;
		
	       
			Login_to_app(null,null);

		}

	}



	private async void Login_to_app(object sender, EventArgs e)
	{
		actind.IsVisible = true;

	 await	Task.Run(() => { connect_to_db("", ""); });
	 
		if(!nothing_found)
		{
				await SecureStorage.SetAsync("username", usrname.Text.ToString().Trim());
				await SecureStorage.SetAsync("password", passcode.Text.ToString().Trim());
			await SecureStorage.SetAsync("employee_id", this.emp_id);
			if(is_admin)
				await SecureStorage.SetAsync("admin","yes");
			else
				await SecureStorage.SetAsync("admin", "no");


			//	App.Current.MainPage = new AppShell();
			//	App.Current.MainPage = new AppShell(this.emp_id,usrname.Text.ToString());

			App.Current.MainPage = new AppShell2(usrname.Text.ToString(),is_admin);





		}
		else
		{
			DisplayAlert("Sorry !", "No account found with " + usrname.Text.ToString(), "Ok !");
		}
	
		

			}

	public  bool check_admin(string employee_id)
	{
		string sql_conn_string = "Server=MYSQL8002.site4now.net;Database=db_a9daf3_vignaan;Uid=a9daf3_vignaan;Pwd=gyanu@18;SSL MODE = None;";

		MySqlConnection conn = new MySqlConnection(sql_conn_string);

		try
		{
			conn.Open();
		}
		catch (Exception ex)
		{
			conn.Close();
			return false;
		}

		string sql_cmd_string = "select * from admin where employee_id='" +employee_id +"';";

		MySqlCommand mySqlCommand = new MySqlCommand(sql_cmd_string, conn);



		try
		{
			mySqlCommand.ExecuteNonQuery();
		}
		catch (Exception ex)
		{
		}

		MySqlDataReader reader = mySqlCommand.ExecuteReader();

		if (reader.HasRows)
		{
			conn.Close();
			return true;
		}

		conn.Close();
		return false;

	}



	public async Task connect_to_db(string usr, string pwd)
	{
		string sql_conn_string = "Server=MYSQL8002.site4now.net;Database=db_a9daf3_vignaan;Uid=a9daf3_vignaan;Pwd=gyanu@18;SSL MODE = None;";

		MySqlConnection conn = new MySqlConnection(sql_conn_string);

		try
		{
			conn.Open();
		}
		catch (Exception ex) 
		{ 
		     conn.Close();
			return;
		}

		string sql_cmd_string = "select * from login_accounts where username='"+usrname.Text.ToString().Trim()+"';";

		MySqlCommand mySqlCommand =	new MySqlCommand(sql_cmd_string, conn);

	

		try
		{
			mySqlCommand.ExecuteNonQuery();
		}
		catch (Exception ex) 
		{ 
		}

		MySqlDataReader reader= null;
		try
		{
			reader = mySqlCommand.ExecuteReader();
		}
		catch
		{

		}
			if (reader.HasRows)
			while (!reader.IsClosed && reader.Read())

			{
				if (reader[1].ToString().Trim() == usrname.Text.ToString().Trim() && reader[2].ToString().Trim() == passcode.Text.ToString().Trim())
				{
					//	DisplayAlert("Welcome !", "Hello " + reader[1].ToString(), "Go !");
					string temp = reader[0].ToString().Trim();
					this.emp_id = temp;

					conn.Close();

					if (check_admin(temp))
					{
						
						is_admin = true;
					}

					else

						return;

				}
				else
				{
					nothing_found = true;
					//DisplayAlert("Sorry !", "No account found with " + usrname.Text.ToString(), "Ok !");
				}
			}

		else
		{
			nothing_found = true;
			//DisplayAlert("Sorry !", "No account found with " + usrname.Text.ToString(), "Ok !");
		}
		conn.Close();
		return;
	}

}