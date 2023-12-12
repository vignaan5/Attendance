using Attendance.Data;
using Attendance.Models;

namespace Attendance.Pages;

public partial class ChatPage : ContentPage
{

	

	public DataClass dt = new DataClass();

	public ChatPage()
	{
		InitializeComponent();
		chatpagevar.Title = "Notification Page";
		var list = new List<Models.CollectionItem>();
		
		


		Task.Run(async () => 
		{
			Thread.Sleep(1000);

			await dt.get_emp_id();
			dt.start_connection();

			list = dt.get_messages_from_DB();
			 if(list.Count==0)
			{
				list=dt.get_recent_messages_from_DB();
			}
			else
			{
#if ANDROID

             Location.LocationClass l1= new Location.LocationClass();
			 l1.send_notification_to_the_user(list.Count.ToString()+" New Messages","Please Check your messages");; 
#endif
			}


			dt.close_connection();

			MainThread.InvokeOnMainThreadAsync(async () => 
			{ 
				 
			      lview.ItemsSource = list;

				lview.ItemSelected += Lview_ItemSelected;
				
			});

		
		});


		Task.Run(async () =>
		{

			await dt.get_emp_id();
			while (true)
			{
				Thread.Sleep(900*1000);
				dt.start_connection();

				list = dt.get_messages_from_DB();
				if (list.Count == 0)
				{
					list = dt.get_recent_messages_from_DB();
				}
				else
				{
#if ANDROID

             Location.LocationClass l1= new Location.LocationClass();
			 l1.send_notification_to_the_user(list.Count.ToString()+" New Messages","Please Check your messages");; 
#endif
				}


				dt.close_connection();

				MainThread.InvokeOnMainThreadAsync(async () =>
				{

					lview.ItemsSource = list;

					

				});

			}


		});

	}

	private  void Lview_ItemSelected(object sender, SelectedItemChangedEventArgs e)
	{
		ListView obj =(ListView) sender;
		Models.CollectionItem template = (Models.CollectionItem)obj.SelectedItem;
		if(template.SenderID==dt.emp_id2)
		{

			return;
		}

		Navigation.PushAsync(new Chats(template));
		

	}

	private void sbar_TextChanged(object sender, TextChangedEventArgs e)
	{

		Navigation.PushAsync(new ViewEmployeeLocations(true));
	}

	private void sbar_Clicked(object sender, EventArgs e)
	{
		Navigation.PushAsync(new ViewEmployeeLocations(true));
	}
}