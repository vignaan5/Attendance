using Attendance.Data;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using MySqlConnector;

namespace Attendance.Pages;

public partial class UpdateSales : ContentPage
{
	public Dictionary<string, Dictionary<string, string>> products = new Dictionary<string, Dictionary<string, string>>();
	public DataClass dt  = new DataClass();	
	public List<string> picker_list = new List<string>();
	public HashSet<int> hsh = new HashSet<int>();
	public int total_price=0;
	public UpdateSales()
	{
		InitializeComponent();
		exc_tmp();

	}

	public async void exc_tmp()
	{
		await Task.Run(() => {
			get_items_from_db();
			add_items_to_picker();
		});

		additm.IsEnabled = true;
		additm.IsVisible = true;

		uitmbtn.IsEnabled = true;

		uitmbtn.IsVisible = true;

		actind.IsVisible = false;
		
	}

	public void get_items_from_db()
	{
		dt.start_connection();
		products=dt.get_products();
		dt.close_connection();
		
	
	}


	public void add_items_to_picker()
	{
		List<string> lis = new List<string>();

		foreach(var item in products) 
		{
			lis.Add(item.Key.ToString());
		}

			picker_list=lis;
	}


	private void remove_vs_item(object sender, EventArgs e)
	{
		
	}



	public int get_tally_sum()
	{
		int x = 0;

		foreach (var item1 in vs)
		{
			if(item1.GetType()==typeof(ScrollView))
			{
				ScrollView sc = (ScrollView)item1;

				HorizontalStackLayout h = (HorizontalStackLayout)sc.Content;

				foreach(var item2 in (HorizontalStackLayout)h)
				{
					if(item2.GetType()==typeof(Label)) 
					{
						Label lbl = (Label)item2;
						string[] display_lable_price = ((Label)lbl).Text.ToString().Split(':');
						try
						{
							int xx = Convert.ToInt32(display_lable_price[1].Trim());
							try
							{
								x += xx;
							}
							catch
							{
								
							}
						}
						catch (Exception ex)
						{
							x = 0;
						}
					}
				}
			}
		}


		return x;
	}


	private void Button_Clicked(object sender, EventArgs e)
	{
		
		List<int> qlis = new List<int>(100);

		for (int i = 0; i < 100;i++)
		{
			qlis.Add(i+1);
		}

		Picker qpicker = new Picker {Title="Select Quantity"};
		qpicker.ItemsSource = qlis;


		ScrollView sc = new ScrollView();
		sc.Orientation = ScrollOrientation.Horizontal;

	
		 Label display_price = new Label { Text="Price : " };


		HorizontalStackLayout temphs = new HorizontalStackLayout();

		Picker itempicker = new Picker{Title ="Select a Product" };

		itempicker.ItemsSource = picker_list;

		itempicker.SelectedIndexChanged += ((object sender, EventArgs args) =>
		{
			if (qpicker.SelectedIndex == -1)
				return;

			if (!check_is_safe())
				return;


			string product_mpr = products[itempicker.ItemsSource[itempicker.SelectedIndex].ToString()]["mrp"];
			string product_qty = qpicker.ItemsSource[qpicker.SelectedIndex].ToString();

			int product_price = Convert.ToInt32(product_mpr) * Convert.ToInt32(product_qty);

			display_price.Text="Price : "+ product_price.ToString()+" ";

			

			uitmbtn.Text= "Check And Tally Items with total price : "+get_tally_sum().ToString();

		});



		qpicker.SelectedIndexChanged += ((object sender, EventArgs e) =>
		{
			if (itempicker.SelectedIndex == -1)
				return;

			if (!check_is_safe())
				return;

			string product_mpr = products[itempicker.ItemsSource[itempicker.SelectedIndex].ToString()]["mrp"];
			string product_qty = qpicker.ItemsSource[qpicker.SelectedIndex].ToString();

			int product_price = Convert.ToInt32(product_mpr) * Convert.ToInt32(product_qty);

			display_price.Text = "Price : " + product_price.ToString()+" ";

			

			uitmbtn.Text = "Check And Tally Items with total price : " + get_tally_sum().ToString();

		
		});
	

		 temphs.Add(itempicker);
		temphs.Add(qpicker);

		

		Button temp = new Button();
		temp.Text = "X";
		temp.MinimumHeightRequest = 20;
		temp.MaximumHeightRequest = 20;
		
		Random r = new Random();

		int uid = r.Next(1, 1000);

		while(hsh!=null &&  hsh.Contains(uid)) 
		{ 
		   uid = r.Next(1, 1000);
		}

		hsh.Add(uid);

		sc.AutomationId = uid.ToString();

		temp.AutomationId = uid.ToString();


		temp.Clicked+= ((object sender,EventArgs e) =>{ 
		  
		  for(int i=0;i<vs.Count;i++)
			{
				if (vs[i]!=null && vs[i].AutomationId==temp.AutomationId) {
					{
						hsh.Remove(Convert.ToInt32(temp.AutomationId));

						string vsofitype = vs[i].GetType().ToString();	

						ScrollView sc_tmp = (ScrollView)vs[i];

						HorizontalStackLayout hs_tmp = (HorizontalStackLayout)sc_tmp.Content;

						foreach(var lbl in  hs_tmp) 
						{ 
						    if(lbl.GetType()==typeof(Label) && check_is_safe()) 
							{

								string[] display_lable_price = ((Label) lbl).Text.ToString().Split(':');
							   total_price-= Convert.ToInt32(display_lable_price[1].Trim() );
								break;
							}
						}
						
						
						uitmbtn.Text = "Check And Tally Items with total price : " + total_price.ToString();
						vs.Remove(vs[i]);
						return;
					}
				}
			}
		
		});
		display_price.VerticalTextAlignment = TextAlignment.Center;
		temphs.Add(display_price);
		temphs.Add(temp);
		temphs.Spacing = 10;
	
		sc.Content= temphs;
		vs.Add(sc);
		vs.Remove(additm);
		vs.Add(additm);

	     

		
		
	}


	public bool check_is_safe()
	{
		if (vs.Count == 4)
			return false;

		foreach (var sc in vs)
		{
			string dd = sc.GetType().ToString();

			if (dd == "Microsoft.Maui.Controls.ScrollView")
			{
				ScrollView tsc = (ScrollView)sc;
				HorizontalStackLayout temphs = tsc.Content as HorizontalStackLayout;
				foreach (var y in temphs)
				{
					string cc = y.GetType().ToString();
					if (cc == "Microsoft.Maui.Controls.Picker")
					{
						Picker p = y as Picker;

						if (p.SelectedIndex == -1)
						{
							
							return false;
						}

					}

				

				}
			}

		}

		return true;
	}


	public void process_mysql_cmd_list(List<string> list)
	{
		dt.start_connection();


		foreach(string q in list)
		{
			MySqlCommand cmd = new MySqlCommand(q, dt.connection);
			try
			{
				cmd.ExecuteNonQuery();
			}
			catch(Exception ex) 
			{
				dt.close_connection();
					return;
			}
		}


		dt.close_connection();
	}


	public async Task make_query()
	{
	 
      
		List<string> commandsstrs = new List<string>();

		              foreach(var sc in  vs) 
		              { 
		     
		                        if(sc.GetType().ToString()== "Microsoft.Maui.Controls.ScrollView")
			                     {
				                           ScrollView sc1 = sc as ScrollView;
				                          HorizontalStackLayout temphs = sc1.Content as HorizontalStackLayout;
				                                             Picker pname = temphs[0] as Picker;
				                                             Picker qty   = temphs[1] as Picker;
				                    string snostr = products[pname.ItemsSource[pname.SelectedIndex].ToString()]["sno"];
				                    int qty_in_int = Convert.ToInt32( (qty.ItemsSource[qty.SelectedIndex].ToString()) );	
				                    int price = Convert.ToInt32((products[pname.ItemsSource[pname.SelectedIndex].ToString()]["mrp"]));
				                      string amount =   (price*qty_in_int) .ToString();

				// string stemp = "INSERT INTO employee_sales2 values ('" + dt.emp_id2 + "','" + snostr + "','"+qty_in_int.ToString()+"','"+amount+ "',convert_tz(now(),'-7:00','+5:30'),convert_tz(now(),'-7:00','+5:30'));";
				string stemp = "INSERT INTO employee_sales2 values ('" + dt.emp_id2 + "'," + snostr + "," + qty_in_int.ToString() + "," + amount + ",convert_tz(now(),'-7:00','+5:30'),convert_tz(now(),'-7:00','+5:30'));";
				commandsstrs.Add(stemp);	
				

			                     }
		              }


		process_mysql_cmd_list(commandsstrs);

	}






	private async void Button_Clicked_1(object sender, EventArgs e)
	{

		actind.IsVisible=true;
		bool is_safe = true;

		await dt.get_emp_id();
		await Task.Run(async () => { 
		
		   if(check_is_safe()) 
			{

			 await	make_query();

			     
			}
		   else is_safe = false;
		
		
		
		});


		if (!is_safe) {
			 actind.IsVisible=false;
			DisplayAlert("Not Selected", "Please Seclect the left items or Remove Items properly", "Continue");
			return;
		}

		Navigation.PopAsync();
		DisplayAlert("Updated !", "Your Sales has been updated", "Continue");


		string usr = await SecureStorage.GetAsync("username");
		string is_admin = await SecureStorage.GetAsync("admin");

		bool is_admin2 = false;

		if(is_admin=="yes")
			is_admin2=true;

		App.Current.MainPage=new AppShell2(usr,is_admin2);
		

	}
}