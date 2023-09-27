using Attendance.Data;
using System.Globalization;

namespace Attendance.Pages;

public partial class EmployeeAttendance : ContentPage
{
	string state = String.Empty;
	DataClass dt = new DataClass();
	public List<List<string>> all_employee_id=new List<List<string>>();
	public Dictionary<string, List<string>> emp_attendance = new Dictionary<string, List<string>>();
	public Dictionary<string, List<string>> emp_week_off = new Dictionary<string, List<string>>();

	public EmployeeAttendance()
	{
		InitializeComponent();
		
	}

	public EmployeeAttendance(string state)
	{
		InitializeComponent();
		this.state=state;
	}

	public string create_html_string(DateTime start,DateTime end)
	{
		DateTime startdate = start;
		string htmlstring = "<html> <body> <table border='1' id='table'><thead><tr bgcolor=#D3D3D3> <th>emp_id</th> <th>first name</th> <th>last name</th> <th>store name</th> <th>area </th> <th> city </th> <th> state </th>" ;

		while(startdate.Date<=end.Date)
		{
			htmlstring += String.Format("<th>{0}</th>", startdate.Date.ToString("dd-MM-yyyy")+" "+startdate.Date.DayOfWeek.ToString());
		    startdate=startdate.AddDays(1);
		}

		htmlstring += "<th> total working days  </th>  </ tr > </ thead > <tbody> ";

		

		for (int employee = 0; employee < all_employee_id.Count; employee++)
		{
			htmlstring += "<tr>";

			htmlstring += String.Format("<td> {0} </td> <td> {1} </td> <td> {2} </td> <td> {3} </td> <td> {4} </td> <td> {5} </td> <td> {6} </td> ", all_employee_id[employee][0], all_employee_id[employee][1], all_employee_id[employee][2], all_employee_id[employee][7], all_employee_id[employee][15], all_employee_id[employee][5], all_employee_id[employee][6]);
			startdate = start;
			int total_working_days = 0;
			 while(startdate.Date<=end)
			{
				if (emp_attendance[startdate.Date.ToString("dd-MM-yyyy")].Contains(all_employee_id[employee][0]))
				{
					if (emp_week_off[startdate.Date.ToString("dd-MM-yyyy")].Contains(all_employee_id[employee][0]))
					{
						htmlstring += "<td bgcolor=#ADD8E6> present on Weekoffday </td>";

					}
					else
					{
						htmlstring += "<td bgcolor=#90EE90> present </td>";
					}
					
					total_working_days++;
				}
				else if(emp_week_off[startdate.Date.ToString("dd-MM-yyyy")].Contains(all_employee_id[employee][0]))
				{
					htmlstring += "<td bgcolor=#FFFFED> Weekoff </td>";

				}
				else
				{
					htmlstring += "<td > </td>";
				}

				startdate = startdate.AddDays(1);
			}

			htmlstring += String.Format("<td>{0}<td>", total_working_days.ToString());

			htmlstring += "</tr>";
		}




		htmlstring += String.Format("</tbody> </table> {0} </body> </html>",dt.get_js2excel_script());


		return htmlstring;
	}


	public void get_employee_list_and_create_attendance_table()
	{
		dt.start_connection();
		all_employee_id=dt.get_all_employee_details_in_a_list();
		dt.close_connection();
	}

	public void get_employee_list_and_create_attendance_table(string state)
	{
		dt.start_connection();
		all_employee_id = dt.get_all_employee_details_in_a_list_of_a_spefic_state(state);
		dt.close_connection();
	}




	private async void Button_Clicked(object sender, EventArgs e)
	{
		actind.IsVisible = true;

		DateTime startdt = DateTime.Now;
		try
		{
			startdt= Convert.ToDateTime(startDate.Date.Day.ToString() + "-" + startDate.Date.Month.ToString() + "-" + startDate.Date.Year.ToString());
		}
		catch(Exception ex) 
		{ 
		
		}
		DateTime enddt = DateTime.Now;
		try
		{
		  enddt =	Convert.ToDateTime(endDate.Date.Day.ToString() + "-" + endDate.Date.Month.ToString() + "-" + endDate.Date.Year.ToString());
		}
		catch(Exception ex) 
		{ 
		
		}
		DateTime start = startdt;


		await Task.Run(async () =>
		{
			if (this.state == String.Empty)
			{

				get_employee_list_and_create_attendance_table();
			}
			else
			{
				get_employee_list_and_create_attendance_table(this.state);
			}
				dt.start_connection();



			while (startdt.Date <= enddt.Date)
			{
				string daytoyear = "";
				try
				{
					daytoyear = startdt.Date.ToString("dd-MM-yyyy");
				}
				catch (Exception ex)
				{

				}

				string yeartoday = "";
				try
				{
					yeartoday = startdt.Date.ToString(startdt.Date.ToString("yyyy-MM-dd"));
				}
				catch (Exception ex)
				{
				}

			  string day_name =	startdt.Date.DayOfWeek.ToString();




				emp_week_off[daytoyear] = dt.employee_ids_who_were_on_weekoff(day_name);
				emp_attendance[daytoyear] = dt.employee_ids_who_were_present_on_specific_day(yeartoday);

				startdt = startdt.Date.AddDays(1);
			}

			dt.close_connection();


		});


		string htmlstr = create_html_string(start, enddt);

		vs.Clear();

		VerticalStackLayout insidevs = new VerticalStackLayout();

		Button xlbutton = new Button { Text = "Generate Excel", HorizontalOptions = LayoutOptions.Center };

		xlbutton.Clicked += (async (object sender, EventArgs e) => {

			string path = AppDomain.CurrentDomain.BaseDirectory;

			var destination = System.IO.Path.Combine(path, "report.html");
#if WINDOWS

			destination = @"C:\Users\Public\Documents\report.html";

#endif

			File.WriteAllText(destination, htmlstr);
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

		insidevs.Add(new WebView { Source = new HtmlWebViewSource { Html = htmlstr } }); ;
		insidevs.Add(xlbutton);
		vs.Add(new ScrollView { Content = insidevs });

	  
		
	}
}