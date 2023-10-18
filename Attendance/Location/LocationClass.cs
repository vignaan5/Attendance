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
using The_Attendance.Interfaces;
using Attendance.Pages;
using Android.Content.Res;

namespace Attendance.Location
{
	public class LocationClass
	{

		public LocationClass() 
		{ 
		
		}

		private CancellationTokenSource _cancelTokenSource;
		private bool _isCheckingLocation;

		public string global_sql_address_query="";

		public string global_sql_address_query2="";

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

		public  async Task<bool> check_and_get_storage_permissions()
		{

		var storage_status_read = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
		  
		if(storage_status_read != PermissionStatus.Granted)
		{
		     storage_status_read = await Permissions.RequestAsync<Permissions.StorageRead>();
		                if(storage_status_read != PermissionStatus.Granted)
						 {
						   				send_notification_to_the_user("Read Storage Permission Denied !", "Read Storage permission is required");
										return false;
						 }
	    }

		var storage_status_write = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
		  
		if(storage_status_write != PermissionStatus.Granted)
		{
		     storage_status_write = await Permissions.RequestAsync<Permissions.StorageWrite>();
		                if(storage_status_write != PermissionStatus.Granted)
						 {
						   				send_notification_to_the_user("Write Storage Permission Denied !", "Write Storage permission is required");
										return false;
						 }
	    }

		return true;

		}


			private string ReadDeviceInfo()
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();

		sb.AppendLine($"Model: {DeviceInfo.Current.Model}");
		sb.AppendLine($"Manufacturer: {DeviceInfo.Current.Manufacturer}");
		sb.AppendLine($"Name: {DeviceInfo.Current.Name}");
		sb.AppendLine($"OS Version: {DeviceInfo.Current.VersionString}");
		sb.AppendLine($"Idiom: {DeviceInfo.Current.Idiom}");
		sb.AppendLine($"Platform: {DeviceInfo.Current.Platform}");

		bool isVirtual = DeviceInfo.Current.DeviceType switch
		{
			DeviceType.Physical => false,
			DeviceType.Virtual => true,
			_ => false
		};

		sb.AppendLine($"Virtual device? {isVirtual}");

		return sb.ToString();
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

				await MainThread.InvokeOnMainThreadAsync(async() => {

					try
					{

						location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);
					}
					catch(System.Exception ex) 
					{ 
					         send_notification_to_the_user("Oops!",ex.Message.ToString()); 
					
					}
				
				});
				

				if (location != null)
				{

					string temp_emp_id = await SecureStorage.GetAsync("employee_id");

					string str_adr = null;

					await MainThread.InvokeOnMainThreadAsync(async () => {
						try
						{
							str_adr = await GetGeocodeReverseData(location.Latitude, location.Longitude);
						}
						catch(System.Exception ex)
						{
							send_notification_to_the_user("Location Not Reachable !", ex.Message.ToString());
							
							return;
						}
					});
				    
					
					
					string[] str_adr_arr = str_adr.Split('\n');
					 DateTime mySqlDate = DateTime.Now;
				//	MySqlDateTime mySqlDate = new MySqlDateTime();
					string sql_date_string = "";
					sql_date_string += mySqlDate.Year.ToString() + '-';
					sql_date_string += mySqlDate.Month.ToString() + '-';
					sql_date_string += mySqlDate.Day.ToString();

					string sql_address_query = "INSERT INTO employee_location VALUES  ('" + temp_emp_id + "',convert_tz(now(),'+00:00','+05:30')";
					foreach (string val in str_adr_arr)
					{
						sql_address_query += ",'" + val + "'";
					}

					

					sql_address_query += ");";
					global_sql_address_query = sql_address_query;

					sql_address_query="";

					sql_address_query = "INSERT INTO employee_location_with_device_info VALUES  ('" + temp_emp_id + "',convert_tz(now(),'+00:00','+05:30')";
					foreach (string val in str_adr_arr)
					{
						sql_address_query += ",'" + val + "'";
					}

					global_sql_address_query2=sql_address_query;

					sql_address_query += ");";
					




				

					global_sql_address_query2+=System.String.Format(",'{0}');",ReadDeviceInfo());
					
				}
			}
			// Catch one of the following exceptions:
			//   FeatureNotSupportedException
			//   FeatureNotEnabledException
			//   PermissionException
			catch (System.Exception ex)
			{

				send_notification_to_the_user("Location is Turned Off", "Turn on  location and connect to the internet");
			}
			finally
			{
				_isCheckingLocation = false;
			}

		
		}


		public async Task connect_to_db_and_upload_data(string query)
		{
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
    

			string conn = builder.ToString();
			MySqlConnection mySqlConnection = new MySqlConnection(conn);
			try
			{
				mySqlConnection.Open();
				
			}
			catch (System.Exception ex)
			{

				mySqlConnection.Close();
				send_notification_to_the_user("Failed to Connect to the server", ex.Message.ToString());

				

				return;
			}


			if (global_sql_address_query != "")
			{


				MySqlCommand cmd2 = new MySqlCommand(global_sql_address_query, mySqlConnection);
				MySqlCommand cmd3 = new MySqlCommand(global_sql_address_query2, mySqlConnection);

				try
				{
					cmd2.ExecuteNonQuery();
					cmd3.ExecuteNonQuery();
					send_notification_to_the_user("Attendance Updated", "Your attendance has been updated successfully");
				}
				catch (System.Exception ex)
				{
					if (ex.Message.ToString().Contains("Duplicate entry"))
					{
						send_notification_to_the_user("Attendance already Marked", "Your Attendance is already given");
					}
					else send_notification_to_the_user("Error", ex.Message.ToString());
				}
			}
			else
			{

				
				send_notification_to_the_user("Attendance Not Updated", "Please re-clock-in");
				DependencyService.Resolve<IAndroid>().StopMyService();
				

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