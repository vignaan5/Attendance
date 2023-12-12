using Attendance.Data;

namespace Attendance.Pages;

public partial class UpdateAvailable : ContentPage
{
	public DataClass dt = new DataClass();
	public UpdateAvailable()
	{
		InitializeComponent();
   
   }

	private async void dupdatebtn_Clicked(object sender, EventArgs e)
	{
		dt.start_connection();
		string url=dt.get_url_of_new_apk();
		dt.close_connection();

		if(url != null && url!="")
		{
			try
			{
				Uri uri = new Uri(url);
				await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
			}
			catch (Exception ex)
			{
				DisplayAlert("Error", ex.Message.ToString(), "Ok");
			}
		}
		else
		{
			DisplayAlert("Error","NothingFound", "Ok");
		}

	}
}