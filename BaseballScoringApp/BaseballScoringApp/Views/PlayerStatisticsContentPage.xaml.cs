using BaseballModelsLib.Models;
using BaseballScoringApp.Models;
using BaseballScoringApp.ViewModels;
using BaseballScoringApp.Views;
namespace BaseballScoringApp;

public partial class PlayerStatisticsContentPage : ContentPage
{

    private PlayerStatisticsContentPageViewModel mViewModel;

    public PlayerStatisticsContentPage(Player player)
    {
        InitializeComponent();
        AccessViewModel();
        mViewModel.Player = player;
        
    }

    private void AccessViewModel()
    {
        mViewModel = null;
        // Cast the BindingContext to the ViewModel type
        if (BindingContext is PlayerStatisticsContentPageViewModel viewModel)
        {
            mViewModel = (PlayerStatisticsContentPageViewModel)BindingContext;
            // Access properties or call methods on the ViewModel
        }
    }

    private void OnPageAppearing(object sender, EventArgs e)
    {
        // Access your ViewModel through BindingContext
        if (BindingContext is PlayerStatisticsContentPageViewModel viewModel)
        {
            viewModel.GetStatistics();
        }
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private async void Layout_Clicked(object sender, EventArgs e)
    {
        if (sender is StackLayout tappedLayout )
        {
            BBStatisticsKPI statistics = (BBStatisticsKPI)tappedLayout.BindingContext;
            var statDetailList = tappedLayout.FindByName<CollectionView>("StatDetailList");
            
            if (statistics != null && statistics.detailScores != null && statistics.detailScores.Any(score => score.StatisticsName != statistics.StatisticsName) && statDetailList != null) 
            {
                statDetailList.ItemsSource = statistics.detailScores;
                statDetailList.IsVisible = !statDetailList.IsVisible;
                mViewModel.Statistics = mViewModel.Statistics;
            }
        }
    }

}