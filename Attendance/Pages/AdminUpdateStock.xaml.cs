using Attendance.Data;

namespace Attendance.Pages;

public partial class AdminUpdateStock : ContentPage
{
	public string emp_id { get; set; }
	public DataClass dt = new DataClass();
	bool dont_trigger_emp_search_text_event = false;
	public List<string> nothing_found_temp = new List<string> { "Nothing Found" };

	public AdminUpdateStock()
	{
		InitializeComponent();
		search_emp.TextChanged += search_emp_TextChanged;

		svs.IsVisible = true;
		svs.IsEnabled = true;
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

		get_stock_report2.IsVisible = true;

		get_stock_report2.IsEnabled = true;

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

	private void get_stock_report_Clicked(object sender, EventArgs e)
	{
		Navigation.PushAsync(new UpdateStock(this.emp_id, true));
	}

	private void get_stock_report2_Clicked(object sender, EventArgs e)
	{
		Navigation.PushAsync(new ViewRecentStocks(this.emp_id,true));
	}
}