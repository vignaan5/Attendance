using Attendance.Data;
using MySqlConnector;
using The_Attendance.Interfaces;

namespace Attendance.Pages;

public partial class UpdateSalesAndroid : ContentPage
{
	DataClass dt = new DataClass();
	public List<string> nothing_found_temp = new List<string> { "Nothing Found" };
	public List<string> selected_items=new List<string>();
	public List<string> empty_list = new List<string>();
	public string employee_ID = String.Empty;
	public List<string> dpitems = new List<string>();
	public Dictionary<int,int> current_stock=new Dictionary<int,int>();

	


	public UpdateSalesAndroid()
	{
		InitializeComponent();
		Task.Run(async () => 
		{
			await dt.get_emp_id();
			dt.start_connection();
			current_stock = dt.get_current_store_stock_in_dic(dt.emp_id2);
			dt.close_connection();
		
		});
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

	public UpdateSalesAndroid(string emp_id, DatePicker temp_date_time)
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



	private void Products_search_bar_TextChanged(object sender, TextChangedEventArgs e)
	{
		products_list.ItemsSource = nothing_found_temp;
		products_list.IsVisible = true;

		if (products_search_bar.Text.Trim() == "")
		{
			products_list.ItemsSource = empty_list;
			return;
		}

		Task.Run(async () =>
		{

			while (dt.is_conn_open)
			{

			}

			dt.start_connection();
			List<List<string>> products = dt.search_product_in_db(null, products_search_bar.Text.Trim());
			dt.close_connection();

			List<string> results = new List<string>();

			for (int i = 0; i < products.Count; i++)
			{
				results.Add("Sno :" + products[i][0] + " " + products[i][1] + " Mrp :" + products[i][3]);
			}

			if (results.Count == 0)
			{
				MainThread.InvokeOnMainThreadAsync(async () => {
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

	private void products_list_ItemSelected(object sender, SelectedItemChangedEventArgs e)
	{
		
		string selected_item = (string)products_list.SelectedItem;
		products_list.ItemsSource = null;

		ScrollView sc = new ScrollView {Orientation=ScrollOrientation.Horizontal };

		Label item = new Label {Text=selected_item };

		HorizontalStackLayout itemhs= new HorizontalStackLayout {Spacing=10,HorizontalOptions=LayoutOptions.Center,VerticalOptions=LayoutOptions.Center};
		
	     Picker quantity_picker = new Picker();

		List<string> qty = new List<string>();

		for(int i=1;i<100;i++)
		{
			qty.Add(i.ToString());
		}

		quantity_picker.ItemsSource=qty;

		Button remove_btn = new Button { Text="X"};


		remove_btn.Clicked += (s, e) => { vs.Remove(sc); calc_all_items_price(); };


		quantity_picker.SelectedIndexChanged += (s,e)=> 
		{
			Label temp = (Label)itemhs[0];
			int mrp = Convert.ToInt32(temp.Text.Split(':')[2].Trim());
			int price = mrp * (quantity_picker.SelectedIndex + 1);
			itemhs[2] = new Label { Text = "Price : " + price.ToString() };
			calc_all_items_price();

		};


		itemhs.Add(item);
		itemhs.Add(quantity_picker);
		
		itemhs.Add(new Label { Text = "Price :" });
		itemhs.Add(remove_btn);

		
		


		sc.Content= itemhs;

		

		vs.Remove(uitmbtn);
		vs.Add(sc);
		vs.Add(uitmbtn);

		((Picker)itemhs[1]).SelectedIndex = 0;


		products_search_bar.Text = "";



#if ANDROID
                 
Attendance.Platforms.Android.KeyboardHelper.HideKeyboard();

#endif
	}

	private void Quantity_picker_SelectedIndexChanged(object sender, EventArgs e)
	{
		throw new NotImplementedException();
	}

	public void calc_all_items_price()
	{
		int total_price = 0;
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

							continue;
						}
					}




					if (details.GetType() == typeof(Label))
					{

						Label price = (Label)details;
						if (price.Text.Contains("Price"))
						{
							total_price += Convert.ToInt32(price.Text.Split(':')[1].Trim());
						}

					}

				}

			}
		}

		if (total_price == 0)
		{
			uitmbtn.Text = "Check And Tally Items";
		}
		else
		{
			uitmbtn.Text = "Check And Tally Items With Price :" + total_price.ToString();
		}

	}


	public bool is_safe()
	{
		

		if (vs.Count == 1)
			return false;

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



	private async void uitmbtn_Clicked(object sender, EventArgs e)
	{
		uitmbtn.IsEnabled = false;
		
		if (!is_safe())
		{
			DisplayAlert("Please Check quantity or Remove item", "Add quantity to the item", "Ok");
			uitmbtn.IsVisible = true;
			uitmbtn.IsEnabled = true;
			return;
		}
#if ANDROID
		if(!DependencyService.Resolve<IAndroid>().IsForeGroundServiceRunning())
		{
			DisplayAlert("Not Clocked in !", "Please clock in to update your sales", "ok");
			Navigation.PopAsync();
			return;
		}
#endif


		Task.Run(async () =>
		{
			await dt.get_emp_id();
		bool item_not_in_stock=	await make_query();


			if (!item_not_in_stock)
			{
				MainThread.InvokeOnMainThreadAsync(() =>
				{

					DisplayAlert("Updated!", "Your Sales has been updated", "Ok");
					Navigation.PopAsync();

				});
			}

		});


	}

	public async Task<bool> make_query()
	{


		List<string> commandsstrs = new List<string>();

		Dictionary<int,int> temp_current_stock= new Dictionary<int,int>();	

		foreach(var x in current_stock)
		{
			temp_current_stock.Add(x.Key, x.Value);
		}

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
				int price = Convert.ToInt32((pname.Text.Split(':')[2].Trim()));
				string amount = (price * qty_in_int).ToString();

				int snoint = Convert.ToInt32(snostr);

				temp_current_stock[snoint] -= qty_in_int;

				if (temp_current_stock[snoint]<0)
				{
					temp_current_stock.Clear();
					MainThread.InvokeOnMainThreadAsync(() => {
						DisplayAlert("Item with Sno :" + snoint + " is Out of Stock", "The item stock is not found in the store", "Ok"); uitmbtn.IsEnabled = true;
					});
					return true;
				}


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
		return false;
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