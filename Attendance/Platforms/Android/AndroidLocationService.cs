using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Gestures;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using AndroidX;
using AndroidX.Core;
using AndroidX.Core.App;
using Attendance.Location;
using static Android.Icu.Text.CaseMap;

namespace The_Attendance.Platforms
{
	[Service]
	public class AndroidLocationService : Service,Interfaces.IAndroid
	{

		public static bool is_foreground_service_running=false ;
		public override IBinder OnBind(Intent intent)
		{
			throw new NotImplementedException();
		}


		[return: GeneratedEnum]
		public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
		{
			string title = "Attendance Service";

			const string channel_id = "default";

			const string channel_name = "Default";

			



			Task.Run(() => 
			{   	
				LocationClass locationClass = new LocationClass();	

			    while (is_foreground_service_running) 
				{
					locationClass.Execute_process();

					Thread.Sleep(4500000);
				}
			});



			NotificationManager manager = (NotificationManager)Android.App.Application.Context.GetSystemService(NotificationService);

			var channelNameJava = new Java.Lang.String(channel_name);

			if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
			{
				var channel = new NotificationChannel(channel_id, channelNameJava, NotificationImportance.High)
				{
					Description = "Channel Description"
				};
				manager.CreateNotificationChannel(channel);
			}
			var builder = new NotificationCompat.Builder(Android.App.Application.Context, channel_id).SetContentTitle(title)
				.SetContentText("Clocked In !")
					.SetLargeIcon(BitmapFactory.DecodeResource(Android.App.Application.Context.Resources, Android.Resource.Drawable.SymDefAppIcon))
					.SetSmallIcon(Android.Resource.Drawable.SymDefAppIcon)
					.SetPriority((int)NotificationPriority.High)
					.SetVisibility((int)NotificationVisibility.Public)
					.SetDefaults((int)NotificationDefaults.Sound | (int)NotificationDefaults.Vibrate);

			Notification notification = builder.Build();

			StartForeground(1001, notification);





			return base.OnStartCommand(intent, flags, startId);
		}

		public override void OnCreate()
		{
			is_foreground_service_running = true;
			base.OnCreate();
			
		}

		public override void OnDestroy()
		{
			is_foreground_service_running = false;
			base.OnDestroy();
			
		}

		public void StartMyService()
		{

			var intent = new Intent(Android.App.Application.Context, typeof(AndroidLocationService));
			 
			Android.App.Application.Context.StartForegroundService(intent);
		}

		public void StopMyService()
		{
			var intent = new Intent(Android.App.Application.Context,typeof(AndroidLocationService));
			LocationClass templ = new LocationClass();
			templ.send_notification_to_the_user("Clocked Out", "You have successfully Clocked out");
			Android.App.Application.Context.StopService(intent);


		}

		public bool IsForeGroundServiceRunning()
		{
			return is_foreground_service_running;

		}

		public void update_duration(int seconds)
		{
			throw new NotImplementedException();
		}
	}
}
