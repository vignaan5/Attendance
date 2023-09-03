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

	   string sql_cmd_string1 = "select * from login_accounts where employee_id='" +eid.Text.ToString() + "';";
		string sql_cmd_string2 = "select * from login_accounts where username='" + username.Text.ToString() + "';";
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

		string sql_conn_string = "Server=MYSQL8002.site4now.net;Database=db_a9daf3_vignaan;Uid=a9daf3_vignaan;Pwd=gyanu@18;SSL MODE = None;";

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

		string s1 = eid.Text.ToString();
		string s2 = fname.Text.ToString();
		string s3 = lstname.Text.ToString();
		string s4 = current_age.ToString();
		string s5 = feild_list[1];
		string s6 = city.Text.ToString();
		string s7 = state.Text.ToString();
		string s8 = store_name.Text.ToString();
		string s9 = feild_list[0];
		string s10=econtact.Text.ToString();
		string s11 = ebnk_acc.Text.ToString();
		string s12 = ifsc.Text.ToString();
		string s13 = etarget.Text.ToString();
		string s14 = eweekofdaypicker.ItemsSource[eweekofdaypicker.SelectedIndex].ToString();
		string s15 = bnk_name.Text.ToString();
		string s16 = area.Text.ToString();



		new_sql_cmd_string = String.Format("insert into employee values ( '{0}','{1}','{2}',{3},'{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}');",s1,s2,s3,s4,s5,s6,s7,s8,s9,s10,s11,s12,s13,s14,s15,s16);


		string sql_cmd_string1 = "insert into login_accounts values ('" + eid.Text.ToString()+"','"+username.Text.ToString()+"','"+ passcode.Text.ToString() + "');";

		MySqlCommand mySqlCommand = new MySqlCommand(sql_cmd_string1, conn);
		MySqlCommand mySqlCommand1 = new MySqlCommand(new_sql_cmd_string, conn);	


		try
		{
			mySqlCommand1.ExecuteNonQuery();
		}
		catch
		{

		}

		try
		{
			mySqlCommand.ExecuteNonQuery();
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
				if( ((Picker)feild).SelectedIndex==-1  )
					return true;
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
		}
		else
		{
			DisplayAlert("Duplicate", "Employee already exists or Username already exists", "Ok");
		}


	}
}