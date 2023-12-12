using Attendance.Data;


namespace Attendance.Pages;

public partial class UploadClayvedaNewVersion : ContentPage
{
	public DataClass dt = new DataClass();

	public UploadClayvedaNewVersion()
	{
		InitializeComponent();
		Task.Run(async () => 
		{
			Thread.Sleep(300);

			MainThread.InvokeOnMainThreadAsync(async () => 
			{
				string pass = await DisplayPromptAsync("Enter Passcode", "");
				if (pass == "729300")
				{
					exceute_process();
				}
				else
				{
					Navigation.PopAsync();
				}
			
			});
		   
		
		});
	}


	public async void exceute_process()
	{
		var result = await PickAndShow(PickOptions.Default);
		if (result != null)
		{
			if (result.FileName.EndsWith("apk", StringComparison.OrdinalIgnoreCase) )
			{

				using var stream = await result.OpenReadAsync();

				BinaryReader br = new BinaryReader(stream);
				byte[] apk = br.ReadBytes((int)stream.Length);

				string path = AppDomain.CurrentDomain.BaseDirectory;

				var destination = System.IO.Path.Combine(path, "clayveda.apk");

#if WINDOWS

				File.WriteAllBytes(@"C:\Users\Public\Downloads\clayveda.apk", apk);


#endif

#if ANDROID
       				File.WriteAllBytes(destination,apk);

					bool opened= await Launcher.Default.OpenAsync(new OpenFileRequest("Install APk", new ReadOnlyFile(destination)));

					if(opened)
					{

					}
                 
#endif

			}
			else
			{
				
			}
		}


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

}