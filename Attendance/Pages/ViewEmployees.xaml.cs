using Attendance.Data;
using Microsoft.Maui.Controls.PlatformConfiguration;
using MySqlConnector;
using System;

namespace Attendance.Pages;

public partial class ViewEmployees : ContentPage
{
	public List<Frame> Frames = new List<Frame>();
	public List<List<string>> emp_details = new List<List<string>>();	
	public DataClass dt = new DataClass();
	public  ViewEmployees()
	{
		InitializeComponent();
		Task.Run(() => { get_employee_details(); });
		
	}

	public ViewEmployees(string emp_id)
	{
		InitializeComponent();
		Task.Run(() => { get_employee_details(emp_id); });
	}

	public async void get_employee_details()
	{
		 dt.start_connection();
		 emp_details= dt.get_employee_details();

		if (emp_details != null)
		{
			for (int i = 0; i < emp_details.Count; i++)
			{
				int target = 0;
				int rem_sales = dt.get_remaining_sales_needed_to_reach_target(ref target, emp_details[i][0]);

				if (rem_sales > 0)
				{
					emp_details[i].Insert(20,rem_sales.ToString());
				}
				else
				{
					emp_details[i].Insert(20, rem_sales.ToString());
				}

				int this_month_sale = 0;

				try
				{
				 this_month_sale=	Convert.ToInt32(emp_details[i][12]);
				}
				catch(Exception ex)
				{
					this_month_sale = 0;
				}
				this_month_sale -= rem_sales;

					


				emp_details[i].Insert(21,this_month_sale.ToString());

			}

			dt.close_connection();
			string html = create_html_string(emp_details);
			MainThread.InvokeOnMainThreadAsync(async () =>
			{

				vs.Clear();

				VerticalStackLayout innervs_stack = new VerticalStackLayout();

				innervs_stack.Add(new WebView { Source = new HtmlWebViewSource { Html = html } });
				


				Button xlbutton = new Button { Text = "Generate Excel" };

				xlbutton.Clicked += (async (object sender, EventArgs e) =>
				{

					string path = AppDomain.CurrentDomain.BaseDirectory;



					var destination = System.IO.Path.Combine(path, "report.html");


#if WINDOWS
        
   destination= @"C:\Users\Public\Documents\report.html";
            
#endif



					try
					{


						File.WriteAllText(destination, html);
					}
					catch(Exception ex)
					{
						DisplayAlert("Error", ex.Message.ToString(),"Ok");
					}
#if WINDOWS
              try
			  {
             System.Diagnostics.Process.Start(@"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",destination);
                 }
				 catch(Exception ex)
				 {
				 DisplayAlert("Error", ex.Message.ToString(),"Ok");
				 }
			 return;

#endif

					await Launcher.Default.OpenAsync(new OpenFileRequest("Download Excel from Web Browser", new ReadOnlyFile(destination)));
				     

				});

				innervs_stack.Add(xlbutton);

				vs.Add(new ScrollView { Content = innervs_stack });


			});
		}
		else {
			dt.close_connection();
			Navigation.PopAsync();
		}
	}

	public async void get_employee_details(string emp_id)
	{
		dt.start_connection();
		emp_details = dt.get_employee_details_from_a_state(dt.get_current_employee_state(emp_id));

		if (emp_details != null)
		{
			for (int i = 0; i < emp_details.Count; i++)
			{
				int target = 0;
				int rem_sales = dt.get_remaining_sales_needed_to_reach_target(ref target, emp_details[i][0]);

				if (rem_sales > 0)
				{
					emp_details[i].Insert(20, rem_sales.ToString());
				}
				else
				{
					emp_details[i].Insert(20, rem_sales.ToString());
				}

				int this_month_sale = Convert.ToInt32(emp_details[i][12]);

				this_month_sale -= rem_sales;




				emp_details[i].Insert(21, this_month_sale.ToString());

			}

			dt.close_connection();
			string html = create_html_string(emp_details);
			MainThread.InvokeOnMainThreadAsync(async () =>
			{

				vs.Clear();

				VerticalStackLayout innervs_stack = new VerticalStackLayout();

				innervs_stack.Add(new WebView { Source = new HtmlWebViewSource { Html = html } });



				Button xlbutton = new Button { Text = "Generate Excel" };

				xlbutton.Clicked += (async (object sender, EventArgs e) =>
				{

					string path = AppDomain.CurrentDomain.BaseDirectory;



					var destination = System.IO.Path.Combine(path, "report.html");


#if WINDOWS
        
   destination= @"C:\Users\Public\Documents\report.html";
            
#endif



					try
					{


						File.WriteAllText(destination, html);
					}
					catch (Exception ex)
					{
						DisplayAlert("Error", ex.Message.ToString(), "Ok");
					}
#if WINDOWS
              try
			  {
             System.Diagnostics.Process.Start(@"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",destination);
                 }
				 catch(Exception ex)
				 {
				 DisplayAlert("Error", ex.Message.ToString(),"Ok");
				 }
			 return;

#endif

					await Launcher.Default.OpenAsync(new OpenFileRequest("Download Excel from Web Browser", new ReadOnlyFile(destination)));


				});

				innervs_stack.Add(xlbutton);

				vs.Add(new ScrollView { Content = innervs_stack });


			});
		}
		else
		{
			dt.close_connection();
			Navigation.PopAsync();
		}
	}



	public string create_html_string(List<List<string>> rows)
	{
		string htmlstring = "<html> <body> <table border='1' id='table'> <thead>   <tr bgcolor=#D3D3D3>  <th> Employee ID </th> <th> First Name </th> <th>  Last Name </th> <th> Age </th> <th> Join Date </th> <th> City </th> <th> State </th> <th> Store Name </th> <th> DOB </th> <th> Contact </th>  <th> Bank Account No </th>  <th> IFSC Code </th> <th> Monthly Target </th> <th> WeekOff Day </th> <th> Name As Per Bank Account </th> <th> Area </th> <th> UserName </th> <th> IsAccount Active </th> <th> Remaining Sales to reach monthly target </th> <th>This Month Sales</th> <th>Zone</th>  </tr> </thead> <tbody> ";

		int sum = 0;

		for (int i = 0; i < rows.Count; i++)
		{
			htmlstring += "<tr>";

			
			for(int j = 0; j < rows[i].Count;j++)
			{
				if (j == rows[i].Count-5 || j == rows[i].Count-6)
				{
					continue;
				}

		       if(j == 4 || j==8)
				{
					string[] onlydate = rows[i][j].Split(' ');

					htmlstring += String.Format("<td>{0}</td>", onlydate[0]);
					continue;
				}

			   if(j==21)
				{
					int month_target = 0;
					try
					{
						month_target= Convert.ToInt32(rows[i][12]);
					}
					catch(Exception ex)
					{
						month_target = 0;
					}
					int current_sale = 0;

					try
					{
					 current_sale=	Convert.ToInt32(rows[i][21]);
					}
					catch(Exception ex)
					{
						current_sale = 0;
					}

					if(current_sale>month_target)
					{
						htmlstring += String.Format("<td bgcolor=#008000>{0}</td>", rows[i][j]);
						continue;
					}

					htmlstring += String.Format("<td>{0}</td>", rows[i][j]);
					continue;

				}

				htmlstring += String.Format("<td>{0}</td>", rows[i][j]);

			}

			

			htmlstring += "</tr>";

		}

		htmlstring += String.Format( "</tbody> </table> {0} </body></html>",dt.get_js2excel_script());

		return htmlstring;



	}





	public  void connect_to_db_and_get_employees()
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