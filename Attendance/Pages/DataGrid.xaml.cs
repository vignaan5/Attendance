using Attendance.Data;

using System.Runtime.InteropServices;

namespace Attendance.Pages;

public partial class DataGrid : ContentPage
{
	List<List<string>> rows = new List<List<string>>();
	DataClass dt = new DataClass();
	public DataGrid()
	{
		InitializeComponent();
	}





	public string get_date_from_picker()
	{
		string d = dpicker.Date.Date.ToString();
		
		string[] tempd = d.Split(' ');
		string[] days = tempd[0].Split("/");

		string year = days[2];
		string month = days[1];
		string day = days[0];

		string sql_string_date = "" + dpicker.Date.Year.ToString() + "-" + dpicker.Date.Month.ToString() + "-" + dpicker.Date.Day.ToString();
		cpage.Title= sql_string_date+"'s Daily Report";
		return sql_string_date;
	}


	public string get_date_from_picker(DatePicker temp_date_picker)
	{
	

		string sql_string_date = "" + temp_date_picker.Date.Year.ToString() + "-" + temp_date_picker.Date.Month.ToString() + "-" + temp_date_picker.Date.Day.ToString()	;
		cpage.Title = sql_string_date + "'s Daily Report";
		return sql_string_date;
	}

	public string create_html_string()
	{
		string htmlstring = "<html> <body> <table border='1' id='table'> <thead> <tr bgcolor=#D3D3D3> <th> Sno </th> <th> Particulars </th> <th>  HSN/SAC </th> <th> MRP </th> <th> PCS </th> <th> Amount </th> </tr> </thead> <tbody> ";
		string sql_str = get_date_from_picker();
		int sum = 0;
		dt.start_connection();
	    rows=dt.get_rows(sql_str, ref sum);
		dt.close_connection();

		for(int i=0;i<rows.Count;i++)
		{
			htmlstring += "<tr>";
			for(int j = 0; j < rows[i].Count;j++)
			{
				htmlstring += "<td>" + rows[i][j] + "</td>";


			}

			htmlstring += "</tr>";
		}

	   

		htmlstring += String.Format("</tbody></table> {0}  <h1> Daily Sale : {1} </h1> </body> </html>",dt.get_js2excel_script(), sum);
		
		
		return htmlstring;
	}


	private void get_report(object sender,EventArgs e)
	{
	
		
		vs.Clear();
		
		vs.Add(new WebView { Source = new HtmlWebViewSource { Html = create_html_string() } }) ; 
	    DatePicker start_date = new DatePicker();
		
		start_date.Date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
		DatePicker to_date = new DatePicker();
		Label label = new Label();
		label.Text = "Choose Start date and To date for Cumulative Sales";
		vs.Add(label);
		
		HorizontalStackLayout hs_temp = new HorizontalStackLayout();
		hs_temp.Add(new Label { Text = "From" });
		hs_temp.Add(start_date);
		hs_temp.Add(new Label { Text="To"});
		hs_temp.Add(to_date);

		vs.Add(hs_temp);

		Button xlbutton = new Button { Text="Generate Excel",HorizontalOptions=LayoutOptions.Center };

		xlbutton.Clicked += (async (object sender, EventArgs e) => {

			string path = AppDomain.CurrentDomain.BaseDirectory;

			var destination = System.IO.Path.Combine(path, "report.html");

			File.WriteAllText(destination, create_html_string());

			await Launcher.Default.OpenAsync(new OpenFileRequest("Download Excel from Web Browser", new ReadOnlyFile(destination)));


		});

		Button button = new Button();

		button.Text = "Get Cumulative Sales";

		button.Clicked+=((object sender,EventArgs e) =>
		{
			dt.start_connection();
		 int Csales =	dt.get_cumiliatvie_sales(get_date_from_picker(start_date),get_date_from_picker(to_date));
			dt.close_connection() ;

			vs.Remove(button);
			vs.Remove(hs_temp);
			vs.Remove(label);

			Label CsLable = new Label { Text = " Cumulative Sales : " + Csales.ToString(), FontSize = 30 };

			vs.Add(CsLable);
		
		
		});

	
		vs.Add(button);
		vs.Add(xlbutton);

	}
}