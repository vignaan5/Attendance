using Attendance.Data;
using System.Drawing;

namespace Attendance.Pages;

public partial class Chats : ContentPage
{
	public Models.CollectionItem info { get; set; }
	public DataClass dt = new DataClass();
	public Chats()
	{
		InitializeComponent();
	}

	public Chats(Models.CollectionItem details)
	{

		InitializeComponent();
		info=details;
		chatspagevar.Title = details.SenderID;

		var list = new List<Models.CollectionItem>();
		
					dt.start_connection();
				list =dt.get_msg_between_two_people(details.SenderID, details.ReceiverID);
		              dt.update_seen_msg(details.SenderID,details.ReceiverID);
				      dt.close_connection();
			
		foreach (var item in list) 
		{
		
			Editor e = new Editor { IsReadOnly=true };

			e.Text = item.SenderID + "\n\n"+item.Message;

			if(item.SenderID!=details.SenderID)
			{
				e.BackgroundColor = Microsoft.Maui.Graphics.Color.Parse("LightGreen");
				e.HorizontalOptions = LayoutOptions.End;
			}
			else
			{
				e.BackgroundColor = Microsoft.Maui.Graphics.Color.Parse("LightGray");

				e.HorizontalOptions=LayoutOptions.Start;
			}

			vs.Add(e);
		
		}

		Task.Run(async () => 
		{
			while (true)
			{
				Thread.Sleep(60 * 1000);
				dt.start_connection();
				list = dt.get_msg_between_two_people(details.SenderID, details.ReceiverID);
				dt.update_seen_msg(details.SenderID, details.ReceiverID);
				dt.close_connection();

				MainThread.InvokeOnMainThreadAsync(async () => 
				{
					vs.Clear();

					foreach (var item in list)
					{

						Editor e = new Editor { IsReadOnly = true };

						e.Text = item.SenderID + "\n\n" + item.Message;

						if (item.SenderID != details.SenderID)
						{
							e.BackgroundColor = Microsoft.Maui.Graphics.Color.Parse("LightGreen");
							e.HorizontalOptions = LayoutOptions.End;
						}
						else
						{
							e.BackgroundColor = Microsoft.Maui.Graphics.Color.Parse("LightGray");

							e.HorizontalOptions = LayoutOptions.Start;
						}

						vs.Add(e);

					}

				});

			}

		});
		
	}

	private void Submit_Clicked(object sender, EventArgs e)
	{
		if(entertxt.Text==null || entertxt.Text.Trim()=="" || entertxt.Text.Trim().Length>148)
		{
			return;
		}

		dt.start_connection();
		dt.update_new_msg_to_db(info.SenderID, info.ReceiverID, entertxt.Text.Trim());
		dt.close_connection();

		Editor temp = new Editor { HorizontalOptions=LayoutOptions.End,BackgroundColor= Microsoft.Maui.Graphics.Color.Parse("LightGreen"),Text=entertxt.Text,IsReadOnly=true };

		vs.Add(temp);

		entertxt.Text = "";



	}
}