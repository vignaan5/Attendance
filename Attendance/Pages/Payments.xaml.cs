using Attendance.Data;
using Microsoft.Maui.Controls.Internals;

namespace Attendance.Pages;

public partial class Payments : ContentPage
{
	public DataClass dt = new DataClass();
	public Payments()
	{
		InitializeComponent();
		get_paymets();
	}


	public async Task<FileResult> PickAndShow(PickOptions options)
	{
		try
		{
			var result = await FilePicker.Default.PickAsync(options);
			

			return result;
		}
		catch (Exception ex)
		{
			// The user canceled or something went wrong
		}

		return null;
	}




	public async void get_paymets()
	{
		List<Image> payment_screenshots = new List<Image>();
		List<List<string>> payment_details = new List<List<string>>();
		dt.start_connection();
		payment_screenshots = dt.get_blob_image(ref payment_details);
		dt.close_connection();

		

		for(int i=0;i<payment_details.Count;i++)
		{

			Label details = new Label { Text = "Bill Details", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };

			Label datel = new Label { Text = "Date :"+payment_details[i][0].Split(' ')[0],VerticalOptions=LayoutOptions.Center,HorizontalOptions=LayoutOptions.Center };
			Label amount = new Label { Text ="amount : "+ payment_details[i][1], VerticalOptions = LayoutOptions.Center,HorizontalOptions=LayoutOptions.Center };
			Label status = new Label { Text = payment_details[i][2], VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center,FontSize=20,FontAttributes=FontAttributes.Bold };

			VerticalStackLayout fvs = new VerticalStackLayout { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };

			fvs.Add(details);
			fvs.Add(datel);
			fvs.Add(amount);
			if (payment_screenshots[i] != null)
			{
				fvs.Add(payment_screenshots[i]);
			}
			else
			{
				Button btn = new Button { Text = "Upload Payment Details", HorizontalOptions = LayoutOptions.Center };
				btn.Clicked += (async(object sender,EventArgs e)=>
				{
					var result= await PickAndShow(PickOptions.Images);

					if (result != null)
					{
						if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
							result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase) || result.FileName.EndsWith("jpeg", StringComparison.OrdinalIgnoreCase))
						{

							using var stream = await result.OpenReadAsync();
							
							BinaryReader br = new BinaryReader(stream);
							byte[] img = br.ReadBytes((int)stream.Length);
							string date_sql = datel.Text;
							string[] date_sql_arr = date_sql.Split('/');						
							date_sql = date_sql_arr[2]+"-"+date_sql_arr[1]+"-"+date_sql_arr[0].Split(':')[1].Trim();

							dt.start_connection();
							bool res = dt.upload_payment(img, date_sql);
							dt.close_connection();

							Navigation.PopAsync();
						     if(res)
							{
								DisplayAlert("Uploaded image", "We will get back to you shortly", "ok");
							}
							 else
							{
								DisplayAlert("Uploading image Failed", "We will get back to you shortly", "ok");

							}


						}
					}



				});

				fvs.Add(btn);
			}
				fvs.Add(status);

			Frame f = new Frame { HorizontalOptions= LayoutOptions.Center,VerticalOptions=LayoutOptions.Center,Content=fvs,MaximumHeightRequest= 600,MaximumWidthRequest=500 };

			vs.Add(f);
		

		}


	}

}