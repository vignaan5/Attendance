using Attendance.Data;

namespace Attendance.Pages;

public partial class ViewEmployeeLocations : ContentPage
{
	public string emp_id { get; set; }

	DataClass dt = new DataClass();
	public List<string> nothing_found_temp = new List<string> { "Nothing Found" };
	
	List<List<string>> emp_locations = new List<List<string>>();	
	public ViewEmployeeLocations()
	{
		InitializeComponent();
		emplist.ItemsSource = nothing_found_temp;
		emplist.IsVisible = true;
	}

	private async void Button_Clicked(object sender, EventArgs e)
	{
		actind.IsVisible = true;
	  await	Task.Run(() => {

		  dt.start_connection();
		 emp_locations= dt.get_employee_location_details_on_a_specific_day_with_device_info(emp_id,dtpicker.Date.ToString("yyyy-MM-dd"));
		  dt.close_connection();
		
		});

		
		vs.Clear();

		if(emp_locations.Count==0)
		{
			DisplayAlert("Nothing Found !", "No Location Details Found for this employee", "Ok");
			return;
		}

		vs.MinimumWidthRequest = 500;
		vs.HorizontalOptions= LayoutOptions.Center;

		foreach(List<string> location_details in emp_locations) 
		{
			VerticalStackLayout tempvs = new VerticalStackLayout { Spacing=10};
		     for(int i=0;i<location_details.Count;i++) 
			{

				if(i==0)
				{
					Label lbl = new Label { Text = "Employee_id :" + location_details[i], FontSize = 10 };
					tempvs.Add(lbl);
				}

			 else 	if (i==1)
				{
					Label lbl = new Label { Text= "Date and Time :" + location_details[i],FontSize=10 };
					tempvs.Add(lbl);
				}
				else if(i==2)
				{
					Label lbl = new Label { Text = "Admin Area :" + location_details[i] };
					tempvs.Add(lbl);
				}
				else if(i==3)
				{
					Label lbl = new Label { Text = "Country Code :" + location_details[i] };
					tempvs.Add(lbl);
				}
				else if(i==4)
				{
					Label lbl = new Label { Text = "Country Name:" + location_details[i] };
					tempvs.Add(lbl);
				}
				else if(i==5)
				{
					Label lbl = new Label { Text = "FeatureName :" + location_details[i] };
					tempvs.Add(lbl);
				}
				else if (i==6)
				{

					Label lbl = new Label { Text = "Locality :" + location_details[i] };
					tempvs.Add(lbl);

				}
				else if (i == 7)
				{

					Label lbl = new Label { Text = "PostalCode :" + location_details[i] };
					tempvs.Add(lbl);

				}

				else if (i == 8)
				{

					Label lbl = new Label { Text = "SubAdminArea :" + location_details[i] };
					tempvs.Add(lbl);

				}
				else if (i == 9)
				{

					Label lbl = new Label { Text = "SubLocality :" + location_details[i] };
					tempvs.Add(lbl);

				}

				else if (i == 10)
				{

					Label lbl = new Label { Text = "SubThoroughfare :" + location_details[i] };
					tempvs.Add(lbl);

				}
				else if (i == 11)
				{

					Label lbl = new Label { Text = "Thoroughfare :" + location_details[i] };
					tempvs.Add(lbl);

				}
				else if (i == 12)
				{

					Label lbl = new Label { Text = "Latitude :" + location_details[i] };
					tempvs.Add(lbl);

				}
				else if (i == 13)
				{

					Label lbl = new Label { Text = "Longitude :" + location_details[i] };
					tempvs.Add(lbl);

				}
				else if(i== 14) 
				{
					Label lbl = new Label { Text = "Device Info :\n" + location_details[i] };
					tempvs.Add(lbl);

				}

			





			}

			Button mapsbutton = new Button { Text = "View On Maps", HorizontalOptions = LayoutOptions.Center };

			mapsbutton.Clicked += (async (object sender,EventArgs e) => {

				double latitude = Convert.ToDouble(location_details[location_details.Count-3]);
				double longitude = Convert.ToDouble(location_details[location_details.Count - 2]);
				Microsoft.Maui.Devices.Sensors.Location l = new Microsoft.Maui.Devices.Sensors.Location(latitude, longitude);


				//string locationName = FeatureName.Text+","+localityLable.Text;

				string mapsUri = $"geo:{latitude},{longitude}?q={Uri.EscapeDataString("")}";



				try
				{
					//   await Launcher.OpenAsync(new Uri(mapsUri));

					await Map.Default.OpenAsync(latitude, longitude);
				}
				catch (Exception ex)
				{
					// Handle any exceptions if necessary
					Console.WriteLine($"Error opening maps: {ex.Message}");
				}

			});

			tempvs.Add(mapsbutton);	

			Frame f = new Frame { Content=tempvs };
			vs.Add(f);
		}


	}

	private void emplis_ItemSelected(object sender, SelectedItemChangedEventArgs e)
	{
		if ((string)emplist.SelectedItem == "Nothing Found")
			return;

		this.emp_id = (string)emplist.SelectedItem;
		this.emp_id= this.emp_id.Split(' ')[0];
		searchvs.Clear();
		
		getlcnbtn.IsVisible=true;
		getlcnbtn.IsEnabled = true;
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

}