using BaseballModelsLib.Models;
using BaseballScoringApp.Models;
using System.Text.Json;
using MLBRestAPI;
using Microsoft.Extensions.Logging;
using System.Text;
using BaseballScoringApp.Services;
using System.Net.Http.Headers;

namespace BaseballScoringApp;

public partial class TestPage : ContentPage
{
    public TestPage()
    {
        InitializeComponent();
    }
    private async void ButtonImportTeams_Clicked(object sender, EventArgs e)
    {
        BBDataRepository repo = BBDataRepository.getInstance();
        await repo.importTeamsfromMLBAPI(this);            
    }
    private async void ButtonImportRoster_Clicked(object sender, EventArgs e)
    {
        BBDataRepository repo = BBDataRepository.getInstance();
        await repo.importPlayerRosterfromMLBAPI(this);
    }
    private async void ButtonGetPlayers_Clicked(object sender, EventArgs e)
    {
        var url2 = Globals.serverURL + "/api/players/listplayers";
        using var httpclient2 = new HttpClient();
        {
            var response = await httpclient2.GetAsync(url2);
            if (response.IsSuccessStatusCode)
            {
                var stringvalue = await response.Content.ReadAsStringAsync();
                List<Player> p = JsonSerializer.Deserialize<List<Player>>(stringvalue);
                foreach (Player pl in p)
                {
                    var s = pl.Name;
                }
            }
        }
    }
    private async void ButtondetailPlayer_Clicked(object sender, EventArgs e)
    {
        var url = Globals.serverURL + "/api/players/findplayer?id=1";
        using var httpclient = new HttpClient();
        {
            var jwtToken = await JwtService.GetTokenAsync();
            httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await httpclient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var stringvalue = await response.Content.ReadAsStringAsync();
                Player p = JsonSerializer.Deserialize<Player>(stringvalue);
                var s = p.Name;

                BBDataRepository rp = BBDataRepository.getInstance();

            }
        }
    }
    public async void ButtonImportTeamsCSV_Clicked(object sender, EventArgs e) // This method is triggered by the button click
    {
        BBDataRepository repo = BBDataRepository.getInstance();
        await repo.importTeamsFromCSV(this);
    }

    private async void ButtonImportRosterCSV_Clicked(object sender, EventArgs e)
    {
        BBDataRepository repo = BBDataRepository.getInstance();
        await repo.importPlayerRosterfromCSV(this);
    }
}