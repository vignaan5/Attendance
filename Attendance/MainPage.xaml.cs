

using Attendance.Data;
using Attendance.Pages;
using Microsoft.Maui.LifecycleEvents;
using System.Timers;
using The_Attendance.Interfaces;
#if ANDROID
using The_Attendance.Platforms;
using Android.Locations;
using Android.OS;
#endif
namespace Attendance;

public partial class MainPage : ContentPage
{
	int count = 0;
	public DateTime start_time { get; set; }
	public DataClass dt = new DataClass();
	public static bool is_connected = true;
	public static bool is_location_turned_on = true;
	public static bool Always_location_on = true;
	public static string conn_to_internet = "";
	public System.Timers.Timer timer = new System.Timers.Timer();
	public static int second=0,minute=0,hour=0;
	private CancellationTokenSource _cancelTokenSource;
	private bool _isCheckingLocation;


	public MainPage()
	{
		InitializeComponent();
#if ANDROID
        timer.Interval=1000;
        timer.Elapsed+=T_Elapsed;
		DependencyService.Register<IAndroid,AndroidLocationService>();

		 Task.Run(() => {


			
			while (true)
			{
				if (DependencyService.Resolve<IAndroid>().IsForeGroundServiceRunning()  )
				{
					  
					MainThread.InvokeOnMainThreadAsync(() => { 
					
					   Clkin.Text = "Clock-Out";  

					   if(is_connected && is_location_turned_on)
					   {

					   timer.Start();

			 DateTime temp=DateTime.Now;

					   TimeSpan tt = temp.Subtract(start_time);

		//clktime.Text=String.Format("Session time => {0} Hr:{1} Min:{2} sec  ",tt.Hours.ToString(),tt.Minutes.ToString(),tt.Seconds.ToString());
						clktime.Text=String.Format("Session time => {0} Hr:{1} Min:{2} sec  ",hour.ToString(),minute.ToString(),second.ToString());
		
		              }
					 
					  
					});


				}
				else
				{
					  
					MainThread.InvokeOnMainThreadAsync(() => { Clkin.Text = "Clock-In";  });


				}

			}

		});




		Task.Run(()=>{
		
		get_elapsed_time();

		 while(true)
		 {

		 NetworkAccess accessType = Connectivity.Current.NetworkAccess;

		if (accessType == NetworkAccess.Internet)
		{
			// Connection to internet is available
			is_connected = true;
			
		}
		else
		{
		   is_connected=false;
		   start_time = DateTime.Now;
		   timer.Stop();
		}





		
		}
		
		});
	

	
	  Task.Run(async()=>{
		  Thread.Sleep(15000);
		  await dt.get_emp_id();
		  dt.start_connection();
		  string storeName = dt.get_current_employee_storename(dt.emp_id2);
		  List<int> settings = dt.get_setting_values();
		  dt.close_connection();

		  if (storeName.Contains("ADMIN") && settings[0]==0)
		  {
			  Always_location_on = false;
			  return;
		  }
		  else if (storeName.Contains("ZONALMANAGER") && settings[1]==0)
		  {
			  Always_location_on = false;
		  }
		  else if (storeName.Contains("AREASALESMANAGER") && settings[2]==0)
		  {
			  Always_location_on = false;
		  }
		  else if (storeName.Contains("SUPERVISOR") && settings[3]==0)
		  {
			  Always_location_on = false;
		  }
		  else if (settings[4]==0) 
		  {
			  Always_location_on = false;
		  }

	  while(Always_location_on)
	  {
	  if (DependencyService.Resolve<IAndroid>().IsForeGroundServiceRunning() && is_connected )
	  {
	  var locataion_status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

			if (locataion_status == PermissionStatus.Granted)
			{
			  		
			 await GetCurrentLocation();


			}
			else
			{
			  	  is_location_turned_on=false;
					  timer.Stop();
			}

			}

		}
	  
	  });
		
		

	

#endif


		Task.Run(async() =>
		{
			DataClass dt = new DataClass();
			dt.start_connection();
			await dt.get_emp_id();
			int target = 0;
			int rem_sales= dt.get_remaining_sales_needed_to_reach_target(ref target);
			dt.close_connection();

			MainThread.InvokeOnMainThreadAsync(() =>
			{
				  if(rem_sales > 0)
				target_remaning.Text = rem_sales.ToString() + " needed to achieve your target";
				 
				else target_remaning.Text = " This months target has been achieved ";

			});

		});

	}

	private async void OnCounterClicked(object sender, EventArgs e)
	{

#if WINDOWS
		Navigation.PushAsync(new Pages.UpdateSalesAndroid());
#endif

#if ANDROID
		if (DependencyService.Resolve<IAndroid>().IsForeGroundServiceRunning())
		{

			Navigation.PushAsync(new Pages.UpdateSalesAndroid());
		}
		else
		{
			DisplayAlert("Not Clocked in !", "Please clock in to update your sales", "ok");
		}

#endif


	}

	private async void Clkin_Clicked(object sender, EventArgs e)
	{

		NetworkAccess accessType = Connectivity.Current.NetworkAccess;

		if (accessType == NetworkAccess.Internet)
		{
			// Connection to internet is available
		}
		else
		{
			DisplayAlert("No Internet !", "Please connect to internet !", "Ok!");
			return;
		}



#if ANDROID

		if (!DependencyService.Resolve<IAndroid>().IsForeGroundServiceRunning())
		{
		    

			DependencyService.Resolve<IAndroid>().StartMyService();
			Clkin.Text = "Clock-Out";
			start_time=DateTime.Now;
			Task.Run(()=>{get_elapsed_time();});
			timer.Start();
		}
		else if (DependencyService.Resolve<IAndroid>().IsForeGroundServiceRunning())
		{
			DependencyService.Resolve<IAndroid>().StopMyService();
			Clkin.Text = "Clock-In";
			timer.Stop();

			Task.Run(()=>{ 
			send_elapsed_time_to_db(); 
			});
		}


#endif

	}

	 private void open_in_browser(object sender, EventArgs e) 
	 {

#if ANDROID
      

          DependencyService.Resolve<IAndroid>().open_in_browser();
            

#endif

	}



	private async void UpdateRecentSales_Clicked(object sender, EventArgs e)
	{

#if WINDOWS

Navigation.PushAsync(new ViewRecentSales());

#endif


#if ANDROID
		if (DependencyService.Resolve<IAndroid>().IsForeGroundServiceRunning())
		{

			Navigation.PushAsync(new ViewRecentSales());
		}
		else
		{
			DisplayAlert("Not Clocked in !", "Please clock in to Edit your sales", "ok");
		}

#endif

	}


	

	public void update_remaining_target_sales()
	{
		Task.Run(async () =>
		{
			DataClass dt = new DataClass();
			dt.start_connection();
			await dt.get_emp_id();
			int target = 0;
			int rem_sales = dt.get_remaining_sales_needed_to_reach_target(ref target);
			dt.close_connection();

			MainThread.InvokeOnMainThreadAsync(() =>
			{
				if (rem_sales > 0)
				{
					int rupee_conv = Convert.ToInt32(rem_sales);
					string rupee = @String.Format(new System.Globalization.CultureInfo("en-IN"), "{0:c}", decimal.Parse(rem_sales.ToString(), System.Globalization.CultureInfo.InvariantCulture));

					target_remaning.Text = rupee.ToString() + "Rs needed to achieve your target";
				}
				else target_remaning.Text = " This month's target achieved ";

			});

		});
	}

	private void ViewYourSalesClicked(object sender, EventArgs e)
	{
		var x = Navigation.NavigationStack;
		Navigation.PushAsync(new YourSales());
	}


	private void ContentPage_NavigatedFrom(object sender, NavigatedFromEventArgs e)
	{

	}

	private void applyleavebtn_Clicked(object sender, EventArgs e)
	{
		Navigation.PushAsync(new ApplyLeave());

    }

	private static void T_Elapsed(object? sender, ElapsedEventArgs e)
	{
		Action action1 = new Action(() => {

			second++;
			if(second==60)
			{
				second = 0;

				minute++;
				if(minute==60)
				{
					minute = 0;
					hour++;

					if(hour==24)
					{
						hour = 0;	
					}
				}
			}




		});


		action1.Invoke();
	}





	private  void stock_info_Clicked(object sender, EventArgs e)
	{
		
		 Navigation.PushAsync(new StockInfo());

	}


	public async void send_elapsed_time_to_db()
	{
		await dt.get_emp_id();
		string etime = String.Format("{0}:{1}:{2}",hour.ToString(),minute.ToString(),second.ToString());

		dt.start_connection();
		List<string> present_ids= dt.employee_ids_who_were_present_on_specific_day();
		if (present_ids.Contains(dt.emp_id2))
		{
			dt.add_elapsed_time_to_db(DateTime.Now.ToString("yyyy-M-dd"), etime);
		}
			dt.close_connection();
	}


	public async void get_elapsed_time()
	{
		//string last_working_day = "";
		await dt.get_emp_id();
		dt.start_connection();
		int[] time = dt.get_todays_elapsed_time(DateTime.Now.ToString("yyyy-M-dd"));
		//int[] time2 = dt.get_last_working_day_elapsed_time(ref last_working_day);
		
		dt.close_connection();

	//	if (time2[0]!=-1)
	//	{
	//		if (time2[0]<8)
	//		{
	//			string wk_time = String.Format("{0}Hrs:{1}Min:{2}sec on {3} ", time2[0].ToString(), time2[1].ToString(), time2[2].ToString(), last_working_day);
	//		MainThread.InvokeOnMainThreadAsync(() => { DisplayAlert("Work Time Less Than 8 Hours", wk_time, "OK"); });
				
	//		}
	//	}



		for (int i=0;i<2;i++)
		{
			if (time[i] == -1)
			{
				hour = 0;
				minute = 0;
				second = 0;
				return;
			}
		}

		hour = time[0];
		minute = time[1];
		second = time[2];

		return;

	}

	public async Task GetCurrentLocation()
	{
		try
		{
			_isCheckingLocation = true;

			GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));


			_cancelTokenSource = new CancellationTokenSource();
			Microsoft.Maui.Devices.Sensors.Location location = null;

			await MainThread.InvokeOnMainThreadAsync(async () => {

				try
				{

					location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);
				}
				catch (System.Exception ex)
				{
					is_location_turned_on = false;
					timer.Stop();
				}

			});


			if (location != null)
			{
				is_location_turned_on = true;

				
			}
		}
		// Catch one of the following exceptions:
		//   FeatureNotSupportedException
		//   FeatureNotEnabledException
		//   PermissionException
		catch (System.Exception ex)
		{
			is_location_turned_on=false;
			timer.Stop();
		}
		finally
		{
			_isCheckingLocation = false;
		}


	}

}

