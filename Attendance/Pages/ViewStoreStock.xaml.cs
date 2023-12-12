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
	public string state = String.Empty;
	public static string stores_first_entry_date = "";
	public string zone = String.Empty;
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

		Task.Run(async () => 
		{
			await dt.get_emp_id();
			dt.start_connection();
			DateTime min_date = dt.get_employee_first_stock_entry_date(dt.emp_id2);
			dt.close_connection();

			MainThread.InvokeOnMainThreadAsync(async () => 
			  { 
			         st_date.MinimumDate = min_date;
				  end_date_picker.MinimumDate = min_date;
				  stockhs.IsVisible = true;
				  stockhs.IsEnabled = true;
			   
			 });

		
		});

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

		search_emp.TextChanged += search_emp_TextChanged;

		svs.IsVisible = true;
		svs.IsEnabled = true;

		get_stock_report.IsEnabled = false;

		get_stock_report.IsVisible = false;


		get_stock_report.Clicked += get_stock_report_Clicked2;

	}

	

	public ViewStoreStock(bool search_for_employee,string state)
	{
		InitializeComponent();
		DateTime date_time_today = DateTime.Now;
		List<string> years = new List<string>();

		this.state=state;

		for (int i = 2023; i <= date_time_today.Year; i++)
		{
			years.Add(i.ToString());
		}

		search_emp.TextChanged += search_emp_TextChanged_with_state;

		year_picker.ItemsSource = years;

		year_picker.SelectedIndex = (year_picker.Items.Count - 1);

		month_picker.SelectedIndex = (date_time_today.Month - 1);

		svs.IsVisible = true;
		svs.IsEnabled = true;

		get_stock_report.IsEnabled = false;

		get_stock_report.IsVisible = false;


		get_stock_report.Clicked += get_stock_report_Clicked2;

	}


	public ViewStoreStock(bool search_for_employee, string state,string zone)
	{
		InitializeComponent();
		DateTime date_time_today = DateTime.Now;
		List<string> years = new List<string>();

		this.state = state;
		this.zone = zone;

		for (int i = 2023; i <= date_time_today.Year; i++)
		{
			years.Add(i.ToString());
		}

		search_emp.TextChanged +=search_emp_TextChanged_with_zone;

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
		if (st_date.Date > end_date_picker.Date)
		{
			DisplayAlert("Invalid Date !", "The date ranges are not valid", "OK!");
			return;
		}


		if (vs.Contains(xlbtn))
			vs.Remove(xlbtn);

		string start_dt = String.Format("01/{0}/{1}", month_picker.SelectedIndex + 1, year_picker.SelectedItem as string);
		DateTime start_date = DateTime.ParseExact(start_dt, "dd/M/yyyy", null);

		DateTime end_date = EndOfMonth(start_date);


		await dt.get_emp_id();

		int daily_sale = 0;

		string opening_stock_sum = "0"; string opeing_stock_value_sum = "0";

		string sale_range_sum = "0";

		List<string> invoice_header = new List<string>();
		List<string> fake_invoice_header = new List<string>();
		List<string> invoice_column_sums = new List<string>();
		List<string> fake_invoice_column_sums = new List<string>();
		List<string> closing_stock_sums = new List<string>();
		List<string> dynamic_opening_stock_header = new List<string>();
		dt.start_connection();
		string state = dt.get_current_employee_state();



		//List<List<string>> stock_info = dt.get_store_stock(start_date.ToString("yyyy-MM-dd"), end_date.ToString("yyyy-MM-dd"), ref daily_sale, state);

		List<List<string>> stock_info = dt.get_opening_and_closing_stock(st_date.Date.ToString("yyyy-M-dd"), end_date_picker.Date.ToString("yyyy-M-dd"),ref closing_stock_sums);
		
		List<List<string>> invoice_qty_info = dt.get_employee_all_invoice_coloumns(dt.emp_id2,st_date.Date.ToString("yyyy-M-dd"),end_date_picker.Date.ToString("yyyy-M-dd"),ref invoice_header,ref invoice_column_sums);
		
		List<List<string>> fake_invoice_qty_info = dt.get_employee_all_invoice_coloumns(dt.emp_id2, dt.get_employee_first_stock_entry_date(dt.emp_id2).ToString("yyyy-M-d"), end_date_picker.Date.ToString("yyyy-M-d"),ref fake_invoice_header,ref fake_invoice_column_sums);

		List<List<string>> dynamic_opening_stock = dt.get_dynamic_opening_stock_to_a_date(dt.emp_id2, st_date.Date.AddDays(-1).ToString("yyyy-M-dd"), ref dynamic_opening_stock_header, ref opening_stock_sum, ref opeing_stock_value_sum);
		                                                        
		                                         if(dt.get_employee_first_stock_entry_date(dt.emp_id2).Date==st_date.Date)
		                                         {
			                                                 for(int i=0;i<dynamic_opening_stock.Count;i++)
			                                                       for(int j = 0; j < dynamic_opening_stock[i].Count;j++)
					                                                        dynamic_opening_stock[i][j] ="Dosen't Exist";

		                                         }
		List<string> date_range_sale = dt.get_sold_pcs_between_dates(dt.emp_id2, st_date.Date.ToString("yyyy-M-dd"), end_date_picker.Date.ToString("yyyy-M-dd"),ref sale_range_sum);
		
		dt.close_connection();

		List<string> dsalelis = new List<string> { "", "", "", "", "", "", "", "", "", "Total Stock Value", daily_sale.ToString() };

		List<List<string>> fake_stock_info = new List<List<string>>();


		foreach (List<string> lis in stock_info)
		{
			List<string> temp = new List<string>();

			foreach (string strings in lis)
			{
				temp.Add(strings);
			}

			fake_stock_info.Add(temp);

		}



		//stock_info.Add(dsalelis);

		for (int k = 0;dynamic_opening_stock!=null && k < dynamic_opening_stock.Count; k++)
		{
			stock_info[k].InsertRange(4, dynamic_opening_stock[k]);
		}



		for (int k = 0; k < invoice_qty_info.Count; k++)
		{
			stock_info[k].InsertRange(6, invoice_qty_info[k]);

		}


		for (int k = 0;dynamic_opening_stock!=null && k < dynamic_opening_stock.Count; k++)
		{
			fake_stock_info[k].InsertRange(4, dynamic_opening_stock[k]);
		}



		for (int k = 0; k < fake_invoice_qty_info.Count; k++)
		{
			fake_stock_info[k].InsertRange(6, fake_invoice_qty_info[k]);

		}


		int col = 6 + fake_invoice_header.Count;

		for (int i = 0; i < fake_stock_info.Count; i++)
		{
			fake_stock_info[i][col] = date_range_sale[i];

		}


		for (int k = 0; k < fake_stock_info.Count; k++)
		{
			int sale_val = Convert.ToInt32(fake_stock_info[k][fake_stock_info[k].Count - 6]);
			int summed_stock = 0;
			for (int m = 0; fake_invoice_qty_info.Count > 0 && m < fake_invoice_qty_info[k].Count; m++)
			{
				int stock_val = Convert.ToInt32(fake_invoice_qty_info[k][m]);
				summed_stock += stock_val;
				if (summed_stock - sale_val > 0)
				{



					string[] days = fake_invoice_header[m].Split('(', ')');
					stock_info[k].Add(days[1]);
					m = fake_invoice_qty_info[k].Count;

				}
				else if (m == fake_invoice_qty_info[k].Count - 1)
				{

					stock_info[k].Add("No Data");


				}

			}


		}


		col = 6 + invoice_header.Count;


		for (int i = 0; i < stock_info.Count; i++)
		{
			stock_info[i][col] = date_range_sale[i];

		}


		closing_stock_sums[0] = sale_range_sum;
		List<string> sum_row = new List<string> { "", "Total", "", "" };
		sum_row.Add(opening_stock_sum);
		sum_row.Add(opeing_stock_value_sum);
		sum_row.AddRange(invoice_column_sums);
		sum_row.AddRange(closing_stock_sums);
		sum_row.Add("");

		stock_info.Add(sum_row);

		List<string> stock_header = new List<string> { "Sno", "paticulars", "HSN_SAC", "MRP" };
		if (dynamic_opening_stock_header != null)
		{
			stock_header.AddRange(dynamic_opening_stock_header);
		}
		stock_header.AddRange(invoice_header);
		stock_header.Add("Sales");
		stock_header.Add("Return Stock");
		stock_header.Add("Closing_stock");
		stock_header.Add("Closing_stock_value");
		stock_header.Add("Stock Age");

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


	private async void get_stock_report_Clicked2(object sender, EventArgs e)
	{
		if (st_date.Date > end_date_picker.Date)
		{
			DisplayAlert("Invalid Date !", "The date ranges are not valid", "OK!");
			return;
		}


		if (vs.Contains(xlbtn))
			vs.Remove(xlbtn);

		string start_dt = String.Format("01/{0}/{1}", month_picker.SelectedIndex + 1, year_picker.SelectedItem as string);
		DateTime start_date = DateTime.ParseExact(start_dt, "dd/M/yyyy", null);

		DateTime end_date = EndOfMonth(start_date);


		await dt.get_emp_id();

		int daily_sale = 0;

		string opening_stock_sum="0"; string opeing_stock_value_sum="0";

		string sale_range_sum = "0";

		List<string> invoice_header = new List<string>();
		List<string> fake_invoice_header = new List<string>();
		List<string> invoice_column_sums = new List<string>();
		List<string> fake_invoice_column_sums = new List<string>();
		List<string> closing_stock_sums = new List<string>();
		List<string> dynamic_opening_stock_header = new List<string>();
		dt.start_connection();
		string state = dt.get_current_employee_state();



		//List<List<string>> stock_info = dt.get_store_stock(start_date.ToString("yyyy-MM-dd"), end_date.ToString("yyyy-MM-dd"), ref daily_sale, state);

		

		List<List<string>> stock_info = dt.get_opening_and_closing_stock(emp_id,st_date.Date.ToString("yyyy-M-dd"), end_date_picker.Date.ToString("yyyy-M-dd"), ref closing_stock_sums);

		List<List<string>> invoice_qty_info = dt.get_employee_all_invoice_coloumns(emp_id, st_date.Date.ToString("yyyy-M-dd"), end_date_picker.Date.ToString("yyyy-M-dd"), ref invoice_header, ref invoice_column_sums);

		List<List<string>> fake_invoice_qty_info = dt.get_employee_all_invoice_coloumns(emp_id, dt.get_employee_first_stock_entry_date(emp_id).ToString("yyyy-M-d"), end_date_picker.Date.ToString("yyyy-M-d"), ref fake_invoice_header, ref fake_invoice_column_sums);

		List<string> date_range_sale = dt.get_sold_pcs_between_dates(emp_id, st_date.Date.ToString("yyyy-M-dd"), end_date_picker.Date.ToString("yyyy-M-dd"), ref sale_range_sum);

		List<List<string>> dynamic_opening_stock = dt.get_dynamic_opening_stock_to_a_date(emp_id,st_date.Date.AddDays(-1).ToString("yyyy-M-dd"),ref dynamic_opening_stock_header,ref  opening_stock_sum,ref  opeing_stock_value_sum);
		if (dt.get_employee_first_stock_entry_date(emp_id).Date == st_date.Date)
		{
			for (int i = 0; i < dynamic_opening_stock.Count; i++)
				for (int j = 0; j < dynamic_opening_stock[i].Count; j++)
					dynamic_opening_stock[i][j] = "Dosen't Exist";

		}
		dt.close_connection();

		List<string> dsalelis = new List<string> { "", "", "", "", "", "", "", "", "", "Total Stock Value", daily_sale.ToString() };

		List<List<string>> fake_stock_info = new List<List<string>>();


		foreach (List<string> lis in stock_info)
		{
			List<string> temp = new List<string>();

			foreach (string strings in lis)
			{
				temp.Add(strings);
			}

			fake_stock_info.Add(temp);

		}



		//stock_info.Add(dsalelis);

		for(int k=0;k<dynamic_opening_stock.Count;k++)
		{
			stock_info[k].InsertRange(4, dynamic_opening_stock[k]);
		}



		for (int k = 0; k < invoice_qty_info.Count; k++)
		{
			stock_info[k].InsertRange(6, invoice_qty_info[k]);

		}


		for (int k = 0; k < dynamic_opening_stock.Count; k++)
		{
			fake_stock_info[k].InsertRange(4, dynamic_opening_stock[k]);
		}



		for (int k = 0; k < fake_invoice_qty_info.Count; k++)
		{
			fake_stock_info[k].InsertRange(6, fake_invoice_qty_info[k]);

		}


		int col = 6 + fake_invoice_header.Count;

		for (int i = 0; i < fake_stock_info.Count; i++)
		{
			fake_stock_info[i][col] = date_range_sale[i];

		}


		for (int k = 0; k < fake_stock_info.Count; k++)
		{
			int sale_val = 0;
			try
			{
				 sale_val = Convert.ToInt32(fake_stock_info[k][fake_stock_info[k].Count - 6]);
			}
			catch(Exception ex)
			{
				 sale_val = 0;
			}

			int summed_stock = 0;
			for (int m = 0; fake_invoice_qty_info.Count > 0 && m < fake_invoice_qty_info[k].Count; m++)
			{
				int stock_val = Convert.ToInt32(fake_invoice_qty_info[k][m]);
				summed_stock += stock_val;
				if (summed_stock - sale_val > 0)
				{



					string[] days = fake_invoice_header[m].Split('(', ')');
					stock_info[k].Add(days[1]);
					m = fake_invoice_qty_info[k].Count;

				}
				else if (m == fake_invoice_qty_info[k].Count - 1)
				{

					stock_info[k].Add("No Data");


				}

			}


		}


		col = 6 + invoice_header.Count;


		for (int i = 0; i < stock_info.Count; i++)
		{
			stock_info[i][col] = date_range_sale[i];

		}


		closing_stock_sums[0] = sale_range_sum;
		List<string> sum_row = new List<string> { "", "Total", "", ""};
		sum_row.Add(opening_stock_sum);
		sum_row.Add(opeing_stock_value_sum);
		sum_row.AddRange(invoice_column_sums);
		sum_row.AddRange(closing_stock_sums);
		sum_row.Add("");

		stock_info.Add(sum_row);

		List<string> stock_header = new List<string> { "Sno", "paticulars", "HSN_SAC", "MRP" };
		stock_header.AddRange(dynamic_opening_stock_header);
		stock_header.AddRange(invoice_header);
		stock_header.Add("Sales");
		stock_header.Add("Return Stock");
		stock_header.Add("Closing_stock");
		stock_header.Add("Closing_stock_value");
		stock_header.Add("Stock Age");

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

		Task.Run(async () =>
		{
			await dt.get_emp_id();
			dt.start_connection();
			DateTime min_date = dt.get_employee_first_stock_entry_date(this.emp_id);
			dt.close_connection();

			MainThread.InvokeOnMainThreadAsync(async () =>
			{
				st_date.MinimumDate = min_date;
				end_date_picker.MinimumDate = min_date;
				stockhs.IsVisible = true;
				stockhs.IsEnabled = true;

			});


		});


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


	private void search_emp_TextChanged_with_state(object sender, TextChangedEventArgs e)
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
			List<List<string>> result = dt.search_employee_in_db(search_emp.Text.Trim(), search_emp.Text.Trim(), search_emp.Text.Trim(),state);
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



	private void search_emp_TextChanged_with_zone(object sender, TextChangedEventArgs e)
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
			List<List<string>> result = dt.search_employee_in_db(search_emp.Text.Trim(), search_emp.Text.Trim(), search_emp.Text.Trim(), state,zone);
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