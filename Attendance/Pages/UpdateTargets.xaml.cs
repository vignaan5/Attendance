using Attendance.Data;

namespace Attendance.Pages;

public partial class UpdateTargets : ContentPage
{
	public Dictionary<string, Dictionary<string, List<Dictionary<string, string>>>> state_dic = new Dictionary<string, Dictionary<string, List<Dictionary<string, string>>>>();
	DataClass dt = new DataClass();
	public string state = "";
	public string area = "";
	public UpdateTargets()
	{
		InitializeComponent();
		
			get_store_details_dic();
		
	}

	private void pick(object sender, EventArgs e)
	{

	}




	public async void get_store_details_dic()
	{
     
	  dt.start_connection();
	  state_dic= dt.get_all_store_details();
		dt.close_connection();
		List<string> temp = new List<string>();	

		foreach(var x in  state_dic) 
		{ 
		     temp.Add(x.Key);
		}

		Picker store_picker= new Picker { Title="Choose a State"};
		

		store_picker.ItemsSource= temp;

		vs.Clear();

	


		store_picker.SelectedIndexChanged += ((object sender, EventArgs e) => { 
			
			     if(store_picker.Title=="Choose a State")
			      {
				     store_picker.Title = "Choose an area";
				  Label temp2 = new Label { Text = String.Format("State :{0}", store_picker.ItemsSource[store_picker.SelectedIndex].ToString()) };
				  vs.Remove(store_picker);
				  vs.Add(temp2);

				  temp.Clear();
				string[] temp3 = temp2.Text.Split(':');
				  foreach(var x in state_dic[temp3[1]])
				  {
					temp.Add(x.Key);
				  }

				state = temp3[1];

			     Picker	store_picker2 = new Picker { Title = "Choose an area" };
				  store_picker2.ItemsSource=temp;
				store_picker2.SelectedIndexChanged += ((object sender, EventArgs e) => {

					if (store_picker2.Title == "Choose an area" && store_picker2.SelectedIndex >= 0)
					{
						Label temp2 = new Label { Text = String.Format("Area :{0}", store_picker2.ItemsSource[store_picker2.SelectedIndex].ToString()) };
						vs.Remove(store_picker2);
						vs.Add(temp2);
					  Picker	store_picker3 = new Picker { Title = "Choose a Store" };
						temp.Clear();
						string[] temp3 = temp2.Text.Split(':');
						temp.Clear();
						area = temp3[1];
						foreach(var x in state_dic[state][temp3[1]])
						{
							temp.Add(x["store_name"]);
						}
						store_picker3.ItemsSource = temp;

						store_picker3.SelectedIndexChanged += ((object sender, EventArgs e) =>
						{
							string str_name = store_picker3.ItemsSource[store_picker3.SelectedIndex].ToString();
							
							var z = state_dic[state][area][store_picker3.SelectedIndex];

							vs.Clear();
							foreach (var y in z)
							{
								Label l = new Label { Text = y.Key + " : " + y.Value, FontSize = 30 };
								vs.Add(l);
							}
						});
						vs.Add(store_picker3);

					}

				});
				  vs.Add(store_picker2);
				  
			
			     }
				 
			
			
			});

		vs.Add(store_picker);


	}


}