using Attendance.Data;

namespace Attendance.Pages;

public partial class GeoLocationSettings : ContentPage
{
	public DataClass dt = new DataClass();
	public List<int> settings = new List<int>();
	public static bool got_settings_from_db = false;
	public GeoLocationSettings()
	{
		InitializeComponent();
		dt.start_connection();
	    List<int> settings = dt.get_setting_values();
		dt.close_connection();
		if (settings[0]==1)
		{
			adminswitch.IsToggled = true;
		}
		if (settings[1]==1)
		{
			zonalswitch.IsToggled= true;
		}
		if (settings[2]==1)
		{
			areasaleswitch.IsToggled= true;
		}
		if (settings[3]==1)
		{
			supervisorswitch.IsToggled = true;
		}
		if (settings[4]==1)
		{
			beautyswitch.IsToggled = true;
		}

	}


	public void get_setting_from_db()
	{
		dt.start_connection();
		 settings = dt.get_setting_values();
		dt.close_connection();
	}

	public void update_settings()
	{
		dt.start_connection();
		dt.update_setting_values(settings);
		dt.close_connection();
	}


	private void adminswitch_Toggled(object sender, ToggledEventArgs e)
	{
		
			get_setting_from_db();
		
		settings[0] = Convert.ToInt32(adminswitch.IsToggled);
		dt.start_connection();
		dt.update_setting_values(settings);
		dt.close_connection();
	}

	private void zonalswitch_Toggled(object sender, ToggledEventArgs e)
	{
		
			get_setting_from_db();
		
		settings[1] = Convert.ToInt32(zonalswitch.IsToggled);
		dt.start_connection();
		dt.update_setting_values(settings);
		dt.close_connection();
	}

	private void areasaleswitch_Toggled(object sender, ToggledEventArgs e)
	{
		
			get_setting_from_db();
		
		settings[2] = Convert.ToInt32(areasaleswitch.IsToggled);
		dt.start_connection();
		dt.update_setting_values(settings);
		dt.close_connection();
	}

	private void beautyswitch_Toggled(object sender, ToggledEventArgs e)
	{
		
			get_setting_from_db();
		
		settings[4] = Convert.ToInt32(beautyswitch.IsToggled);
		dt.start_connection();
		dt.update_setting_values(settings);
		dt.close_connection();
	}

	private void supervisorswitch_Toggled(object sender, ToggledEventArgs e)
	{
		
			get_setting_from_db();
		
		settings[3] = Convert.ToInt32(supervisorswitch.IsToggled);
		dt.start_connection();
		dt.update_setting_values(settings);
		dt.close_connection();
	}
}