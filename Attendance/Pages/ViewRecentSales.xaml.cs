using Attendance.Data;
using Microsoft.Maui.Graphics;
using MySqlConnector;

namespace Attendance.Pages;

public partial class ViewRecentSales : ContentPage
{
	public List<Dictionary<string,string>> recent_sales=new List<Dictionary<string, string>>();
	public DataClass dt = new DataClass();
	public string today ="";
	public ViewRecentSales()
	{
		InitializeComponent();
		get_recent_sold_items();
	}

	public ViewRecentSales(DataClass dt)
	{
		InitializeComponent();
		this.dt = dt;
	}

	public async void get_recent_sold_items()
	{
		
		await Task.Run(async() => {
			dt.start_connection();
			await dt.get_emp_id();
			DatePicker temp= new DatePicker();
			 today=temp.Date.Year.ToString()+"-"+temp.Date.Month.ToString()+"-"+temp.Date.Day.ToString();
			recent_sales = dt.get_employee_recent_sales_on_the_day(today);
			dt.close_connection();    
		});
		 
		if(recent_sales==null || recent_sales.Count<=0)
		{
			actind.IsVisible=false;
			Label nothing = new Label { Text="No items were sold today"};
			vs.Add(nothing);
			return;
		}

         
		foreach(var product in recent_sales)
		{
			VerticalStackLayout vs2 = new VerticalStackLayout();

			string emp_id = "";
			string pcs = "";
			string productname = "";
			string yoursale = "";
			string sold_time = "";
			string sold_date = "";
			string sno = "";
			string amount = "";

			foreach (var product_details in product)
			{

				Label l = new Label { Text = product_details.Key + " : " + product_details.Value };

				 if(product_details.Key=="product")
				{
					l.FontSize = 20;
					productname= product_details.Value;
				}
				if (product_details.Key == "sno")
				{
				
					sno = product_details.Value;
				}
				if(product_details.Key == "pcs")
				{

					pcs = product_details.Value;
				}
				if(product_details.Key == "your_sale")
				{

					amount = product_details.Value;
				}
				if (product_details.Key == "emp_id")
				{

					emp_id = product_details.Value;
				}

				if (product_details.Key == "sold_time")
				{

					sold_time = product_details.Value;
				}

				if (product_details.Key == "sold_date")
				{

				 string[]	sold_date1 = product_details.Value.Split(' ');
					string[] sold_date2 = sold_date1[0].Split('/');

					sold_date = sold_date2[2] + "-" + sold_date2[0] + "-" + sold_date2[1];
					
				}


				vs2.Add(l);
			}

			
			Button rm = new Button { Text="Remove Sale",HorizontalOptions=LayoutOptions.Center};

			rm.Clicked += (async(object sender, EventArgs e) =>
			{
				string remove_sql_qry = String.Format("delete from employee_sales2 where emp_id='{0}' and Sno={1} and pcs={2} and The_date='{3}' and The_Time='{4}' and amount={5};",emp_id,sno,pcs,sold_date,sold_time,amount);

				dt.start_connection();
				MySqlCommand cmd= new MySqlCommand(remove_sql_qry,dt.connection);

				try
				{
					cmd.ExecuteNonQuery();
				}
				catch (Exception ex) { }

				dt.close_connection();

			   
				DisplayAlert("Sale Removed !", "your unwanted sale has been removed", "ok");
				Navigation.PopAsync();

				string usr = await SecureStorage.GetAsync("username");
				string is_admin = await SecureStorage.GetAsync("admin");

				bool is_admin2 = false;

				if (is_admin == "yes")
					is_admin2 = true;

				App.Current.MainPage = new AppShell2(usr, is_admin2);

			});

			vs2.Add(rm);

			Frame frame = new Frame { MaximumHeightRequest = 400, MaximumWidthRequest = 400 };

			frame.Content = vs2;

			
			frame.HorizontalOptions = LayoutOptions.Center;
			
			

			vs.Add(vs2);

		}

		actind.IsVisible = false;

		}

	}




