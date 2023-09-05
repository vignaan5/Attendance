
using Attendance.Data;


namespace Attendance.Pages;

public partial class YourSales : ContentPage
{
	public int total_sale = 0;
	DataClass dt = new DataClass();
	public YourSales()
	{
		InitializeComponent();
	}


	public string create_html_string(List<List<string>> rows)
	{
		string htmlstring = "<html> <body> <table border='1' id='table'><thead> <tr bgcolor=#D3D3D3> <th> ProductName </th> <th> MRP </th> <th>  Qty Sold </th> <th> Amount </th> <th> Date </th>  </tr> </thead> <tbody>";
		
		int sum = 0;
		
		 for(int i=0;i< rows.Count;i++) 
		{
			htmlstring += "<tr>";

			string[] datet = rows[i][5].Split(' ');

			htmlstring += String.Format("<td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td>", rows[i][8], rows[i][10], rows[i][2], rows[i][3] ,datet[0]);

			htmlstring += "</tr>";
		  
		}

		htmlstring +=String.Format("</tbody></table>{0}<h1>All Sales = {1} </h1></body></html>",dt.get_js2excel_script(),total_sale);

		return htmlstring;

        
		
		}

		private async void GetYourSalesClicked(object sender, EventArgs e)
	{
		actind.IsVisible = true;

		Task.Run(async() => 
		{
			string str_date = dt.date_picker_to_sql_date(dtstart);
			string end_date = dt.date_picker_to_sql_date(dtend);
			await dt.get_emp_id();
			dt.start_connection();
			List<List<string>> strings = dt.get_employee_sales_between_two_dates(str_date, end_date);
			dt.close_connection();

			

			for(int i=0;i<strings.Count;i++)
			{
				int to_int = 0;
				try
				{
					 to_int = Convert.ToInt32(strings[i][3]);
				}
				catch (Exception ex) 
				{
					to_int = 0;
				}
				 
				 total_sale+=to_int;
			
			}

			string html_str = create_html_string(strings);

			MainThread.InvokeOnMainThreadAsync(() => {

				vs.Clear();

				VerticalStackLayout inner_vs = new VerticalStackLayout {  };

				Button xlbutton = new Button { Text = "Generate Excel" };
				xlbutton.Clicked += (async (object sender,EventArgs e) =>
				{
					string path = AppDomain.CurrentDomain.BaseDirectory;

					var destination = System.IO.Path.Combine(path, "report.html");


#if WINDOWS
        
   destination= @"C:\Users\Public\Documents\report.html";
            
#endif


					File.WriteAllText(destination, html_str);


#if WINDOWS
              try
			  {
             System.Diagnostics.Process.Start(@"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",destination);
                 }
				 catch(Exception ex)
				 {
				 DisplayAlert("Error", ex.Message.ToString(),"Ok");
				 }
			 return;

#endif



					await Launcher.Default.OpenAsync(new OpenFileRequest("Download Excel from Web Browser", new ReadOnlyFile(destination)));

				});
				xlbutton.HorizontalOptions = LayoutOptions.Center;
				inner_vs.Add(xlbutton);

				inner_vs.Add(new WebView { Source = new HtmlWebViewSource { Html = html_str } });


				vs.Add(new ScrollView {  Content = inner_vs });
			  
				
			});

		});
		




	}
}