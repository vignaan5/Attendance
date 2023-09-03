using Attendance.Data;

namespace Attendance.Pages;

public partial class ViewTeamReport : ContentPage
{
	Dictionary<string, Dictionary<string,List<Dictionary<string,string>>>> stores = new Dictionary<string, Dictionary<string, List<Dictionary<string, string>>>>();
 public	Dictionary<string, Dictionary<string, string>> state_report = new Dictionary<string, Dictionary<string, string>>();
	public DataClass dt = new DataClass();
	public ViewTeamReport()
	{
		InitializeComponent();
		get_store_list();

		DateTime temp = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
		dtstart.Date = temp.Date;
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


	public string create_html_string()
	{
		string htmlstring = "<html> <body> <table border='1'><tr bgcolor=#D3D3D3> <th>emp_id</th> ";

		int sum = 0;

		foreach (var dates in state_report.ElementAt(0).Value)
		{

				htmlstring += String.Format("<th>{0}</th>", dates.Key);
			
		}

		htmlstring += "</tr>";


		foreach(var employee in state_report)
		{
			htmlstring += String.Format("<tr> <td> {0} </td>", employee.Key);
			foreach(var date in employee.Value)
			{
				htmlstring += String.Format("<td>{0}</td>", date.Value);
			}

			htmlstring += " </tr> ";
		}

	

		return htmlstring;
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

		vs.Add(new WebView { Source = new HtmlWebViewSource { Html=html_str} });
	
	}

}