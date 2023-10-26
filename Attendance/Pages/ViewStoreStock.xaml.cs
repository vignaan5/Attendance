using Attendance.Data;

namespace Attendance.Pages;

public partial class ViewStoreStock : ContentPage
{
	public DataClass dt = new DataClass();
	public Button xlbtn = new Button { Text = "Generate Excel",HorizontalOptions=LayoutOptions.Center };
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
		List<List<string>> stock_info = dt.get_store_stock(start_date.ToString("yyyy-MM-dd"), end_date.ToString("yyyy-MM-dd"), ref daily_sale, state);
		dt.close_connection();

		List<string> dsalelis = new List<string> {"","","","","Total Stock Value",daily_sale.ToString() };

		stock_info.Add(dsalelis);

		List<string> stock_header = new List<string> { "Sno", "Particulars", "HSN SAC", "MRP", "PCS", "Amount" };

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

	private async void goback_btn_Clicked(object sender, EventArgs e)
	{
	

	 await	Navigation.PopToRootAsync();

	}
}