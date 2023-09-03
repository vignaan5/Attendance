#if ANDROID
using Android.App;
using Android.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using and1 = Android;
using AndroidX;
using AndroidX.Core.App;
using Microsoft.Maui.Devices.Sensors;
using Android;
using Android.App;
using Android.Content;
using Android.Gestures;
using Android.OS;
using Android.Runtime;
using AndroidX;
using AndroidX.Core.App;
using Java.Lang;
using AndroidX.Core;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific.AppCompat;
using Microsoft.Maui.Controls;
using Android.Systems;
using Android.Graphics;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;
using MySqlConnector;
using Android.Media;
using Android.Service;
using Java.Sql;


namespace Attendance.Location
{
	public class LocationClass
	{

		public LocationClass() 
		{ 
		
		}

		private CancellationTokenSource _cancelTokenSource;
		private bool _isCheckingLocation;

		public string global_sql_address_query;

		const string channel_id = "default";
		const string channel_name = "Default";
		int notify_index = 0;
		public void send_notification_to_the_user(string title, string message)
		{
			NotificationManager manager = (NotificationManager)and1.App.Application.Context.GetSystemService(and1.App.Application.NotificationService);

			var channelNameJava = new Java.Lang.String(channel_name);

			if (and1.OS.Build.VERSION.SdkInt >= and1.OS.BuildVersionCodes.O)
			{
				var channel = new NotificationChannel(channel_id, channelNameJava, NotificationImportance.High)
				{
					Description = "Channel Description"
				};

				manager.CreateNotificationChannel(channel);
			}

			NotificationCompat.Builder builder = new NotificationCompat.Builder(and1.App.Application.Context, channel_id).SetContentTitle(title)
				.SetContentText(message)
				.SetLargeIcon(BitmapFactory.DecodeResource(and1.App.Application.Context.Resources, Resource.Drawable.mtrl_checkbox_button_icon))
				.SetSmallIcon(Resource.Drawable.notify_panel_notification_icon_bg)
				.SetPriority((int)NotificationPriority.High)
				.SetVisibility((int)NotificationVisibility.Public)
				.SetDefaults((int)NotificationDefaults.Sound | (int)NotificationDefaults.Vibrate);

			Notification notification = builder.Build();

			manager.Notify(notify_index++, notification);


		}


		public async Task<string> GetGeocodeReverseData(double latitude = 47.673988, double longitude = -122.121513)
		{
			IEnumerable<Placemark> placemarks = await Geocoding.Default.GetPlacemarksAsync(latitude, longitude);

			Placemark placemark = placemarks?.FirstOrDefault();

			if (placemark != null)
			{
				return
					$"{placemark.AdminArea}\n" +
					$" {placemark.CountryCode}\n" +
					$"{placemark.CountryName}\n" +
					$"{placemark.FeatureName}\n" +
					$"{placemark.Locality}\n" +
					$"{placemark.PostalCode}\n" +
					$"{placemark.SubAdminArea}\n" +
					$"{placemark.SubLocality}\n" +
					$"{placemark.SubThoroughfare}\n" +
					$"{placemark.Thoroughfare}\n" +
					$"{placemark.Location.Latitude}\n" +
					$"{placemark.Location.Longitude}";
			}

			return "";
		}


		public async Task<bool> check_and_get_permissions()

		{

			var locataion_status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

			if (locataion_status != PermissionStatus.Granted)
			{
				locataion_status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

			}

			if (locataion_status != PermissionStatus.Granted)
			{

				send_notification_to_the_user("Location Denied !", "Location permission is required");

				return false;
			}


			return true;

		}

		public async Task GetCurrentLocation()
		{
			try
			{
				_isCheckingLocation = true;

				GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));


				_cancelTokenSource = new CancellationTokenSource();
				Microsoft.Maui.Devices.Sensors.Location location = null;

				await MainThread.InvokeOnMainThreadAsync(async() => {  location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token); });
				

				if (location != null)
				{

					string temp_emp_id = await SecureStorage.GetAsync("employee_id");

					string str_adr = null;

					await MainThread.InvokeOnMainThreadAsync(async () => { str_adr = await GetGeocodeReverseData(location.Latitude, location.Longitude); });
				    
					
					
					string[] str_adr_arr = str_adr.Split('\n');
					 DateTime mySqlDate = DateTime.Now;
				//	MySqlDateTime mySqlDate = new MySqlDateTime();
					string sql_date_string = "";
					sql_date_string += mySqlDate.Year.ToString() + '-';
					sql_date_string += mySqlDate.Month.ToString() + '-';
					sql_date_string += mySqlDate.Day.ToString();

					string sql_address_query = "INSERT INTO employee_location VALUES  ('" + temp_emp_id + "',convert_tz(now(),'-7:00','+5:30'),convert_tz(now(),'-7:00','+5:30')";
					foreach (string val in str_adr_arr)
					{
						sql_address_query += ",'" + val + "'";
					}

					sql_address_query += ");";
					global_sql_address_query = sql_address_query;

				}
			}
			// Catch one of the following exceptions:
			//   FeatureNotSupportedException
			//   FeatureNotEnabledException
			//   PermissionException
			catch (System.Exception ex)
			{

				send_notification_to_the_user("Location is Turned Off", "Turnon the location to send data");
			}
			finally
			{
				_isCheckingLocation = false;
			}

		
		}


		public async Task connect_to_db_and_upload_data(string query)
		{

			string conn = "Server=MYSQL8002.site4now.net;Database=db_a9daf3_vignaan;Uid=a9daf3_vignaan;Pwd=gyanu@18;SSL MODE = None;";
			MySqlConnection mySqlConnection = new MySqlConnection(conn);
			try
			{
				mySqlConnection.Open();

			}
			catch (System.Exception ex)
			{

				mySqlConnection.Close();
				send_notification_to_the_user("Exception Thrown", ex.Message.ToString());

				return;
			}


			if (global_sql_address_query != "")
			{


				MySqlCommand cmd2 = new MySqlCommand(global_sql_address_query, mySqlConnection);
				try
				{
					cmd2.ExecuteNonQuery();
					send_notification_to_the_user("Attendance Updated", "Your attendance has been updated successfully");
				}
				catch (System.Exception ex)
				{
					if(ex.Message.ToString().Contains("Duplicate entry"))
					{
						send_notification_to_the_user("Attendance already Marked", "Your Attendance is already given");
					}
					else send_notification_to_the_user("Error", ex.Message.ToString());
				}
			}


			try
			{
				mySqlConnection.Close();

			}
			catch (System.Exception ex)
			{


				send_notification_to_the_user("Exception Thrown", ex.Message.ToString());

				return;
			}


		}







		public async void Execute_process()
		{
			await GetCurrentLocation();


			await connect_to_db_and_upload_data(global_sql_address_query);

			
		}

	
	}

}
#endif