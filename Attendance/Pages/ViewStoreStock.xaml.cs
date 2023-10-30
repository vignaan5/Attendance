using Attendance.Data;
using Microsoft.Maui.ApplicationModel;

namespace Attendance.Pages;

public partial class ViewStoreStock : ContentPage
{
	public DataClass dt = new DataClass();
	public Button xlbtn = new Button { Text = "Generate Excel",HorizontalOptions=LayoutOptions.Center };
	public List<string> nothing_found_temp = new List<string> { "Nothing Found" };
	public string emp_id = String.Empty;
	bool dont_trigger_emp_search_text_event = false;
	public ViewStoreStock()
	{
		InitializeComponent();
		DateTime date_time_today = DateTime.Now;
		List<string> years = new List<string>();

		for(int i=2023;i<=date_time_today.Year;i++)
		{
			years.Add(i.ToString());
		}

		year_picker.ItemsSource=years;

		year_picker.SelectedIndex=(year_picker.Items.Count-1);

		month_picker.SelectedIndex = (date_time_today.Month - 1);

		get_stock_report.Clicked += get_stock_report_Clicked;

	}



	public ViewStoreStock(bool search_for_employee)
	{
		InitializeComponent();
		DateTime date_time_today = DateTime.Now;
		List<string> years = new List<string>();

		for (int i = 2023; i <= date_time_today.Year; i++)
		{
			years.Add(i.ToString());
		}

		year_picker.ItemsSource = years;

		year_picker.SelectedIndex = (year_picker.Items.Count - 1);

		month_picker.SelectedIndex = (date_time_today.Month - 1);

		svs.IsVisible = true;
		svs.IsEnabled = true;

		get_stock_report.IsEnabled = false;

		get_stock_report.IsVisible = false;


		get_stock_report.Clicked += get_stock_report_Clicked2;

	}


	public static DateTime StartOfMonth( DateTime date)
	{
		return new DateTime(date.Year, date.Month, 1, 0, 0, 0);
	}

	public static DateTime EndOfMonth( DateTime date)
	{
		
		return StartOfMonth(date).AddMonths(1).AddSeconds(-1);
	}




	private async void get_stock_report_Clicked(object sender, EventArgs e)
	{
		if (vs.Contains(xlbtn))
			vs.Remove(xlbtn);

		string start_dt = String.Format("01/{0}/{1}",month_picker.SelectedIndex+1,year_picker.SelectedItem as string);
		DateTime start_date = DateTime.ParseExact(start_dt, "dd/M/yyyy", null);

		DateTime end_date = EndOfMonth(start_date);


		await dt.get_emp_id();
		int daily_sale = 0;

		dt.start_connection();
	 string state =	dt.get_current_employee_state();
		//List<List<string>> stock_info = dt.get_store_stock(start_date.ToString("yyyy-MM-dd"), end_date.ToString("yyyy-MM-dd"), ref daily_sale, state);
		List<List<string>> stock_info = dt.get_opening_and_closing_stock();
		dt.close_connection();

		List<string> dsalelis = new List<string> {"","","","","","","","","","Total Stock Value",daily_sale.ToString() };
		stock_info.Add(dsalelis);

		List<string> stock_header = new List<string> { "Sno", "paticulars", "HSN_SAC", "MRP", "opening_stock", "qty", "Sales", "sold_pcs", "return_sales", "Defective", "Closing_stock" };

		string html_str = dt.create_html_string(stock_header, stock_info);

		wview = new WebView { Source = new HtmlWebViewSource { Html = html_str } };
		wview.IsEnabled = true;
		wview.IsVisible=true;

		xlbtn.Clicked += (async(object sender,EventArgs e) => 
		{

			dt.Excel_Function(html_str, "report.html");
		
		});


		vs.Add(xlbtn);


	}


	private async void get_stock_report_Clicked2(object sender, EventArgs e)
	{
		if (vs.Contains(xlbtn))
			vs.Remove(xlbtn);

		string start_dt = String.Format("01/{0}/{1}", month_picker.SelectedIndex + 1, year_picker.SelectedItem as string);
		DateTime start_date = DateTime.ParseExact(start_dt, "dd/M/yyyy", null);

		DateTime end_date = EndOfMonth(start_date);


		await dt.get_emp_id();
		int daily_sale = 0;

		dt.start_connection();
		string state = dt.get_current_employee_state();
		//List<List<string>> stock_info = dt.get_store_stock(start_date.ToString("yyyy-MM-dd"), end_date.ToString("yyyy-MM-dd"), ref daily_sale, state);
		List<List<string>> stock_info = dt.get_opening_and_closing_stock(emp_id);
		dt.close_connection();

		List<string> dsalelis = new List<string> { "", "", "", "", "", "", "", "", "", "Total Stock Value", daily_sale.ToString() };
		stock_info.Add(dsalelis);

		List<string> stock_header = new List<string> { "Sno", "paticulars", "HSN_SAC", "MRP", "opening_stock", "qty", "Sales", "sold_pcs", "return_sales", "Defective", "Closing_stock" };

		string html_str = dt.create_html_string(stock_header, stock_info);

		wview = new WebView { Source = new HtmlWebViewSource { Html = html_str } };
		wview.IsEnabled = true;
		wview.IsVisible = true;

		xlbtn.Clicked += (async (object sender, EventArgs e) =>
		{

			dt.Excel_Function(html_str, "report.html");

		});


		vs.Add(xlbtn);


	}


	private void emplis_ItemSelected(object sender, SelectedItemChangedEventArgs e)
	{
		if ((string)emplist.SelectedItem == "Nothing Found")
			return;

		this.emp_id = (string)emplist.SelectedItem;
		this.emp_id = this.emp_id.Split(' ')[0];

		emplist.IsVisible = false;

		get_stock_report.IsVisible = true;

		get_stock_report.IsEnabled = true;

		dont_trigger_emp_search_text_event = true;

		search_emp.Text = this.emp_id;

		dont_trigger_emp_search_text_event = false;

	
	}

	private void search_emp_TextChanged(object sender, TextChangedEventArgs e)
	{

		if (search_emp.Text.Trim() == "")
		{

			emplist.ItemsSource = nothing_found_temp;
			return;
		}

		if (dont_trigger_emp_search_text_event)
			return;

		Task.Run(async () => {

			while (dt.is_conn_open)
			{

			}

			dt.start_connection();
			List<List<string>> result = dt.search_employee_in_db(search_emp.Text.Trim(), search_emp.Text.Trim(), search_emp.Text.Trim());
			dt.close_connection();

			MainThread.InvokeOnMainThreadAsync(() => {
				List<string> temp = new List<string>();
				foreach (List<string> list in result)
				{
					temp.Add(list[0] + " " + list[1] + " " + list[2]);

				}
				if (temp.Count == 0)
				{
					emplist.ItemsSource = nothing_found_temp;
				}
				else
				{
					emplist.ItemsSource = temp;
				}

				emplist.IsVisible = true;
			});


		});




	}







	private async void goback_btn_Clicked(object sender, EventArgs e)
	{
	

	 await	Navigation.PopToRootAsync();

	}
}