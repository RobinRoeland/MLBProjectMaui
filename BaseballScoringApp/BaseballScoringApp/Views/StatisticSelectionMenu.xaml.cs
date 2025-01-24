using BaseballModelsLib.Models;

namespace BaseballScoringApp.Views;

public partial class StatisticSelectionMenu : ContentPage
{
    private readonly Player _player;

    public StatisticSelectionMenu(Player player)
    {
        InitializeComponent();
        _player = player;

        var menuGroups = new Dictionary<string, Dictionary<string, Command>>();

        menuGroups.Add("Statistics", new Dictionary<string, Command> {
            { "Season 1", new Command(() => HandleSeason(1)) },
            { "Season 2", new Command(() => HandleSeason(2)) },
            { "Season 3", new Command(() => HandleSeason(3)) }
        });

        MenuItems.ItemsSource = menuGroups;
    }

    private void HandleSeason(int season)
    {
    }
}