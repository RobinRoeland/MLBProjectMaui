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
        var url2 = "https://lookup-service-prod.mlb.com/json/named.team_all_season.bam?sport_code='mlb'&all_star_sw='N'&sort_order=name_asc&season='2024'";
        using var httpclient2 = new HttpClient();
        {
            BBDataRepository repo = BBDataRepository.getInstance();
            List<BBTeam> listTeams = new List<BBTeam>();
            var response = await httpclient2.GetAsync(url2);
            if (response.IsSuccessStatusCode)
            {
                var stringvalue = await response.Content.ReadAsStringAsync();
                Team_AllSeason teamallseason = JsonSerializer.Deserialize<Team_AllSeason>(stringvalue);
                if (teamallseason != null)
                {
                    repo.mTeamsList.Clear();
                    foreach (Row r in teamallseason.team_all_season.queryResults.row)
                    {
                        Globals.logger.LogInformation(r.name_display_full);
                        BBTeam nt = new BBTeam();
                        nt.Id = Convert.ToInt32(r.mlb_org_id);
                        nt.Name = r.name_display_full;
                        nt.VenueName = r.venue_name;
                        nt.NameDisplayBrief = r.name_display_brief;
                        nt.LeagueName = r.league;
                        nt.FranchiseCode = r.franchise_code;
                        nt.City = r.city;
                        nt.MLB_Org_ID = r.mlb_org_id;

                        nt.Deleted = false;
                        nt.TotalPlayers = 0;
                        listTeams.Add(nt);
                    }

                }
            }
            else
                return;

            //step 2 call api to add teams in server
            try
            {
                string jsonPayload = JsonSerializer.Serialize(listTeams, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                // Prepare the content with JSON payload
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var urlpost = Globals.serverURL + "/api/teams/addteams";
                using var httpclient = new HttpClient();
                {
                    var jwtToken = await JwtService.GetTokenAsync();
                    httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                    var responsepost = await httpclient.PostAsync(urlpost, content);
                    if (responsepost.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Teams successfully sent to API.");
                        repo.mTeamsList.AddRange(listTeams);
                        DisplayAlert("MLBTeams", $"{listTeams.Count} Teams successfully sent to API.", "Ok");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to send teams. Status Code: {response.StatusCode}");
                        string responseContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Response Content: {responseContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
    private async void ButtonImportRoster_Clicked(object sender, EventArgs e)
    {
        BBDataRepository repo = BBDataRepository.getInstance();

        int countadded = 0;
        foreach (BBTeam team in repo.mTeamsList)
        {
            //for each team , fetch roster

            string url2 = $"https://statsapi.mlb.com/api/v1/teams/{team.MLB_Org_ID}/roster";
            using var httpclient2 = new HttpClient();
            {
                List<BBPlayer> listPlayers= new List<BBPlayer>();
                var response = await httpclient2.GetAsync(url2);
                if (response.IsSuccessStatusCode)
                {
                    var stringvalue = await response.Content.ReadAsStringAsync();
                    RosterResponse rosterresponse = JsonSerializer.Deserialize<RosterResponse>(stringvalue);
                    if (rosterresponse  != null)
                    {
                        foreach (Roster r in rosterresponse.roster)
                        {
                            Globals.logger.LogInformation(r.person.fullName);
                            BBPlayer nt = new BBPlayer();
                            nt.Id = Convert.ToInt32(r.person.id);
                            nt.Name = r.person.fullName;
                            nt.Position = r.position.abbreviation;
                            if (r.jerseyNumber == "")
                                r.jerseyNumber = "99";
                            nt.Rugnummer= Convert.ToInt16(r.jerseyNumber);
                            nt.APILink = r.person.link;
                            nt.MLBPersonId = r.person.id;
                            nt.TeamId = r.parentTeamId;
                            nt.Team = repo.getTeamByID(r.parentTeamId);
                            nt.Deleted = false;
                            // team id moet nog gezet worden na id voor team ok
                            listPlayers.Add(nt);
                        }
                    }
                }
                else
                    return;

                //step 2 call api to add teams in server
                try
                {
                    string jsonPayload = JsonSerializer.Serialize(listPlayers, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });
                    // Prepare the content with JSON payload
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    var urlpost = Globals.serverURL + "/api/players/addplayers";
                    using var httpclient = new HttpClient();
                    {
                        var jwtToken = await JwtService.GetTokenAsync();
                        httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                        var responsepost = await httpclient.PostAsync(urlpost, content);
                        if (responsepost.IsSuccessStatusCode)
                        {
                            countadded += listPlayers.Count;
                            Console.WriteLine("$\"{listPlayers.Count} Players successfully sent to API.");
                            //DisplayAlert("MLBPlayers", $"{listPlayers.Count} Players of {team.Name} successfully sent to API.", "Ok");
                        }
                        else
                        {
                            Console.WriteLine($"Failed to send players. Status Code: {response.StatusCode}");
                            string responseContent = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"Response Content: {responseContent}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }
        DisplayAlert("MLBPlayers", $"Adding player roster completed, tot added {countadded}.", "Ok");
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
}