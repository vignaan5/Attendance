using Attendance.Data;


namespace Attendance.Pages;

public partial class Attendance2 : ContentPage
{

public	DataClass dt = new DataClass();
public Dictionary<string, List<Dictionary<string, string>>> dates = new Dictionary<string, List<Dictionary<string, string>>>();	
 public List<string> all_employee_id=new List<string>();
public string state = String.Empty;

	public Attendance2()
	{
		InitializeComponent();
		actind.IsVisible = true;
		
			
			
				List<string> list = new List<string>();
				dt.start_connection();
				
				list= dt.get_all_states_from_employees();

				dt.close_connection();

				list.Add("All");

        
				statepicker.ItemsSource = list; 
				actind.IsVisible = false;

			
		

		
	}


	public  void get_attendance()
	{
		if(datepicker_start.Date>datepicker_end.Date)
		{
			return;
		}


		state = (string)statepicker.ItemsSource[statepicker.SelectedIndex];
		dt.start_connection();
		
		all_employee_id = dt.get_all_employee_id_in_a_specific_state(state);
		Dictionary<string, List<string>> approved_leaves = dt.get_all_employee_approved_leave_dates(state);

		while (datepicker_start.Date<=datepicker_end.Date)
		{
			string appdate = datepicker_start.Date.ToString("dd-MM-yyyy");
			string sqldate = datepicker_start.Date.ToString("yyyy-MM-dd");

			foreach (string empid in  all_employee_id)
			{
				

				List<string>present_emp_ids=dt.employee_ids_who_were_present_on_specific_day(sqldate);

				if(dates.ContainsKey(appdate))
				{
					Dictionary<string, string> emp = dt.get_employee_details_with_column_names(empid);
					emp["date"]=appdate;
					
					if(present_emp_ids.Contains(empid))
					 {
						emp["present"] = "yes";
					 }
					 else
					{
						emp["present"] = "no";
					}
					dates[appdate].Add(emp);
				}
				else
				{
					dates[appdate]=new List<Dictionary<string,string>>();
					Dictionary<string, string> emp = dt.get_employee_details_with_column_names(empid);
					emp["date"] = appdate;

					if (present_emp_ids.Contains(empid))
					{
						emp["present"] = "yes";
					}
					else
					{
						emp["present"] = "no";
					}
					dates[appdate].Add(emp);

				} 

			}

			  datepicker_start.Date=  datepicker_start.Date.AddDays(1);
		}
		

	}

	private void abtn_Clicked(object sender, EventArgs e)
	{
		get_attendance();
	}

	private void abtn_Clicked_1(object sender, EventArgs e)
	{

	}
}