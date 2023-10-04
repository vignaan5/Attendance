using Attendance.Data;

namespace Attendance.Pages;

public partial class ViewProductRankings : ContentPage
{
	List<string> states = new List<string>();
	List<string> xvalues = new List<string>();
	List<string> yvalues = new List<string>();
	List<List<string>> products_ranks = new List<List<string>>();
    DataClass dt = new DataClass();
	public ViewProductRankings()
	{
		InitializeComponent();

		Task.Run(async () =>
		{

			dt.start_connection();
			states = dt.get_all_states_from_employees();
			dt.close_connection();






			MainThread.InvokeOnMainThreadAsync(async () =>
			{
				states.Add("ALL");
				statepicker.ItemsSource = states;

				orderbypicker.ItemsSource = new List<string> { "Order by Qty", "Order by Sales" };
				
				;

				orderbypicker.SelectedIndexChanged += (object sender, EventArgs e) =>
				{

					string htmlstr = "";
					dt.start_connection();
					if (statepicker.SelectedIndex == statepicker.ItemsSource.Count - 1)
					{
						products_ranks = dt.get_product_rankings(null, ref htmlstr);
					}
					else
					{
						products_ranks = dt.get_product_rankings(statepicker.ItemsSource[statepicker.SelectedIndex].ToString().Trim(), ref htmlstr);
					}

					dt.close_connection();

					xvalues.Clear();
					yvalues.Clear();


					


					for (int i = 0; i < products_ranks.Count; i++)
					{

						for (int j = 0; j < products_ranks[i].Count; j++)
						{
							if (j == 0)
							{
								xvalues.Add(products_ranks[i][j]);
							}
							else if (j == 1 && orderbypicker.SelectedIndex==0)
							{
								if (products_ranks[i][j] != "")
									yvalues.Add(products_ranks[i][j]);
								else yvalues.Add("0");
							}
							else if(j==2 && orderbypicker.SelectedIndex==1) 
							{
								if (products_ranks[i][j] != "")
									yvalues.Add(products_ranks[i][j]);
								else yvalues.Add("0");
							}
						}


					}

					string graph_html = dt.create_bar_graph("ClayVeda Products Rankings", xvalues, yvalues);





					wview.Source = new HtmlWebViewSource { Html = htmlstr };
					gview.Source = new HtmlWebViewSource { Html = graph_html };
					vs.Clear();
					create_excel_button(htmlstr);
					create_graph_button(graph_html);



				};

				statepicker.SelectedIndex = statepicker.ItemsSource.Count - 1;
				orderbypicker.SelectedIndex = 0;


			});

		});

	}

	public void create_excel_button(string htmlstr)
	{
		Button xlbutton = new Button { Text = "Generate Excel", HorizontalOptions = LayoutOptions.Center };

		xlbutton.Clicked += (async (object sender, EventArgs e) => {

			string path = AppDomain.CurrentDomain.BaseDirectory;

			var destination = System.IO.Path.Combine(path, "report.html");

#if WINDOWS

			destination = @"C:\Users\Public\Documents\report.html";

#endif

			File.WriteAllText(destination, htmlstr);
#if WINDOWS
			try
			{
				System.Diagnostics.Process.Start(@"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe", destination);
			}
			catch (Exception ex)
			{
				DisplayAlert("Error", ex.Message.ToString(), "Ok");
			}
			return;

#endif
			await Launcher.Default.OpenAsync(new OpenFileRequest("Download Excel from Web Browser", new ReadOnlyFile(destination)));


		});


		vs.Add(xlbutton);
	}

	public void create_graph_button(string htmlstr)
	{
		Button xlbutton = new Button { Text = "Generate Graph", HorizontalOptions = LayoutOptions.Center };

		xlbutton.Clicked += (async (object sender, EventArgs e) => {

			string path = AppDomain.CurrentDomain.BaseDirectory;

			var destination = System.IO.Path.Combine(path, "reportg.html");

#if WINDOWS

			destination = @"C:\Users\Public\Documents\reportg.html";

#endif

			File.WriteAllText(destination, htmlstr);
#if WINDOWS
			try
			{
				System.Diagnostics.Process.Start(@"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe", destination);
			}
			catch (Exception ex)
			{
				DisplayAlert("Error", ex.Message.ToString(), "Ok");
			}
			return;

#endif
			await Launcher.Default.OpenAsync(new OpenFileRequest("Download Excel from Web Browser", new ReadOnlyFile(destination)));


		});


		vs.Add(xlbutton);
	}


}