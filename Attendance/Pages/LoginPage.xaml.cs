namespace Attendance.Pages;

using Attendance.Data;
using MySqlConnector;
using System;

public partial class LoginPage : ContentPage
{
	public string emp_id { get; set; }
	public bool is_admin=false;
	public bool nothing_found=false;
	public string version = "0.3";
	public string store_name=String.Empty;
#if ANDROID26_0_OR_GREATER
	public Location.LocationClass l = new Location.LocationClass();
#endif   
	public  LoginPage()
	{
		InitializeComponent();
		
		check_already_signed_in();

	}

	public LoginPage(bool has_checked_signin_to_be_true)
	{
		InitializeComponent ();	
	}


	public async Task CopyFileToAppDataDirectory(string filename)
	{
		// Open the source file
		using Stream inputStream = await FileSystem.Current.OpenAppPackageFileAsync(filename);

		// Create an output filename
		string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, filename);

		// Copy the file to the AppDataDirectory
		using FileStream outputStream = File.Create(targetFile);
		await inputStream.CopyToAsync(outputStream);
	}




	public async void check_already_signed_in()
	{
		await CopyFileToAppDataDirectory("DigiCertGlobalRootG2.crt.pem");

		string pass = await SecureStorage.GetAsync("password");
		string usernm = await SecureStorage.GetAsync("username");
		string xz = sc.ScrollX.ToString();
		if (pass != null && usernm != null)
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
		
		

		await	Task.Run(async() => { bool z= await connect_to_db("", ""); if (!z) nothing_found = true; });
	 
		if(!nothing_found)
		{


				await SecureStorage.SetAsync("username", usrname.Text.ToString().Trim());
				await SecureStorage.SetAsync("password", passcode.Text.ToString().Trim());
			await SecureStorage.SetAsync("employee_id", this.emp_id);
			await SecureStorage.SetAsync("store_name", this.store_name.ToUpper().Trim());
			if(is_admin)
				await SecureStorage.SetAsync("admin","yes");
			else
				await SecureStorage.SetAsync("admin", "no");

			DataClass dt = new DataClass();
			dt.start_connection();
			string version_no=  dt.get_version_no();
			dt.close_connection();
			 
			if(version_no != version)
			{
				
					MainThread.InvokeOnMainThreadAsync(() => {

						DisplayAlert("Please Update to login!", "A newer version of this app is available please download it from your admin", "Ok!");

					});

				App.Current.MainPage = new UpdateAvailable();
				return;
				
			}

			//	App.Current.MainPage = new AppShell();
			//	App.Current.MainPage = new AppShell(this.emp_id,usrname.Text.ToString());

			if(store_name!=String.Empty)
			{
			 store_name=	store_name.ToUpper().Trim();
			}


			App.Current.MainPage =  new AppShell2(usrname.Text.ToString(),is_admin,store_name);





		}
		else
		{
			actind.IsVisible = false;
			DisplayAlert("Sorry !", "No account found with " + usrname.Text.ToString(), "Ok !");
			nothing_found = false;
			is_admin = false;
			App.Current.MainPage = new LoginPage(true);
			
		}
	
		

			}

	public  bool check_admin(string employee_id)
	{

		string sslcertificate_path = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "DigiCertGlobalRootG2.crt.pem");
		var builder = new MySqlConnectionStringBuilder
		{
			Server = "clayveda.mysql.database.azure.com",
			UserID = "vignaan",
			Password = "gyanu@18",
			Database = "clayveda",
			TlsVersion = "TLS 1.2",
			SslMode = MySqlSslMode.VerifyCA,
			SslCa = sslcertificate_path,
		};

		string sql_conn_string = builder.ToString();

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

		string sql_cmd_string = "select * from employee where emp_id='" +employee_id +"' and is_admin='yes'";

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

	public string get_version_no(MySqlConnection connection)
	{
		

		string sql_query = "select * from version;";
		MySqlCommand version_cmd = new MySqlCommand(sql_query, connection);

		try
		{
			return version_cmd.ExecuteScalar().ToString();
		}
		catch (Exception ex)
		{

		}

		return "";

	}






	public async Task<bool> connect_to_db(string usr, string pwd)
	{

		string sslcertificate_path = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "DigiCertGlobalRootG2.crt.pem");
		var builder = new MySqlConnectionStringBuilder
		{
			Server = "clayveda.mysql.database.azure.com",
			UserID = "vignaan",
			Password = "gyanu@18",
			Database = "clayveda",
			TlsVersion = "TLS 1.2",
			SslMode = MySqlSslMode.VerifyCA,
			SslCa = sslcertificate_path,
		};

		string sql_conn_string = builder.ToString();


		MySqlConnection conn = new MySqlConnection(sql_conn_string);

		try
		{
			conn.Open();
		}
		catch (Exception ex) 
		{ 
		     conn.Close();
#if ANDROID26_0_OR_GREATER

			MainThread.InvokeOnMainThreadAsync(() =>
			{
				l.send_notification_to_the_user("Can't Connect to the server", ex.Message.ToString());
			});

#endif
			return false;
		}

		string sql_cmd_string = "select * from employee where username='"+usrname.Text.ToString().Trim()+"';";
		MySqlCommand mySqlCommand =	new MySqlCommand(sql_cmd_string, conn);

		string versionNo = get_version_no(conn);
	            

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
				store_name = reader[7].ToString();

				if (reader[19].ToString().Trim()=="no")
				{
					nothing_found = true;
					reader.Close();
					conn.Close();
					MainThread.InvokeOnMainThreadAsync(() => {
						DisplayAlert("Account not active !", "Your account is not in active", "ok!");
					});
					return true;
				}
				

				if (reader[16].ToString().Trim() == usrname.Text.ToString().Trim() && reader[17].ToString().Trim() == passcode.Text.ToString().Trim())
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

						return true;

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
		return true;
	}

}