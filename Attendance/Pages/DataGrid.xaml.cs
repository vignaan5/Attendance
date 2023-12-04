using Attendance.Data;

using System.Runtime.InteropServices;

namespace Attendance.Pages;

public partial class DataGrid : ContentPage
{
	List<List<string>> rows = new List<List<string>>();
	DataClass dt = new DataClass();

	public string state = String.Empty;
	public DataGrid()
	{
		InitializeComponent();
		

			dt.start_connection();
		  List<string> states=	  dt.get_all_states_from_employees();
			dt.close_connection();

		states.Add("All");

		statepicker.ItemsSource = states;

		statepicker.SelectedIndex = statepicker.ItemsSource.Count-1;

	}

	public DataGrid(string state)
	{
		InitializeComponent();
		this.state= state;
		statepicker.ItemsSource = new List<string> {state};
		statepicker.SelectedIndex = 0;
	}

	public DataGrid(List<string> states)
	{
		InitializeComponent();
		
		statepicker.ItemsSource = states;
		statepicker.SelectedIndex = 0;
	}






	public string get_date_from_picker()
	{
		string d = dpicker.Date.Date.ToString();
		
		string[] tempd = d.Split(' ');
		string[] days = tempd[0].Split("/");

		

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
		int cumilative_sales_from_first_day = 0;
		string htmlstring = "<html> <body> <table border='1' id='table'> <thead> <tr bgcolor=#D3D3D3> <th> Sno </th> <th> Particulars </th> <th>  HSN/SAC </th> <th> MRP </th> <th> PCS </th> <th> Amount </th> </tr> </thead> <tbody> ";
		string sql_str = get_date_from_picker();
		int sum = 0;
		DatePicker dtpicker2 = new DatePicker();
		
		string first_day = dtpicker2.Date.Year.ToString()+"-"+dtpicker2.Date.Month.ToString()+"-"+"1";

		dt.start_connection();
		if (state != String.Empty && state!="All")
		{
			rows = dt.get_rows(sql_str, ref sum,state);
		}
		else
		{
			rows = dt.get_rows(sql_str, ref sum);
		}


		if(state != String.Empty  && state!="All") { cumilative_sales_from_first_day = dt.get_cumiliatvie_sales(first_day, sql_str,state); }
		else
		{ cumilative_sales_from_first_day = dt.get_cumiliatvie_sales(first_day, sql_str); }
		
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

		string daily_sale_row= String.Format("<tr><td> </td> <td> </td> <td> </td> <td> </td> <td> Daily sale = </td> <td>{0} </td>", sum.ToString());

		string lst_row = String.Format("<tr><td> </td> <td> </td> <td> </td> <td> </td> <td> Cumilative sales = </td> <td>{0} </td>", cumilative_sales_from_first_day.ToString());
		htmlstring += daily_sale_row;
		htmlstring += lst_row;

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
#if WINDOWS
        
   destination= @"C:\Users\Public\Documents\report.html";
            
#endif

			File.WriteAllText(destination, create_html_string());
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

		Button button = new Button();

		button.Text = "Get Cumulative Sales";

		button.Clicked+=((object sender,EventArgs e) =>
		{
			int Csales = 0;
			dt.start_connection();
			if (state != String.Empty && (string)statepicker.SelectedItem!="All") 
			{
				Csales = dt.get_cumiliatvie_sales(get_date_from_picker(start_date), get_date_from_picker(to_date),state);

			}
			else
			{
				 Csales = dt.get_cumiliatvie_sales(get_date_from_picker(start_date), get_date_from_picker(to_date));
			}
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

	private void statepicker_SelectedIndexChanged(object sender, EventArgs e)
	{
		this.state = statepicker.SelectedItem as string;
		//if(!vs.Contains(genxl))
		//{

		//}

	
	}
}