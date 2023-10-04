using Attendance.Data;

using MySqlConnector;

namespace Attendance.Pages;

public partial class OverrideEmployeeSales : ContentPage
{
	public string emp_id=String.Empty;
	string mysql_conn_str = String.Empty;
	public SearchBar products_search_bar = new SearchBar();
	public ListView products_list = new ListView();
	DataClass dt = new DataClass();
	public List<string> nothing_found_temp = new List<string> {"Nothing Found"};
	
	public OverrideEmployeeSales()
	{
		InitializeComponent();
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

		mysql_conn_str = builder.ToString();
	}

	private void search_emp_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (search_emp.Text.Trim() == "")
		{
			
			emplist.ItemsSource = nothing_found_temp;
			return;
		}

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
					temp.Add(list[0] + "  " + list[1]+" " + list[2]);

				}
				if(temp.Count==0)
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





	private void emplist_ItemSelected(object sender, SelectedItemChangedEventArgs e)
	{
		this.emp_id = (string)emplist.SelectedItem;
		this.emp_id = this.emp_id.Split(' ')[0];

		emp_vs.Clear();

		DatePicker datePicker = new DatePicker();

		DateTime temp_date = DateTime.Parse("01/09/2023 00:00:00");

		datePicker.MinimumDate = temp_date;

		temp_date = DateTime.Parse("10/09/2023 00:00:00");

		datePicker.MaximumDate = temp_date;

		datePicker.HorizontalOptions=LayoutOptions.Center;

	   


			
		    Button usales = new Button { Text = String.Format("Update Sales of employee {0} on date {1}",this.emp_id,datePicker.Date.ToString("dd-MM-yyyy")), HorizontalOptions = LayoutOptions.Center };

		datePicker.DateSelected += ((object sender,DateChangedEventArgs d) =>
		{
			MainThread.InvokeOnMainThreadAsync(() => {
				usales.Text = String.Format("Update Sales of employee {0} on date {1}", this.emp_id, datePicker.Date.ToString("dd-MM-yyyy"));
			});

		});



		usales.Clicked += ((object sender, EventArgs e) => 
		   {
			   MainThread.InvokeOnMainThreadAsync(() => 
			   {

				   Navigation.PushAsync(new UpdateYourSales(this.emp_id, datePicker));
			   });
		
		   });

		emp_vs.Add(datePicker);
		emp_vs.Add(usales);


	}

	private void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
	{
		throw new NotImplementedException();
	}

	private void Products_search_bar_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (products_search_bar.Text.Trim() == "")
		{
			products_list.ItemsSource = nothing_found_temp;
			return;
		}

		Task.Run(async () =>
		{

			while (dt.is_conn_open)
			{

			}

			dt.start_connection();
			List<List<string>> products = dt.search_product_in_db(this.emp_id, products_search_bar.Text.Trim());
			dt.close_connection();

			List<string> results = new List<string>();

			for (int i = 0; i < products.Count; i++)
			{
				results.Add("Sno :"+products[i][0] + " " + products[i][1] + " Mrp :" + products[i][3]);
			}

			if (results.Count == 0)
			{
				MainThread.InvokeOnMainThreadAsync(async() => { 
					products_list.ItemsSource = nothing_found_temp; 
				});
				
			}
			else
			{
				MainThread.InvokeOnMainThreadAsync(async () => {
					products_list.ItemsSource = results;
				});
			
			}

			MainThread.InvokeOnMainThreadAsync(() => {

				products_list.IsVisible = true;
			});
			


		});


	}
}