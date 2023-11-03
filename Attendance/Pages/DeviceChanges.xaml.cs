using Attendance.Data;

namespace Attendance.Pages;

public partial class DeviceChanges : ContentPage
{
	DataClass dt=new DataClass();
	List<List<string>> device_changes = new List<List<string>>();

	public DeviceChanges()
	{
		InitializeComponent();
		Task.Run(async() => 
		{
			dt.start_connection();
			device_changes=dt.get_employee_device_changes();
			dt.close_connection();

			List<string> header = new List<string> {"employee ID","Date","Current Device","Changed from Device","Date"};
		  string htmlstr =	dt.create_html_string(header,device_changes);

			MainThread.InvokeOnMainThreadAsync(() => 
			{
				vs.Add(new WebView { Source=new HtmlWebViewSource { Html=htmlstr } });

				Button xlbutton = new Button { Text = "Generate Excel",HorizontalOptions=LayoutOptions.Center };

				xlbutton.Clicked += (async(object sender,EventArgs e) => 
				{
					dt.Excel_Function(htmlstr, "report.html");

					

				});
				vs.Add(xlbutton);

				vs.Remove(actind);
			});

			
		});
	}

	private void Xlbutton_Clicked(object sender, EventArgs e)
	{
		throw new NotImplementedException();
	}
}