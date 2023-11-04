using Attendance.Data;
using Microsoft.Maui.Controls;
using MySqlConnector;

namespace Attendance.Pages;

public partial class UpdateYourSales : ContentPage
{
	public DataClass dt = new DataClass();
	public List<string> nothing_found_temp = new List<string> { "Nothing Found" };
	public string employee_ID = String.Empty;
	public UpdateYourSales()
	{
		InitializeComponent();
		DatePicker tdp = new DatePicker();
		List<string> dp_items = new List<string>();

		dp_items.Add(tdp.Date.ToString("yyyy-MM-dd"));
		DateTime dt_temp = tdp.Date;
		dt_temp = dt_temp.AddDays(-1);
		tdp.Date = dt_temp;
		dp_items.Add(tdp.Date.ToString("yyyy-MM-dd"));
		dpicker.ItemsSource = dp_items;
		dpicker.SelectedIndex = 0;
	}


    public UpdateYourSales(string emp_id, DatePicker temp_date_time)
	{
		InitializeComponent();
		DatePicker tdp = new DatePicker();
		tdp.Date = temp_date_time.Date;
		List<string> dp_items = new List<string>();

		dp_items.Add(tdp.Date.ToString("yyyy-MM-dd"));
		DateTime dt_temp = tdp.Date;
		dt_temp = dt_temp.AddDays(-1);
		tdp.Date = dt_temp;
		dp_items.Add(tdp.Date.ToString("yyyy-MM-dd"));
		dpicker.ItemsSource = dp_items;
		dpicker.SelectedIndex = 0;
		this.employee_ID = emp_id;
	}




	private async void additm_Clicked(object sender, EventArgs e)
	{

		vs.Remove(additm);

		ScrollView sv = new ScrollView { Orientation = ScrollOrientation.Horizontal };



		HorizontalStackLayout itemhs = new HorizontalStackLayout { Spacing = 10 };

		VerticalStackLayout product_search_vs = new VerticalStackLayout();

		SearchBar product_search_bar = new SearchBar { Placeholder="Search For Products" };

		ListView products_list = new ListView {IsVisible=true, IsEnabled=true };

		products_list.ItemsSource = nothing_found_temp;



		Button remove_btn = new Button { Text = "X" };

		product_search_bar.TextChanged += ((object sender, TextChangedEventArgs e) => 
		    { 
			  if(product_search_bar.Text.Trim()=="" )
			  {
					products_list.IsEnabled=true;
					products_list.IsVisible =true;
					products_list.ItemsSource = nothing_found_temp;

					return;
			  }

				Task.Run(() => 
				{ 
				  while(dt.is_conn_open)
					{

					}

					 dt.start_connection();
                     List<List<string>> products=dt.search_product_in_db(null, product_search_bar.Text);
					 dt.close_connection();


					List<string> results = new List<string>();

					for (int i = 0; i < products.Count; i++)
					{
						results.Add("Sno :" + products[i][0] + " " + products[i][1] + " Mrp :" + products[i][3]);
					}

					MainThread.InvokeOnMainThreadAsync(() => 
					{

						if(results.Count==0)
						{
							products_list.ItemsSource = nothing_found_temp;
						}
						else
						{
							products_list.IsVisible = true;
							products_list.ItemsSource = results;

						}
					  
					});





				});
			
			
			});

		product_search_vs.Add(product_search_bar);
		product_search_vs.Add(products_list);

		itemhs.Add(product_search_vs);

		Picker quantitypicker = new Picker { IsVisible=false,IsEnabled=false};

		List<string> all_qty = new List<string>();

		  for(int i=1;i<100;i++)
		  {
			 all_qty.Add(i.ToString());	
		  }

		  quantitypicker.ItemsSource= all_qty;
		  
		   itemhs.Add(quantitypicker);

		quantitypicker.SelectedIndexChanged += (s, e) =>
		{
			Label temp = (Label)itemhs[0];
			int mrp= Convert.ToInt32(temp.Text.Split(':')[2].Trim());
			int price = mrp * (quantitypicker.SelectedIndex + 1) ;
			itemhs[2] = new Label { Text = "Price : " +price.ToString()};
			calc_all_items_price();
		};
		 

		remove_btn.Clicked += (s, e) => 
		{
			vs.Remove(sv);
			calc_all_items_price();
		
		};

		itemhs.Add(remove_btn);

		products_list.ItemSelected += (s, e) => 
		{
			string tplis = (string)products_list.SelectedItem;

			if (tplis == "Nothing Found")
				return;

			quantitypicker.IsEnabled = true; 
			quantitypicker.IsVisible = true;
			
			product_search_bar.Text = (string)products_list.SelectedItem;

			itemhs.Remove(product_search_vs);

			products_list.IsEnabled = false;
			products_list.IsVisible = false;

			itemhs.Clear();

			

			itemhs.Add(new Label { Text = product_search_bar.Text,FontSize=15 } );

			itemhs.Add(quantitypicker);

			Label temp = (Label)itemhs[0];
			int mrp = Convert.ToInt32(temp.Text.Split(':')[2].Trim());
			int price = mrp * (quantitypicker.SelectedIndex + 1);

			itemhs.Add(new Label {Text="Price : "+mrp.ToString() });
		
			itemhs.Add(remove_btn);
			calc_all_items_price();





			quantitypicker.SelectedIndex = 0;
		};


		sv.Content= itemhs;

		vs.Add(sv);
		vs.Add(additm);



	}

	public bool is_safe()
	{
		foreach (var item in vs)
		{
			if (item.GetType() == typeof(ScrollView))
			{
				ScrollView sv = (ScrollView)item;

				foreach (var details in (HorizontalStackLayout)sv.Content)
				{

					if (details.GetType() == typeof(Picker))
					{
						Picker q = (Picker)details;
						if (q.SelectedIndex == -1)
						{

							return false;
						}
					}




					

				}

			}
		}

		return true;
	}


	public void calc_all_items_price()
	{
		int total_price = 0;
       foreach(var item in vs)
		{
			if(item.GetType() == typeof(ScrollView))
			{
				ScrollView sv = (ScrollView)item;

				foreach(var details in (HorizontalStackLayout)sv.Content)
				{

					if (details.GetType() == typeof(Picker))
					{
						Picker q = (Picker)details;
						if (q.SelectedIndex == -1)
						{
							
					             continue;
						}
					}




					if (details.GetType() == typeof(Label) )
					{

						Label price = (Label)details;
						if(price.Text.Contains("Price"))
						{
							total_price += Convert.ToInt32(price.Text.Split(':')[1].Trim());
						}

					}
					
				}

			}
		}	

	   if(total_price==0)
		{
			uitmbtn.Text = "Check And Tally Items";
		}
	   else
		{
			uitmbtn.Text = "Check And Tally Items With Price :"+total_price.ToString();
		}

	}

	private async void uitmbtn_Clicked(object sender, EventArgs e)
	{
		if(!is_safe())
		{
			DisplayAlert("Please Check quantity or Remove item", "Add quantity to the item", "Ok");
			return;
		}

		actind.IsVisible = true;

		 Task.Run(async () => 
		{
			await dt.get_emp_id();
			await make_query();

			MainThread.InvokeOnMainThreadAsync(() => {

				DisplayAlert("Updated!", "Your Sales has been updated", "Ok");
				Navigation.PopAsync();
			
			});
		
		});


	}

	public async Task make_query()
	{


		List<string> commandsstrs = new List<string>();

		foreach (var sc in vs)
		{

			if (sc.GetType().ToString() == "Microsoft.Maui.Controls.ScrollView")
			{
				ScrollView sc1 = sc as ScrollView;
				HorizontalStackLayout temphs = sc1.Content as HorizontalStackLayout;
				Label pname = temphs[0] as Label;
				Picker qty = temphs[1] as Picker;
				Label lprice = temphs[2] as Label;

				string[] tempsno = pname.Text.Split(' ');
				string[] tempsno2 = tempsno[1].Split(':');

				string snostr = tempsno2[1].Trim();

				
				int qty_in_int = Convert.ToInt32((qty.ItemsSource[qty.SelectedIndex].ToString()));
				int price = Convert.ToInt32((lprice.Text.Split(':')[1].Trim()));
				string amount = (price * qty_in_int).ToString();

				// string stemp = "INSERT INTO employee_sales2 values ('" + dt.emp_id2 + "','" + snostr + "','"+qty_in_int.ToString()+"','"+amount+ "',convert_tz(now(),'-7:00','+5:30'),convert_tz(now(),'-7:00','+5:30'));";
				//string stemp = "INSERT INTO employee_sales2 values (emp_id,sno,pcs,amount,The_Time,The_date) ('" + dt.emp_id2 + "'," + snostr + "," + qty_in_int.ToString() + "," + amount + ",convert_tz(now(),'-7:00','+5:30'),convert_tz(now(),'-7:00','+5:30'));";
				string stemp = "";
				if (dpicker.SelectedIndex == 0)
				{
					if (employee_ID == String.Empty)
					{
						stemp = String.Format("INSERT INTO employee_sales2 (emp_id,sno,pcs,amount,The_Time,The_date) values ('{0}',{1},{2},{3},now(),now());", dt.emp_id2, snostr, qty_in_int.ToString(), amount);
					}
					else if (employee_ID != String.Empty)
					{
						stemp = String.Format("INSERT INTO employee_sales2 (emp_id,sno,pcs,amount,The_Time,The_date) values ('{0}',{1},{2},{3},'{4}','{5}');", employee_ID, snostr, qty_in_int.ToString(), amount, "23:59:59", dpicker.ItemsSource[0]);

					}
				}
				else if (dpicker.SelectedIndex == 1)
				{
					if (employee_ID == String.Empty)
					{
						stemp = String.Format("INSERT INTO employee_sales2 (emp_id,sno,pcs,amount,The_Time,The_date) values ('{0}',{1},{2},{3},'{4}','{5}');", dt.emp_id2, snostr, qty_in_int.ToString(), amount, "23:59:59", dpicker.ItemsSource[1]);
					}
					else if (employee_ID != String.Empty)
					{
						stemp = String.Format("INSERT INTO employee_sales2 (emp_id,sno,pcs,amount,The_Time,The_date) values ('{0}',{1},{2},{3},'{4}','{5}');", employee_ID, snostr, qty_in_int.ToString(), amount, "23:59:59", dpicker.ItemsSource[1]);

					}
				}

				commandsstrs.Add(stemp);


			}
		}


		process_mysql_cmd_list(commandsstrs);

	}


	public void process_mysql_cmd_list(List<string> list)
	{
		dt.start_connection();


		foreach (string q in list)
		{
			MySqlCommand cmd = new MySqlCommand(q, dt.connection);
			try
			{
				cmd.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				dt.close_connection();
				return;
			}
		}


		dt.close_connection();
	}




}