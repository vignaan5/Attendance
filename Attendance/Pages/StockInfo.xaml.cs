using Attendance.Data;

namespace Attendance.Pages;

public partial class StockInfo : ContentPage
{
	public DataClass dt = new DataClass();
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

    private async void edit_stock_Clicked(object sender, EventArgs e)
    {
		await dt.get_emp_id();
		 Navigation.PushAsync(new ViewRecentStocks(dt.emp_id2,true));	
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