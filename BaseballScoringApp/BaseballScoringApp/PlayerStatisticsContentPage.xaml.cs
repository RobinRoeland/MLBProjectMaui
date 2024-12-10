namespace BaseballScoringApp;

public partial class PlayerStatisticsContentPage : ContentPage
{
	public PlayerStatisticsContentPage()
	{
		InitializeComponent();
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
		Navigation.PopAsync();
    }
}