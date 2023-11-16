using Attendance.Data;

using MySqlConnector;
using The_Attendance.Interfaces;

namespace Attendance.Pages;

public partial class UpdateStock : ContentPage
{
   public  DataClass dt = new DataClass();
    public List<string> nothing_found_temp = new List<string> { "Nothing Found" };
    public List<string> selected_items = new List<string>();
    public string employee_ID = String.Empty;
    public List<string> dpitems = new List<string>();
    public string type=String.Empty;
    
    public UpdateStock()
	{
		InitializeComponent();
        DatePicker tdp = new DatePicker();
        List<string> dp_items = new List<string>();

        dp_items.Add(tdp.Date.ToString("yyyy-MM-dd"));
        DateTime dt_temp = tdp.Date;
        dt_temp = dt_temp.AddDays(-1);
        tdp.Date = dt_temp;
     //   dp_items.Add(tdp.Date.ToString("yyyy-MM-dd"));
        
        dpicker.ItemsSource = dp_items;
        dpicker.SelectedIndex = 0;
        invoicebox.Text = "";
        invoicebox.IsEnabled = true;
        invoicebox.IsVisible = true;

        Task.Run(async() => 
        {
            await dt.get_emp_id();
            dt.start_connection();
            
         List<string> invoices=  dt.get_employee_Stock_invoices(dt.emp_id2, DateTime.Now.Date.ToString("yyyy-MM-dd"),DateTime.Now.Date.ToString("yyyy-MM-dd"));
            dt.close_connection();

            if(invoices!=null && invoices.Count>0)
            {
                MainThread.InvokeOnMainThreadAsync(async() => 
                { 

                    Invoice_picker.ItemsSource=invoices;
                    inv_hs.IsEnabled = true;
                    inv_hs.IsVisible = true;
                    Invoice_picker.IsVisible = true;
                    Invoice_picker.IsEnabled = true;

                
                });
            }

        });



    }

    public UpdateStock(string type)
    {
        InitializeComponent();
        DatePicker tdp = new DatePicker();
        List<string> dp_items = new List<string>();

        dp_items.Add(tdp.Date.ToString("yyyy-MM-dd"));
        DateTime dt_temp = tdp.Date;
        dt_temp = dt_temp.AddDays(-1);
        tdp.Date = dt_temp;
       // dp_items.Add(tdp.Date.ToString("yyyy-MM-dd"));
        dpicker.ItemsSource = dp_items;
        dpicker.SelectedIndex = 0;
        this.type= type;
		invoicebox.Text = "";
		invoicebox.IsEnabled = true;
        invoicebox.IsVisible = true;

		Task.Run(async () =>
		{
			await dt.get_emp_id();
			dt.start_connection();
            List<string> invoices = dt.get_employee_Return_invoices(dt.emp_id2, DateTime.Now.Date.ToString("yyyy-MM-dd"), DateTime.Now.Date.ToString("yyyy-MM-dd"));
			dt.close_connection();

            

			if (invoices != null && invoices.Count > 0)
			{
				MainThread.InvokeOnMainThreadAsync(async () =>
				{

					Invoice_picker.ItemsSource = invoices;
					inv_hs.IsEnabled = true;
					inv_hs.IsVisible = true;
					Invoice_picker.IsVisible = true;
					Invoice_picker.IsEnabled = true;


				});
			}

		});

	}


    private void Products_search_bar_TextChanged(object sender, TextChangedEventArgs e)
    {
        products_list.ItemsSource = nothing_found_temp;
        products_list.IsVisible = true;

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

        ScrollView sc = new ScrollView { Orientation = ScrollOrientation.Horizontal };

        Label item = new Label { Text = selected_item };

        HorizontalStackLayout itemhs = new HorizontalStackLayout { Spacing = 10, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };

        Picker quantity_picker = new Picker();

        List<string> qty = new List<string>();

        for (int i = 1; i < 100; i++)
        {
            qty.Add(i.ToString());
        }

        quantity_picker.ItemsSource = qty;

        Button remove_btn = new Button { Text = "X" };


        remove_btn.Clicked += (s, e) => { vs.Remove(sc); calc_all_items_price(); };


        quantity_picker.SelectedIndexChanged += (s, e) =>
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





        sc.Content = itemhs;


        

        vs.Remove(uitmbtn);
        vs.Add(sc);
        vs.Add(uitmbtn);

        ((Picker)itemhs[1]).SelectedIndex = 0;


        products_search_bar.Text = "";


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
        if (!is_safe())
        {
            DisplayAlert("Please Check quantity or Remove item or Invoice No", "Add quantity to the item", "Ok");
            return;
        }

        if(invoicebox.Text.Trim()=="")
        {
			DisplayAlert("Please Check  Invoice No", "Invoice No is mandatory", "Ok");

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
        

            MainThread.InvokeOnMainThreadAsync(async() => {

				await make_query();

				DisplayAlert("Updated!", "Your Sales has been updated", "Ok");
                Navigation.PopToRootAsync();

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
                int price = Convert.ToInt32((pname.Text.Split(':')[2].Trim()));
                string amount = (price * qty_in_int).ToString();

                // string stemp = "INSERT INTO employee_sales2 values ('" + dt.emp_id2 + "','" + snostr + "','"+qty_in_int.ToString()+"','"+amount+ "',convert_tz(now(),'-7:00','+5:30'),convert_tz(now(),'-7:00','+5:30'));";
                //string stemp = "INSERT INTO employee_sales2 values (emp_id,sno,pcs,amount,The_Time,The_date) ('" + dt.emp_id2 + "'," + snostr + "," + qty_in_int.ToString() + "," + amount + ",convert_tz(now(),'-7:00','+5:30'),convert_tz(now(),'-7:00','+5:30'));";
                string stemp = "";
                if (dpicker.SelectedIndex == 0)
                {
                    if (employee_ID == String.Empty)
                    {
                        if(type!=String.Empty && type=="defect")
                        {
                            

                            stemp = String.Format("INSERT INTO employee_defect_stocks (emp_id,sno,pcs,amount,The_Time,The_date,invoice_no) values ('{0}',{1},{2},{3},now(),now(),'{4}');", dt.emp_id2, snostr, qty_in_int.ToString(), amount,invoicebox.Text.Trim());

                        }
                        else
                        {
							
								dt.start_connection();
								List<string> inv = dt.get_employee_invoices(dt.emp_id2, "2023-1-1", DateTime.Now.ToString("yyyy-M-dd"));

								
								if (inv.Contains(invoicebox.Text.Trim(), StringComparer.InvariantCultureIgnoreCase))
								{
								bool today = dt.is_employee_invoice_today_or_yesterday(dt.emp_id2, DateTime.Now.ToString("yyyy-M-dd"), DateTime.Now.ToString("yyyy-M-dd"),invoicebox.Text.Trim());
								if (!today)
									{
									dt.close_connection();
									DisplayAlert("Stock Exists", "stock can only be entered once in store's lifetime", "OK");
										continue;
									}
								}

							dt.close_connection();

							stemp = String.Format("INSERT INTO employee_stocks (emp_id,sno,pcs,amount,The_Time,The_date,invoice_no) values ('{0}',{1},{2},{3},now(),now(),'{4}');", dt.emp_id2, snostr, qty_in_int.ToString(), amount,invoicebox.Text.Trim());

                        }

                    }
                    else if (employee_ID != String.Empty)
                    {
                        if(type!=String.Empty && type=="defect")
                        {
                            stemp = String.Format("INSERT INTO employee_defect_stocks (emp_id,sno,pcs,amount,The_Time,The_date,invoice_no) values ('{0}',{1},{2},{3},'{4}','{5}','{6}');", employee_ID, snostr, qty_in_int.ToString(), amount, "23:59:59", dpicker.ItemsSource[0],invoicebox.Text.Trim());

                        }
                        else
                        {
							dt.start_connection();
							List<string> inv = dt.get_employee_invoices(dt.emp_id2, "2023-1-1", DateTime.Now.ToString("yyyy-M-dd"));

							
							if (inv.Contains(invoicebox.Text.Trim(), StringComparer.InvariantCultureIgnoreCase))
							{
								bool today = dt.is_employee_invoice_today_or_yesterday(dt.emp_id2, DateTime.Now.ToString("yyyy-M-dd"), DateTime.Now.ToString("yyyy-M-dd"), invoicebox.Text.Trim());
								if (!today)
								{
									dt.close_connection();
									DisplayAlert("Stock Exists", "stock can only be entered once in store's lifetime", "OK");
									continue;
								}
							}

							dt.close_connection();

							stemp = String.Format("INSERT INTO employee_stocks (emp_id,sno,pcs,amount,The_Time,The_date,invoice_no) values ('{0}',{1},{2},{3},'{4}','{5}','{6}');", employee_ID, snostr, qty_in_int.ToString(), amount, "23:59:59", dpicker.ItemsSource[0],invoicebox.Text.Trim());

                        }


                    }
                }
                else if (dpicker.SelectedIndex == 1)
                {
                    if (employee_ID == String.Empty)
                    {
                        if (type != String.Empty && type == "defect")
                        {
                            stemp = String.Format("INSERT INTO employee_defect_stocks (emp_id,sno,pcs,amount,The_Time,The_date,invoice_no) values ('{0}',{1},{2},{3},'{4}','{5}','{6}');", dt.emp_id2, snostr, qty_in_int.ToString(), amount, "23:59:59", dpicker.ItemsSource[1], invoicebox.Text.Trim());

                        }
                        else
                        {
							dt.start_connection();
							List<string> inv = dt.get_employee_invoices(dt.emp_id2, "2023-1-1", DateTime.Now.ToString("yyyy-M-dd"));

							
							if (inv.Contains(invoicebox.Text.Trim(), StringComparer.InvariantCultureIgnoreCase))
							{
								bool today = dt.is_employee_invoice_today_or_yesterday(dt.emp_id2, DateTime.Now.ToString("yyyy-M-dd"), DateTime.Now.ToString("yyyy-M-dd"), invoicebox.Text.Trim());
								if (!today)
								{
									dt.close_connection();
									DisplayAlert("Stock Exists", "stock can only be entered once in store's lifetime", "OK");
									continue;
								}
							}
							dt.close_connection();


							stemp = String.Format("INSERT INTO employee_stocks (emp_id,sno,pcs,amount,The_Time,The_date,invoice_no) values ('{0}',{1},{2},{3},'{4}','{5}','{6}');", dt.emp_id2, snostr, qty_in_int.ToString(), amount, "23:59:59", dpicker.ItemsSource[1], invoicebox.Text.Trim());

                        }

                    }
                    else if (employee_ID != String.Empty)
                    {

                        if (type != String.Empty && type == "defect")
                        {
                            stemp = String.Format("INSERT INTO employee_defect_stocks (emp_id,sno,pcs,amount,The_Time,The_date,invoice_no) values ('{0}',{1},{2},{3},'{4}','{5}','{6}');", employee_ID, snostr, qty_in_int.ToString(), amount, "23:59:59", dpicker.ItemsSource[1],invoicebox.Text.Trim());

                        }
                        else
                        {
							dt.start_connection();
							List<string> inv = dt.get_employee_invoices(dt.emp_id2, "2023-1-1", DateTime.Now.ToString("yyyy-M-dd"));

							
							if (inv.Contains(invoicebox.Text.Trim(), StringComparer.InvariantCultureIgnoreCase))
							{
								bool today = dt.is_employee_invoice_today_or_yesterday(dt.emp_id2, DateTime.Now.ToString("yyyy-M-dd"), DateTime.Now.ToString("yyyy-M-dd"), invoicebox.Text.Trim());
								if (!today)
								{
									dt.close_connection();
									DisplayAlert("Stock Exists", "stock can only be entered once in store's lifetime", "OK");
									continue;
								}
							}

							dt.close_connection();

							stemp = String.Format("INSERT INTO employee_stocks (emp_id,sno,pcs,amount,The_Time,The_date,invoice_no) values ('{0}',{1},{2},{3},'{4}','{5}','{6}');", employee_ID, snostr, qty_in_int.ToString(), amount, "23:59:59", dpicker.ItemsSource[1], invoicebox.Text.Trim());

                        }


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

	private async void goback_btn_Clicked(object sender, EventArgs e)
	{


		await Navigation.PopToRootAsync();

	}

	private void Invoice_picker_SelectedIndexChanged(object sender, EventArgs e)
	{
        invoicebox.Text=Invoice_picker.SelectedItem as string;
        invoicebox.Text.Trim();
	}

	
}