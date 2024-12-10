using BaseballScoringApp.ViewModels;

namespace BaseballScoringApp;

public partial class PlayerSelectionContentPage : ContentPage
{
	public PlayerSelectionContentPage()
	{
		InitializeComponent();
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
        Navigation.PushAsync(new PlayerStatisticsContentPage());
    }

    public void OnCheckBoxChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox checkBox &&
                checkBox.BindingContext is KeyValuePair<string, bool> filter)
        {
            // Access your ViewModel through BindingContext
            if (BindingContext is PlayerSelectionContentPageViewModel viewModel)
            {
                viewModel.OnFilterChanged(filter.Key, e.Value);
            }
        }
    }
}