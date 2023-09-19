using Attendance.Data;

namespace Attendance.Pages;

public partial class ViewEmployeeLocations : ContentPage
{
	public DataClass dt = new DataClass();
	List<List<string>> emp_locations = new List<List<string>>();	
	public ViewEmployeeLocations()
	{
		InitializeComponent();
	}

	private async void Button_Clicked(object sender, EventArgs e)
	{
		actind.IsVisible = true;
	  await	Task.Run(() => {

		  dt.start_connection();
		 emp_locations= dt.get_employee_location_details_on_a_specific_day(emp_id_entry.Text.ToString().Trim(),dtpicker.Date.ToString("yyyy-MM-dd"));
		  dt.close_connection();
		
		});

		
		vs.Clear();
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







			}

			Button mapsbutton = new Button { Text = "View On Maps", HorizontalOptions = LayoutOptions.Center };

			mapsbutton.Clicked += (async (object sender,EventArgs e) => {

				double latitude = Convert.ToDouble(location_details[location_details.Count-2]);
				double longitude = Convert.ToDouble(location_details[location_details.Count - 1]);
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


	
}