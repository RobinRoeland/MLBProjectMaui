using BaseballModelsLib.Models;
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
        if (sender is StackLayout stackLayout &&
        stackLayout.BindingContext is Player player)
        {
            Navigation.PushAsync(new PlayerStatisticsContentPage(player));
        }
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

    private async void OnImageLoaded(object sender, EventArgs e)
    {
        if (sender is Image imageView &&
            imageView.BindingContext is Player player &&
            BindingContext is PlayerSelectionContentPageViewModel viewModel)
        {
            await viewModel.SetValidatedImageUrl($"https://img.mlbstatic.com/mlb-photos/image/upload/d_people:generic:headshot:silo:current.png/r_max/w_180,q_auto:best/v1/people/{player.MLBPersonId}/headshot/silo/current", imageView);
        }
    }

}