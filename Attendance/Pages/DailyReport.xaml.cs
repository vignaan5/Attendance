using Attendance.Data;
using MySqlConnector;

namespace Attendance.Pages;

public partial class DailyReport : ContentPage
{
	public List<HorizontalStackLayout> scrollViews = new List<HorizontalStackLayout>();
	public DailyReport()
	{
		InitializeComponent();
	}

	public void create_ui()
	{
		foreach (var view in scrollViews) 
		{ 
		  vs.Add(view);
		}

	}


	private async void get_report(string sql_date)
	{
	  DataClass dt = new DataClass();
		dt.start_connection();

		string sql_cmd = " select * from products2 left join employee_sales2   on products2.Sno = employee_sales2.sno where The_date = '"+sql_date+"' or The_date is null; ";

		MySqlCommand mySql = new MySqlCommand(sql_cmd, dt.connection);

		mySql.ExecuteNonQuery();

		MySqlDataReader reader = mySql.ExecuteReader();

		int sum = 0;

		while (!reader.IsClosed && reader.Read()) 
		
		{

			HorizontalStackLayout temphs = new HorizontalStackLayout();

			Label sno= new Label();
			sno.Text ="S No : "+ reader[0].ToString();

			Label particulars = new Label();
			particulars.Text = "Particulars : "+ reader[1].ToString();

			Label Hsn = new Label();
			Hsn.Text = reader[2].ToString();

			Label mrp = new Label();
			mrp.Text ="M.R.P : " + reader[3].ToString();

			Label pcs = new Label();
			pcs.Text ="PCS : "+ reader[6].ToString();

			Label amt = new Label();
			amt.Text = "Amount : "+ reader[7].ToString();

			if(amt.Text.Length > 0) 
			{
				string[] temp2 = amt.Text.Split(':');
				temp2[1]=temp2[1].Trim();
				if (temp2[1].Length>0)
				sum += Convert.ToInt32(temp2[1]);
			}

			Label Emp = new Label();
			Emp.Text ="Sold my Emp Id : "+  reader[4].ToString();

			temphs.Spacing = 20;
			temphs.Add(sno);
			temphs.Add(particulars);
			temphs.Add(Hsn);
			temphs.Add(mrp);
			temphs.Add(pcs);
			temphs.Add(amt);
			temphs.Add(Emp);

		
			scrollViews.Add(temphs);
		
		}

		ScrollView tsc2 = new ScrollView();
		Label amount = new Label();
		amount.Text = "Total Amount ="+ sum.ToString();
	   HorizontalStackLayout ths = new HorizontalStackLayout();
		ths.Add(amount);
		scrollViews.Add(ths);

		dt.close_connection();

	}





	private async void Button_Clicked(object sender, EventArgs e)
	{
		actind.IsVisible = true;
		 string d = dpicker.Date.Date.ToString();

		string[] tempd = d.Split(' ');
		string[] days = tempd[0].Split("/");

		string year = days[2];
		string day = days[1];
		string month = days[0];

		string sql_string_date = ""+year+"-"+month+"-"+day;

	
			get_report(sql_string_date);
		
		
		

		create_ui();
		actind.IsVisible = false;

	}
}