using MySqlConnector;
namespace Attendance.Pages;

public partial class AddEmployee : ContentPage
{
	public AddEmployee()
	{
		InitializeComponent();
		
	}

	public  bool is_unique()
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

	   string sql_cmd_string1 = "select * from employee where emp_id='" +eid.Text.ToString() + "';";
		string sql_cmd_string2 = "select * from employee where username='" + username.Text.ToString() + "';";
		MySqlCommand mySqlCommand = new MySqlCommand(sql_cmd_string1, conn);

		MySqlCommand mySqlCommand2 = new MySqlCommand(sql_cmd_string2, conn);

		try
		{
			mySqlCommand.ExecuteNonQuery();
			mySqlCommand2.ExecuteNonQuery();
		}
		catch (Exception ex)
		{
		}

		
		MySqlDataReader reader2 = mySqlCommand2.ExecuteReader();

		if(reader2.HasRows)
		{
			DisplayAlert("UserName Already Exists !", "Please choose a different a username", "Ok");
				{
				conn.Close();
				return false;
			}
		}
		reader2.Close();

		MySqlDataReader reader = mySqlCommand.ExecuteReader();
		if (!reader.HasRows )
		{
			conn.Close();
			return true;
		}

		conn.Close();
		return false;

	
	}

	public void add_employee_to_db()
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
			return ;
		}

		string new_sql_cmd_string = "";
		List<string> feild_list = new List<string>();

		foreach(var feild in dobhs)
		{
			

			
					if(feild.GetType()==typeof(DatePicker))
					{
						string temp_date = "";
						temp_date += ((DatePicker)feild).Date.Year.ToString();
						temp_date += "-" + ((DatePicker)feild).Date.Month.ToString();
						temp_date+= "-" + ((DatePicker)feild).Date.Day.ToString();

					  feild_list.Add(temp_date);

					}
				
		}


		foreach (var feild in ejdatehs)
		{



			if (feild.GetType() == typeof(DatePicker))
			{
				string temp_date = "";
				temp_date += ((DatePicker)feild).Date.Year.ToString();
				temp_date += "-" + ((DatePicker)feild).Date.Month.ToString();
				temp_date += "-" + ((DatePicker)feild).Date.Day.ToString();

				feild_list.Add(temp_date);

			}

		}



		DatePicker xyz= new DatePicker();
		 DateOnly dateOnly = new DateOnly();
		int current_age = Convert.ToInt32(xyz.Date.Year.ToString())-Convert.ToInt32( DOBpicker.Date.Year.ToString() );

		string s1 = eid.Text.ToString().Trim();
		string s2 = fname.Text.ToString().Trim();
		string s3 = lstname.Text.ToString().Trim();
		string s4 = current_age.ToString().Trim();
		string s5 = feild_list[1].Trim();
		string s6 = city.Text.ToString().Trim();
		string s7 = state.Text.ToString().Trim();
		string s8 = store_name.Text.ToString().Trim();
		string s9 = feild_list[0].Trim();
		string s10=econtact.Text.ToString().Trim();
		string s11 = ebnk_acc.Text.ToString().Trim();
		string s12 = ifsc.Text.ToString().Trim();
		string s13 = etarget.Text.ToString().Trim();
		string s14 = eweekofdaypicker.ItemsSource[eweekofdaypicker.SelectedIndex].ToString().Trim();
		string s15 = bnk_name.Text.ToString().Trim();
		string s16 = area.Text.ToString().Trim();



		new_sql_cmd_string = String.Format("insert into employee values ( '{0}','{1}','{2}',{3},'{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}');",s1,s2,s3,s4,s5,s6,s7,s8,s9,s10,s11,s12,s13,s14,s15,s16,username.Text.ToString().Trim(),passcode.Text.ToString().Trim(),"no","yes");



		

		MySqlCommand mySqlCommand1 = new MySqlCommand(new_sql_cmd_string, conn);	


		try
		{
			mySqlCommand1.ExecuteNonQuery();
		}
		catch
		{

		}

	

		conn.Close();


	}

	public bool check_empty_feilds()
	{
		foreach(var feild in vs)
		{         if(feild.GetType() == typeof(Entry) )
					{
				if (((Entry)feild).Text == null || ((Entry)feild).Text.Length <= 0)
					return true;
			    
			}

		    if(feild.GetType()==typeof(Picker))
			{
				if (((Picker)feild).SelectedIndex == -1)
				{
					DisplayAlert("Weekoffday not selected !", "Please select employee weekoff day", "ok");
					return true;
				}
			}
		}



		return false;




	}

	private async void sumbitBtn(object sender, EventArgs e)
	{
		if(check_empty_feilds())
		{
			DisplayAlert("Empty Feild !", "Fill all the details and no empty feilds allowed", "Ok");
			return;
		}


		if(is_unique())
		{
			actind.IsVisible = true;	
		  await	Task.Run(() => { 
			  add_employee_to_db(); });
			actind.IsVisible=false;	
			DisplayAlert("Done !", "Added Employee", "Ok");
			Navigation.PopAsync();
		}
		else
		{
			DisplayAlert("Duplicate", "Employee already exists or Username already exists", "Ok");
		}


	}
}