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
		Navigation.PushAsync(new UpdateStock());
	}

    private void edit_stock_Clicked(object sender, EventArgs e)
    {
		 Navigation.PushAsync(new ViewRecentStocks());	
    }

    private void update_defect_stock_Clicked(object sender, EventArgs e)
    {
		Navigation.PushAsync(new UpdateStock("defect"));
    }

    private void edit_defect_stock_Clicked(object sender, EventArgs e)
    {
		Navigation.PushAsync(new ViewRecentStocks("defect"));
    }
}