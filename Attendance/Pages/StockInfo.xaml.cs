namespace Attendance.Pages;

public partial class StockInfo : ContentPage
{
	public StockInfo()
	{
		InitializeComponent();
	}

	private  void view_store_stock_Clicked(object sender, EventArgs e)
	{
		
		 Navigation.PushAsync(new ViewStoreStock());
	}

	private void update_store_stock_Clicked(object sender, EventArgs e)
	{

	}
}