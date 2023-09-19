using Attendance.Pages;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attendance
{
	public partial class AppShell2:Shell
	{
		public string user_name { get; set; }
		public bool is_admin { get; set; }
		public AppShell2(string UserName,bool is_admin)
		{
			this.is_admin= is_admin;
			this.user_name = UserName;
			Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
			 
			  if(is_admin )
			{
				Items.Add(new FlyoutItem
				{
					Title = "Home",
				
					Icon = new FontImageSource
					{
						FontFamily = "fasolid900",

					},
					Items =
			{
				new Tab{
					Title = "Home",
					Items = {
						new ShellContent
						{
							Title = UserName,
							Route = nameof(MainPage),
							ContentTemplate = new DataTemplate(typeof(MainPage))
						},

						new ShellContent
						{
							Title = "AdminPage",
							Route = nameof(Pages.AdminPage),
							ContentTemplate = new DataTemplate(typeof(AdminPage))
						},
						new ShellContent
						{
							Title = "AccountSettings",
							Route = nameof(Pages.AccountSettings),
							ContentTemplate = new DataTemplate(typeof(AccountSettings))
						},
						new ShellContent
						{
							Title = "Logout",
							Route = nameof(Pages.LogoutPage),
							ContentTemplate = new DataTemplate(typeof(LogoutPage))
						},
					}
				}
	
			}



				});
			}
			  else
			{
				Items.Add(new FlyoutItem
				{
					Title = "Home",
					
					Icon = new FontImageSource
					{
						FontFamily = "fasolid900",

					},
					Items =
			{
				new Tab{
					Title = "Home",
					Items = {
						new ShellContent
						{
							Title = UserName,
							Route = nameof(MainPage),
							ContentTemplate = new DataTemplate(typeof(MainPage))
						},
							new ShellContent
						{
							Title = "AccountSettings",
							Route = nameof(Pages.AccountSettings),
							ContentTemplate = new DataTemplate(typeof(AccountSettings))
						},

						new ShellContent
						{
							Title = "Logout",
							Route = nameof(Pages.LogoutPage),
							ContentTemplate = new DataTemplate(typeof(LogoutPage))
						},
					}
				}
			}



				});
			}

			
		}
	}
}
