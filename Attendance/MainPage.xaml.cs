using Attendance.Data;
using Attendance.Pages;
using The_Attendance.Interfaces;
#if ANDROID 
using The_Attendance.Platforms;
#endif
namespace Attendance;

public partial class MainPage : ContentPage
{
	int count = 0;


	public MainPage()
	{
		InitializeComponent();
#if ANDROID
		DependencyService.Register<IAndroid,AndroidLocationService>();

		if(DependencyService.Resolve<IAndroid>().IsForeGroundServiceRunning())
		{
			Clkin.Text = "Clock-Out";
		}

	

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
				target_remaning.Text = rem_sales.ToString() + " needed to achive target";
				 
				else target_remaning.Text = " This months target achived ";

			});

		});

	}

	private async void OnCounterClicked(object sender, EventArgs e)
	{
#if ANDROID

		Navigation.PushAsync(new Pages.UpdateSales());


#endif
	}

	private void Clkin_Clicked(object sender, EventArgs e)
	{
#if ANDROID

		if (Clkin.Text == "Clock-In")
		{

			DependencyService.Resolve<IAndroid>().StartMyService();
			Clkin.Text = "Clock-Out";
		}
		else if (Clkin.Text == "Clock-Out" && DependencyService.Resolve<IAndroid>().IsForeGroundServiceRunning())
		{
			DependencyService.Resolve<IAndroid>().StopMyService();
			Clkin.Text = "Clock-In";
		}

#endif

	}

	private async void UpdateRecentSales_Clicked(object sender, EventArgs e)
	{


		Navigation.PushAsync(new ViewRecentSales());

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
					target_remaning.Text = rem_sales.ToString() + " needed to achive your target";

				else target_remaning.Text = " This month's target achived ";

			});

		});
	}

	private void ContentPage_NavigatedFrom(object sender, NavigatedFromEventArgs e)
	{

	}
}

