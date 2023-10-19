

using Attendance.Data;
using Attendance.Pages;
using Microsoft.Maui.LifecycleEvents;
using The_Attendance.Interfaces;
#if ANDROID
using The_Attendance.Platforms;

#endif
namespace Attendance;

public partial class MainPage : ContentPage
{
	int count = 0;
	public DateTime start_time { get; set; }

	public MainPage()
	{
		InitializeComponent();
#if ANDROID
		DependencyService.Register<IAndroid,AndroidLocationService>();

		 Task.Run(() => {
			
			while (true)
			{
				if (DependencyService.Resolve<IAndroid>().IsForeGroundServiceRunning()  )
				{
					  
					MainThread.InvokeOnMainThreadAsync(() => { 
					
					   Clkin.Text = "Clock-Out";  
					   
					   DateTime temp=DateTime.Now;

					   TimeSpan tt = temp.Subtract(start_time);

		clktime.Text=String.Format("Session time => {0} Hr:{1} Min:{2} sec  ",tt.Hours.ToString(),tt.Minutes.ToString(),tt.Seconds.ToString());
					
					});


				}
				else
				{
					  
					MainThread.InvokeOnMainThreadAsync(() => { Clkin.Text = "Clock-In";  });


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

	private void Clkin_Clicked(object sender, EventArgs e)
	{
#if ANDROID

		if (!DependencyService.Resolve<IAndroid>().IsForeGroundServiceRunning())
		{

			DependencyService.Resolve<IAndroid>().StartMyService();
			Clkin.Text = "Clock-Out";
			start_time=DateTime.Now;
		}
		else if (DependencyService.Resolve<IAndroid>().IsForeGroundServiceRunning())
		{
			DependencyService.Resolve<IAndroid>().StopMyService();
			Clkin.Text = "Clock-In";
			
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
		Navigation.PushAsync(new YourSales());
	}


	private void ContentPage_NavigatedFrom(object sender, NavigatedFromEventArgs e)
	{

	}

	private void applyleavebtn_Clicked(object sender, EventArgs e)
	{
		Navigation.PushAsync(new ApplyLeave());

    }
}

