
using Attendance.Data;


namespace Attendance.Pages;

public partial class Attendance2 : ContentPage
{

public	DataClass dt = new DataClass();
public Dictionary<string, List<Dictionary<string, string>>> dates = new Dictionary<string, List<Dictionary<string, string>>>();	
 public List<string> all_employee_id=new List<string>();
public Dictionary<string, List<Tuple<string, string>>> clock_times = new Dictionary<string, List<Tuple<string, string>>>();
public string state = String.Empty;

	public Attendance2()
	{
		InitializeComponent();
		actind.IsVisible = true;
		
			
			
				List<string> list = new List<string>();
				dt.start_connection();
				
				list= dt.get_all_states_from_employees();

				dt.close_connection();

				list.Add("All");

		       xlbtn.IsEnabled = false;
		xlbtn.IsVisible = false;
				statepicker.ItemsSource = list;
		statepicker.SelectedIndex = 0;
				actind.IsVisible = false;

			
		

		
	}

	public Attendance2(string state)
	{
		InitializeComponent();
		actind.IsVisible = true;



		List<string> list = new List<string> {state };
	

		

		xlbtn.IsEnabled = false;
		xlbtn.IsVisible = false;
		statepicker.ItemsSource = list;
		statepicker.SelectedIndex = 0;
		actind.IsVisible = false;





	}


	public async  void get_attendance()
	{
		if(datepicker_start.Date>datepicker_end.Date)
		{
			await MainThread.InvokeOnMainThreadAsync(() => {
				DisplayAlert("Incorrect Date Range", "end date should be greater than start date", "ok");
			     actind.IsVisible = false;
			});
			return;
		}

		if(statepicker.SelectedIndex==-1)
		{
			await MainThread.InvokeOnMainThreadAsync(() => { DisplayAlert("Select State", "Please select a state", "ok"); actind.IsVisible = false; });
		

			return;
		}


		state = (string)statepicker.ItemsSource[statepicker.SelectedIndex];
		dt.start_connection();
		
		all_employee_id = dt.get_all_employee_id_in_a_specific_state(state);
		Dictionary<string, List<string>> approved_leaves = dt.get_all_employee_approved_leave_dates(null);

		while (datepicker_start.Date<=datepicker_end.Date)
		{
			string appdate = datepicker_start.Date.ToString("dd-MM-yyyy");
			string sqldate = datepicker_start.Date.ToString("yyyy-MM-dd");

			foreach (string empid in  all_employee_id)
			{
				
				
				List<string>present_emp_ids=dt.employee_ids_who_were_present_on_specific_day(sqldate);

				clock_times = dt.get_employee_clock_in_and_out_times_on_a_specific_day(state, sqldate);


				if(dates.ContainsKey(appdate))
				{
					Dictionary<string, string> emp = dt.get_employee_details_with_column_names(empid);
					emp["date"]=appdate;
					
					if(present_emp_ids!=null && present_emp_ids.Contains(empid))
					 {
						emp["present"] = "yes";
					 }
					 else
					{
						emp["present"] = "no";
					}

					if(approved_leaves!=null && approved_leaves.ContainsKey(empid) && approved_leaves[empid].Contains(appdate))
					{
						if (emp["present"]=="yes")
						{
							emp["present_on_leave"] = "yes";
						}
						else
						{
							emp["present_on_leave"] = "no";
						}

						emp["on_leave"] = "yes";
					}
					else
					{
						emp["on_leave"] = "no";
					}

					   if(clock_times.ContainsKey(empid))
					{
						try
						{
							if (clock_times[empid].Count > 0)
							{
								int[] time_arr = dt.get_todays_elapsed_time(empid, sqldate);
								string work_time = dt.convert_elapsed_time_to_string(time_arr);

								emp["clock_time"] = clock_times[empid][0].Item1 + " to " + clock_times[empid][(clock_times[empid].Count - 1)].Item2 + ")\n" + "Work Time : " + work_time;
							   
							}
							else
							{
								int[] time_arr = dt.get_todays_elapsed_time(empid, sqldate);
								string work_time = dt.convert_elapsed_time_to_string(time_arr);
								
								emp["clock_time"] = clock_times[empid][0].Item1 + " to " + clock_times[empid][(clock_times[empid].Count)].Item2 + ")\n" + "Work Time : " + work_time;
							}


						}
						catch(Exception ex)
						{
							int[] time_arr = dt.get_todays_elapsed_time(empid, sqldate);
							string work_time = dt.convert_elapsed_time_to_string(time_arr);
							emp["clock_time"] = ") \nWorktime : " + work_time;
						}
					}
					else
					{
						int[] time_arr = dt.get_todays_elapsed_time(empid, sqldate);
						string work_time = dt.convert_elapsed_time_to_string(time_arr);
						emp["clock_time"] = ") \nWorktime : " + work_time;
					}

					dates[appdate].Add(emp);
				}
				else
				{
					dates[appdate]=new List<Dictionary<string,string>>();
					Dictionary<string, string> emp = dt.get_employee_details_with_column_names(empid);
					emp["date"] = appdate;

					if (present_emp_ids.Contains(empid))
					{
						emp["present"] = "yes";
					}
					else
					{
						emp["present"] = "no";
					}


					if (clock_times.ContainsKey(empid))
					{
						try
						{
							if (clock_times[empid].Count > 0)
							{
								int[] time_arr = dt.get_todays_elapsed_time(empid, sqldate);
								string work_time = dt.convert_elapsed_time_to_string(time_arr);
								emp["clock_time"] = clock_times[empid][0].Item1 + " to " + clock_times[empid][(clock_times[empid].Count - 1)].Item2+")\n"+"Work Time : "+work_time;
							
							}
							else
							{
								int[] time_arr = dt.get_todays_elapsed_time(empid, sqldate);
								string work_time = dt.convert_elapsed_time_to_string(time_arr);
								emp["clock_time"] = clock_times[empid][0].Item1 + " to " + clock_times[empid][(clock_times[empid].Count)].Item2+")\n"+"Work Time : "+work_time;
							}
						
						}
						catch (Exception ex)
						{
							int[] time_arr = dt.get_todays_elapsed_time(empid, sqldate);
							string work_time = dt.convert_elapsed_time_to_string(time_arr);
							emp["clock_time"] = ") \nWorktime : "+work_time;
						}
					}
					else
					{
						int[] time_arr = dt.get_todays_elapsed_time(empid, sqldate);
						string work_time = dt.convert_elapsed_time_to_string(time_arr);
						emp["clock_time"] = ") \nWorktime : " + work_time;
					}


					dates[appdate].Add(emp);

				} 

			}

		 await	MainThread.InvokeOnMainThreadAsync(() => { datepicker_start.Date = datepicker_start.Date.AddDays(1); });
			  
		}

		dt.close_connection();


		string[] table_header_dates = dates.Keys.ToArray();

		List<string> table_header_dates_list = new List<string> { "employee id", "first name", "last name", "store name", "area", "city", "state" };
		
		table_header_dates_list.AddRange( table_header_dates.ToList());

		



		string[,] table_coloumns = new string[all_employee_id.Count, 5];
		string[,] table_row_arr=new string[all_employee_id.Count,table_header_dates_list.Count];

		for(int i=0;i<table_coloumns.GetLength(0);i++)
		{
			for(int j=0;j<table_coloumns.GetLength(1);j++)
			{
				table_coloumns[i,j] = "0";
			}
		}


		Dictionary<string, List<Dictionary<string, string>>> emp_attendance = new Dictionary<string, List<Dictionary<string, string>>>();

		for(int i=0;i<all_employee_id.Count;i++)
		{
			table_row_arr[i,0] = all_employee_id[i];
		}	

		for(int i=7;i<table_header_dates_list.Count;i++)
		{
			for(int j=0;j<dates[table_header_dates_list[i]].Count;j++)
			{
				for(int k=0;k<table_row_arr.GetLength(0);k++) 
				{
					if (dates[table_header_dates_list[i]][j]["emp_id"] == table_row_arr[k,0])
					{
						table_row_arr[k, 1] = dates[table_header_dates_list[i]][j]["firstname"];
						table_row_arr[k, 2] = dates[table_header_dates_list[i]][j]["lastname"];
						table_row_arr[k, 3] = dates[table_header_dates_list[i]][j]["store_name"];
						table_row_arr[k, 4] = dates[table_header_dates_list[i]][j]["area"];
						table_row_arr[k, 5] = dates[table_header_dates_list[i]][j]["city"];
						table_row_arr[k, 6] = dates[table_header_dates_list[i]][j]["state"];

						 if(dates[table_header_dates_list[i]][j].ContainsKey("on_leave") &&  dates[table_header_dates_list[i]][j]["on_leave"]=="yes")
						{
							table_row_arr[k, i] = "on leave";
							int val = Convert.ToInt32(table_coloumns[k, 2]);
							val++;
							table_coloumns[k, 2] = val.ToString();
							
						}
						 else
						{
							DateTime tempdt = DateTime.ParseExact(table_header_dates_list[i],"dd-MM-yyyy",null);

							if (tempdt.Date.DayOfWeek.ToString().ToUpper().Equals(dates[table_header_dates_list[i]][j]["weekoff_day"].ToUpper()) )
							{
								if (dates[table_header_dates_list[i]][j]["present"] == "yes")
								{
									int val = Convert.ToInt32(table_coloumns[k, 0]);
									val++;
									table_coloumns[k,0] = val.ToString();

									table_row_arr[k, i] = String.Format("present on weekoffday ({0})", dates[table_header_dates_list[i]][j]["clock_time"]);
								}
								else
								{
									table_row_arr[k, i] = "weekoff_day";

									int val = Convert.ToInt32(table_coloumns[k, 1]);
									val++;
									table_coloumns[k, 1] = val.ToString();

								}

							}
							else
							{
								table_row_arr[k, i] = dates[table_header_dates_list[i]][j]["present"];

								if (table_row_arr[k,i]=="yes")
								{
									int val = Convert.ToInt32(table_coloumns[k, 0]);
									val++;
									table_coloumns[k, 0] = val.ToString();

									table_row_arr[k, i] = String.Format("present ( {0} )", dates[table_header_dates_list[i]][j]["clock_time"]);

								}
								else if(table_row_arr[k, i] == "no")
								{
									int val = Convert.ToInt32(table_coloumns[k, 3]);
									val++;
									table_coloumns[k, 3] = val.ToString();
								}

							}
						}

						
					}
				}

				

			}
		}

		List<List<string>> table_rows = new List<List<string>>();

		for(int i=0;i<table_row_arr.GetLength(0);i++)
		{
			List<string> temp = new List<string>();
			for(int j=0;j<table_row_arr.GetLength(1);j++)
			{
				temp.Add(table_row_arr[i, j]);
			}

			 for(int j=0;j<5;j++) 
			 {
				if(j==4)
				{
					temp.Add(table_coloumns[i, 0]);
				}

				else temp.Add(table_coloumns[i,j]);
			 }

			table_rows.Add(temp);
		}

		List<string> additional_table_header_list = new List<string> { "present days","weekoff","leave days","absent days","total present days" };

		table_header_dates_list.AddRange(additional_table_header_list);

		string html_string = dt.create_html_string(table_header_dates_list, table_rows,true,clock_times);

		
	



		await MainThread.InvokeOnMainThreadAsync(() => { 
			actind.IsVisible = false; wview.Source = new HtmlWebViewSource { Html = html_string }; 
			xlbtn.IsVisible= true;
			xlbtn.IsEnabled= true;
			xlbtn.Clicked += (async (object sender, EventArgs e) => {

				string path = AppDomain.CurrentDomain.BaseDirectory;

				var destination = System.IO.Path.Combine(path, "report.html");

#if WINDOWS

			destination = @"C:\Users\Public\Documents\report.html";

#endif

				File.WriteAllText(destination, html_string);
#if WINDOWS
			try
			{
				System.Diagnostics.Process.Start(@"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe", destination);
			}
			catch (Exception ex)
			{
				DisplayAlert("Error", ex.Message.ToString(), "Ok");
			}
			return;

#endif
				await Launcher.Default.OpenAsync(new OpenFileRequest("Download Excel from Web Browser", new ReadOnlyFile(destination)));


			});

		} );  
		 




	}

	private async void abtn_Clicked(object sender, EventArgs e)
	{

		

                actind.IsVisible = true;


	await	Task.Run(() => {


			//MainThread.InvokeOnMainThreadAsync(() => {

                get_attendance();



           // });
		
		
		
		});
	 
		 
	 
	}

	public void create_clock_in_and_out_sessions(List<Tuple<string,string>> sessions,string employee_id)
	{
		string htmlstring = String.Format("<html> <body>");

		foreach(var time in sessions)
		{
			htmlstring += string.Format("<h4>{0} to {1} </h4> ", time.Item1, time.Item2);
		}

		htmlstring += String.Format("</body> </html>");


		string path = AppDomain.CurrentDomain.BaseDirectory;

		var destination = System.IO.Path.Combine(path,employee_id+"_session_report.html");



	}



	private void abtn_Clicked_1(object sender, EventArgs e)
	{

	}

	private void xlbtn_Clicked(object sender, EventArgs e)
	{

	}
}