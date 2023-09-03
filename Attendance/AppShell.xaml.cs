namespace Attendance;

public partial class AppShell : Shell
{
	public string emp_id { get; set; }	
	public string user_naam { get; set; }
	public AppShell()
	{
		InitializeComponent();
       
		
	}

	public AppShell(string emp_id,string user_naam)
	{
		
		InitializeComponent();
		Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
		this.Items[0].Title = user_naam;
		 
	}


}
