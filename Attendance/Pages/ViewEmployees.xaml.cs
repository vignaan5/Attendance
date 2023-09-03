using MySqlConnector;
using System;

namespace Attendance.Pages;

public partial class ViewEmployees : ContentPage
{
	public List<Frame> Frames = new List<Frame>();
	public  ViewEmployees()
	{
		InitializeComponent();
		create_ui_for_employees();
	}

	public  void connect_to_db_and_get_employees()
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

		string sql_cmd_string1 = " select * from employee ";

		MySqlCommand mysqlcmd = new MySqlCommand(sql_cmd_string1,conn);

		mysqlcmd.ExecuteNonQuery();

		MySqlDataReader reader = mysqlcmd.ExecuteReader();	

		while(!reader.IsClosed && reader.Read())
		{
			Frame frame = new Frame();
			frame.MinimumHeightRequest = 600;
			
			frame.MinimumHeightRequest = 600;
            
		

			Label l1 = new Label();

			l1.Text = "First Name : " + reader[1].ToString();



			Label l2 = new Label();

			l2.Text = "Last Name : " + reader[2].ToString();

			Label l3 = new Label();

			l3.Text = "Employee ID : " + reader[0].ToString();

			Label l4 = new Label();

			l4.Text = "Age : " + reader[3].ToString();

			Label l5 = new Label();

			l5.Text = "Join Date : " + reader[4].ToString();

			Label l6 = new Label();
			l6.Text = "Last Logged-in Details  : ";

			Label l7 = new Label();

			l7.Text = "Last Logged in Date : " + reader[6].ToString();

			Label l8 = new Label();

			l8.Text = "Latitude : " + reader[17].ToString();

			Label l9 = new Label();

			l9.Text = "Longitude : " + reader[18].ToString();

			VerticalStackLayout tempvs = new VerticalStackLayout();

			tempvs.Add(l1);
			tempvs.Add(l2);
			tempvs.Add(l3);
			tempvs.Add(l4);
			tempvs.Add(l5);
			tempvs.Add(l6);
				tempvs.Add(l7);
			tempvs.Add(l8);

			tempvs.Add(l9);

			Button gmapbtn = new Button();

			gmapbtn.Text = "View On Maps";

			string[] rlatitude=l8.Text.Split(':');
			string[] rlongitude=l9.Text.Split(":");

			gmapbtn.Clicked += (async (object sender,EventArgs e) => { await OpenMaps(rlatitude[1], rlongitude[1] );  });

			tempvs.Add(gmapbtn);

			frame.Content= tempvs;

			Frames.Add(frame);

		   

		}

		conn.Close();


		return;
	}


	public async void create_ui_for_employees()
	{
		await Task.Run(() => { connect_to_db_and_get_employees(); });
		
		foreach(Frame f in Frames) 
		{ 
		   vs.Add(f);
		
		}

		actindicator.IsVisible=false;	

	}



	public async Task OpenMaps(string db_latitude,string db_longitude)
	{

		double latitude = Convert.ToDouble(db_latitude);
		double longitude = Convert.ToDouble(db_longitude);
	 Microsoft.Maui.Devices.Sensors.Location l = new Microsoft.Maui.Devices.Sensors.Location(latitude, longitude);


		//string locationName = FeatureName.Text+","+localityLable.Text;

		//string mapsUri = $"geo:{latitude},{longitude}?q={Uri.EscapeDataString(locationName)}";



		try
		{
			//   await Launcher.OpenAsync(new Uri(mapsUri));

			await Map.Default.OpenAsync(latitude, longitude);
		}
		catch (Exception ex)
		{
			// Handle any exceptions if necessary
			Console.WriteLine($"Error opening maps: {ex.Message}");
		}

	}



}