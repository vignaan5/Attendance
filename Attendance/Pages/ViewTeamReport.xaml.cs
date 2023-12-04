
using Attendance.Data;

namespace Attendance.Pages;

public partial class ViewTeamReport : ContentPage
{
	Dictionary<string, Dictionary<string,List<Dictionary<string,string>>>> stores = new Dictionary<string, Dictionary<string, List<Dictionary<string, string>>>>();
    public List<string> all_States  = new List<string>();
	public	Dictionary<string, Dictionary<string, string>> state_report = new Dictionary<string, Dictionary<string, string>>();
	public DataClass dt = new DataClass();
	public ViewTeamReport()
	{
		InitializeComponent();
		get_state_list();

		DateTime temp = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
		dtstart.Date = temp.Date;
	}

	public ViewTeamReport(string state)
	{
		InitializeComponent();


		all_States.Add(state);
		stpicker.ItemsSource = all_States;

		DateTime temp = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
		dtstart.Date = temp.Date;
		
	}

	public ViewTeamReport(List<string> states)
	{
		InitializeComponent();


		
		stpicker.ItemsSource = states;

		DateTime temp = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
		dtstart.Date = temp.Date;

	}


	public async void get_state_list()
	{
		dt.start_connection();
		all_States = dt.get_all_states_from_employees();
		dt.close_connection();
		all_States.Add("All");
		stpicker.ItemsSource=all_States;
	}



	public async void get_store_list()
	{
		dt.start_connection();
	 stores=	dt.get_all_store_details();
		dt.close_connection();

		List<string> states=new List<string>();
		foreach(var state in stores)
		{
			states.Add(state.Key);
		}

		stpicker.ItemsSource= states;
	}

	string create_sql_date(DatePicker dp) 
	{
		string sql_string_start_day = dtstart.Date.Year.ToString() + "-" + dtstart.Date.Month.ToString() + "-" + dtstart.Date.Day.ToString();

		return sql_string_start_day;
	}

	public string create_html_string_for_employee_daily_sale(string emp_id,string sql_date,string total_sales)
	{
		if (sql_date == "total")
			return "";
		List<List<string>> emp_sales = dt.get_employee_sales_between_two_dates(sql_date,sql_date,emp_id);
		string html_sales_str = String.Format("<html> <body> <h1>{0} sold products on {1} </h1> <table border='1'><thead><tr bgcolor=#D3D3D3> <th>Particulars</th> <th>Mrp</th> <th> PCS sold </th> <th> Amount </th> <th> Date </th> </thead> <tbody> ",emp_id,sql_date);
		for(int i=0;i<emp_sales.Count;i++)
		{
			html_sales_str += String.Format("<tr> <td>{0}  </td> <td> {1} </td>  <td> {2} </td> <td> {3} </td> </tr> ", emp_sales[i][8], emp_sales[i][10], emp_sales[i][2], emp_sales[i][3], emp_sales[i][5]);

		}

		html_sales_str += String.Format("</tbody> </table> <h3>total sales = {0} </h3> </body> </html>",total_sales);

		string path = AppDomain.CurrentDomain.BaseDirectory;

		var destination = System.IO.Path.Combine(path, emp_id+" "+sql_date+".html");
#if WINDOWS

			destination = @"C:\Users\Public\Documents\"+emp_id+" "+sql_date+".html";

#endif

		File.WriteAllText(destination, html_sales_str);


		return destination;
	}


	public string create_html_string()
	{
		string htmlstring = "<html> <body> <table border='1'><thead><tr bgcolor=#D3D3D3> <th>emp_id</th><th>first name</th> <th> last name </th> <th>store name</th> <th>area</th> <th> monthly target </th> ";


		int sum = 0;

		foreach (var dates in state_report.ElementAt(0).Value)
		{

				htmlstring += String.Format("<th>{0}</th>", dates.Key);
			
		}

		htmlstring += "</thead></tr><tbody>";

		dt.start_connection();

		Dictionary<string, int> dates_total = new Dictionary<string, int>();


		foreach (var employee in state_report)
		{
			Dictionary<string, string> emp_details = dt.get_employee_details_with_column_names(employee.Key);

			htmlstring += String.Format("<tr> <td> {0} </td> <td>{1}</td> <td>{2}</td> <td>{3}</td> <td>{4}</td> <td>{5} </td>", employee.Key, emp_details["firstname"], emp_details["lastname"],emp_details["store_name"], emp_details["area"], emp_details["monthly_target"]);

			
			foreach(var date in employee.Value)
			{
				  if(date.Value!="0")
				htmlstring += String.Format("<td><a href='{0}'>{1} </a> </td>",create_html_string_for_employee_daily_sale(employee.Key,date.Key,date.Value) ,date.Value);
				else htmlstring += String.Format("<td> {0} </td>",date.Value);

				if(dates_total.ContainsKey(date.Key))
				{
					dates_total[date.Key]+=Convert.ToInt32(date.Value);
				}
				else
				{
					dates_total[date.Key] = 0;
				}
			}

			htmlstring += " </tr> ";
		}
		dt.close_connection();

		htmlstring += String.Format("<tr bgcolor=#D3D3D3><td> </td> <td></td> <td> </td> <td> </td> <td> </td> <td> total sales </td> ");

		foreach(var date in dates_total)
		{
			htmlstring += String.Format("<td>{0}</td>", date.Value);
		}

		htmlstring += "</tr>";

		return htmlstring+=String.Format("</tbody></table>{0}</body></html>",dt.get_js2excel_script());
	}




	private async void get_state_report(object sender,EventArgs e)
	{
		if(stpicker.SelectedIndex==-1)
		{
			DisplayAlert("No State Selected","Please Select a state","Continue");
			return;
		}


		DatePicker safe_start_date = new DatePicker();
		safe_start_date.Date = dtstart.Date;

		actind.IsVisible = true;

		await Task.Run(async () => {

			string sql_string_start_day = dtstart.Date.Year.ToString() + "-" + dtstart.Date.Month.ToString() + "-" + dtstart.Date.Day.ToString();
			string sql_string_end_day = dtend.Date.Year.ToString() + "-" + dtend.Date.Month.ToString() + "-" + dtend.Date.Day.ToString();



			int total = 0;
			
			dt.start_connection();

			List<string> all_emp_id_list = new List<string>();

			all_emp_id_list = dt.get_all_employee_id_in_a_specific_state(stpicker.ItemsSource[stpicker.SelectedIndex].ToString());

			dt.close_connection();

			dt.start_connection();

			for (int i = 0; i < all_emp_id_list.Count; i++)
			{

				string temp_emp_id = all_emp_id_list[i];

				int total_emp_sum = 0;

				Dictionary<string, string> sales_on_each_day = new Dictionary<string, string>();

				while (dtstart.Date <= dtend.Date)
				{
					string temp_sql_date = create_sql_date(dtstart);

					
					int amount = dt.get_employee_sales_on_a_specific_day(temp_sql_date, stpicker.ItemsSource[stpicker.SelectedIndex].ToString(), temp_emp_id);
				

					sales_on_each_day[temp_sql_date] = amount.ToString();

					total_emp_sum += amount;

				await	MainThread.InvokeOnMainThreadAsync(() => {
						DateTime tempdt = new DateTime();

						tempdt = Convert.ToDateTime(dtstart.Date);

						dtstart.Date = tempdt.AddDays(1);
					statusempid.IsVisible = true;
					statusempid.Text = "Caluculating sales of Employee  " + all_emp_id_list[i] + " ";

					});


				


				}

				
				sales_on_each_day["total"] = total_emp_sum.ToString();

				state_report[all_emp_id_list[i]] = sales_on_each_day;

			 await	MainThread.InvokeOnMainThreadAsync(() => {

				 dtstart.Date = safe_start_date.Date; ;
				});
				


			}
			dt.close_connection();

		});

	
		actind.IsVisible = false;



      string html_str=		create_html_string();


		vs.Clear();
		VerticalStackLayout innervs = new VerticalStackLayout();

		Button xlbutton = new Button { Text = "Generate Excel", HorizontalOptions = LayoutOptions.Center };

		xlbutton.Clicked += (async (object sender, EventArgs e) => {

			string path = AppDomain.CurrentDomain.BaseDirectory;

			var destination = System.IO.Path.Combine(path, "report.html");
#if WINDOWS

			destination = @"C:\Users\Public\Documents\report.html";

#endif

			File.WriteAllText(destination, html_str);
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

		innervs.Add(new WebView { Source = new HtmlWebViewSource { Html = html_str } });
		innervs.Add(xlbutton);

		vs.Add(new ScrollView { Content=innervs });
	
	}

}