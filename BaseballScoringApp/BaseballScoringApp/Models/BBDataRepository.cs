using BaseballModelsLib.Models;
using Microsoft.Extensions.Logging;
using System.Text;
using Newtonsoft.Json;
using System.Text.Json;
using System.Net.Http;
using System.Net.Sockets;
using BaseballScoringApp.Services;
using System.Net.Http.Headers;
using System.Diagnostics;
using Microsoft.Maui.Controls.Internals;
using MLBRestAPI;

namespace BaseballScoringApp.Models
{
    public class BBDataRepository
    {
        public static BBDataRepository dataLayer = null;

        public List<BBTeam> mTeamsList;
        public Boolean mLoggedIn;


        public BBGame? mCurrentGame;

        public bool mPlayersLoadingFinished;

        public BBDataRepository()
        {
            mPlayersLoadingFinished = false;
            mTeamsList = new List<BBTeam>();
            mLoggedIn = false;
            mCurrentGame = null;
        }
        //singleton instande datarepository
        public static BBDataRepository getInstance()
        {
            if (dataLayer == null)
            {
                dataLayer = new BBDataRepository();
            }
            return dataLayer;
        }
        public async Task InitialiseOnLogin()
        {
            await FetchAllTeams();
            await FetchAllPlayers();
        }
        public bool isInitialised()
        {
            return mTeamsList.Count > 0 && mPlayersLoadingFinished;
        }
        public async Task<BBGame> GetLastRunningGame(string forUser)
        {
            List<BBGame> mGameList = null;
            BBGame lastgame = null;
            //initialises mTeamList from API call
            var url = Globals.serverURL + "/api/games/listgamesforuser";
            using var httpclient = new HttpClient();
            {
                try
                {
                    //send false as param
                    string jsonPayload = System.Text.Json.JsonSerializer.Serialize(forUser, new System.Text.Json.JsonSerializerOptions
                    {
                        WriteIndented = true
                    });
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    var jwtToken = await JwtService.GetTokenAsync();
                    httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                    var response = await httpclient.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        String stringvalue = await response.Content.ReadAsStringAsync();
                        mGameList = JsonConvert.DeserializeObject<List<BBGame>>(stringvalue);
                        if (mGameList.Count > 0)
                            lastgame = mGameList[0];
                    }
                }
                catch (HttpRequestException e)
                {
                    var str = e.Message;
                    Debug.WriteLine($"HttpRequestException: {e.Message}");
                    if (e.InnerException != null)
                        Debug.WriteLine($"Inner Exception: {e.InnerException.Message}");
                }
            }
            return lastgame;
        }
        public async Task FetchAllTeams()
        {
            mTeamsList.Clear();
            //initialises mTeamList from API call
            var url = Globals.serverURL + "/api/teams/listteams";
            using var httpclient = new HttpClient();
            {
                //httpclient.BaseAddress = new Uri("http://10.0.2.2:5204/");
                try
                {
                    var jwtToken = await JwtService.GetTokenAsync();
                    httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                    var response = await httpclient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        String stringvalue = await response.Content.ReadAsStringAsync();
                        mTeamsList = JsonConvert.DeserializeObject<List<BBTeam>>(stringvalue);
                    }
                }
                catch (HttpRequestException e)
                {
                    var str = e.Message;
                    Debug.WriteLine($"HttpRequestException: {e.Message}");
                    if (e.InnerException != null)
                        Debug.WriteLine($"Inner Exception: {e.InnerException.Message}");
                }
            }
        }
        public async Task FetchAllPlayers()
        {
            //initialises mTeamList from API call
            var url = Globals.serverURL + "/api/players/listplayers";
            using var httpclient = new HttpClient();
            {
                try
                {
                    var jwtToken = await JwtService.GetTokenAsync();
                    httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                    var response = await httpclient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        String stringvalue = await response.Content.ReadAsStringAsync();
                        List<BBPlayer> playerslist = JsonConvert.DeserializeObject<List<BBPlayer>>(stringvalue);
                        // link players to teams
                        foreach (BBPlayer pl in playerslist)
                        {
                            if (pl.TeamId.HasValue)
                            {
                                BBTeam team = getTeamByID(pl.TeamId.Value);
                                if (team != null)
                                {
                                    team.addPlayer(pl);
                                    pl.Team = team;
                                }
                                else
                                {
                                    Debug.WriteLine("Player team not found : " + pl.Id.ToString());
                                }
                            }
                        }
                    }
                }
                catch (HttpRequestException e)
                {
                    var str = e.Message;
                }
                mPlayersLoadingFinished = true;
            }
        }
        public BBTeam getTeamByID(int id)
        {
            for (int i = 0; i < mTeamsList.Count; i++)
            {
                BBTeam team = mTeamsList[i];
                if (team.Id == id)
                    return team;
            }
            return null;
        }
        public async Task StartANewGame(BBGame newGame)
        {
            //this sends the new game object to the api,
            //if it worked, the new id of the game is returned and added and the currentgame object is set in repo content
            try
            {
                string jsonPayload = System.Text.Json.JsonSerializer.Serialize(newGame, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });
                // Prepare the content with JSON payload
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var urlpost = Globals.serverURL + "/api/games/addgame";
                using var httpclient = new HttpClient();
                {
                    var jwtToken = await JwtService.GetTokenAsync();
                    httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                    var responsepost = await httpclient.PostAsync(urlpost, content);
                    if (responsepost.IsSuccessStatusCode)
                    {
                        //response is gameid
                        mCurrentGame = newGame;
                        string responseJSONContent = await responsepost.Content.ReadAsStringAsync();

                        Game gameReturnedContainingId = JsonConvert.DeserializeObject<Game>(responseJSONContent);
                        mCurrentGame.Id = gameReturnedContainingId.Id;
                        Debug.WriteLine("$\"Game successfully sent to API.");
                        //DisplayAlert("MLBPlayers", $"{listPlayers.Count} Players of {team.Name} successfully sent to API.", "Ok");

                        //this initialises the new game and sets object instances
                        mCurrentGame.StartGame(mCurrentGame.TotalInnings);
                    }
                    else
                    {
                        Debug.WriteLine($"Failed to send New Game request. Status Code: {responsepost.StatusCode}");
                        string responseContent = await responsepost.Content.ReadAsStringAsync();
                        Debug.WriteLine($"Response Content: {responseContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        public async Task<bool> sendGameUpdate()
        {
            //this updates the current game content to the api,
            //if it worked, the true is returned 
            bool returnvalue = true;
            if (mCurrentGame == null)
                return false;
            try
            {
                string jsonPayload = System.Text.Json.JsonSerializer.Serialize(mCurrentGame, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });
                // Prepare the content with JSON payload
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var urlpost = Globals.serverURL + "/api/games/updategame";
                using var httpclient = new HttpClient();
                {
                    var jwtToken = await JwtService.GetTokenAsync();
                    httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                    var responsepost = await httpclient.PostAsync(urlpost, content);
                    if (responsepost.IsSuccessStatusCode)
                    {
                        //response is game containing new gameid if game didn't exist yet
                        string responseJSONContent = await responsepost.Content.ReadAsStringAsync();
                        Game gameReturnedContainingId = JsonConvert.DeserializeObject<Game>(responseJSONContent);
                        mCurrentGame.Id = gameReturnedContainingId.Id;
                        Debug.WriteLine("$\"Game successfully sent to API.");
                    }
                    else
                    {
                        Debug.WriteLine($"Failed to send New Game request. Status Code: {responsepost.StatusCode}");
                        string responseContent = await responsepost.Content.ReadAsStringAsync();
                        Debug.WriteLine($"Response Content: {responseContent}");
                        returnvalue = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred: {ex.Message}");
                returnvalue = false;
            }
            return returnvalue;
        }
        public async Task<bool> removeGameFromRepo(Game aGame)
        {
            //this deletes a given game content to the api, it deletes all scores of the game and the game from the table
            //if it worked, the true is returned 
            bool returnvalue = true;
            if (aGame == null)
                return false;
            try
            {
                string jsonPayload = System.Text.Json.JsonSerializer.Serialize(aGame, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });
                // Prepare the content with JSON payload
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var urlpost = Globals.serverURL + "/api/games/deletegame";
                using var httpclient = new HttpClient();
                {
                    var jwtToken = await JwtService.GetTokenAsync();
                    httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                    var responsepost = await httpclient.PostAsync(urlpost, content);
                    if (responsepost.IsSuccessStatusCode)
                    {
                        //response is game containing new gameid if game didn't exist yet
                        string responseJSONContent = await responsepost.Content.ReadAsStringAsync();
                        Game gameReturnedContainingId = JsonConvert.DeserializeObject<Game>(responseJSONContent);
                        Debug.WriteLine("$\"Game succesfully deleted at API.");
                    }
                    else
                    {
                        Debug.WriteLine($"Failed to execute delete Game request. Status Code: {responsepost.StatusCode}");
                        string responseContent = await responsepost.Content.ReadAsStringAsync();
                        Debug.WriteLine($"Response Content: {responseContent}");
                        returnvalue = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred: {ex.Message}");
                returnvalue = false;
            }
            return returnvalue;
        }
        public async Task<bool> removeAllStatisticsForGameFromRepo(Game aGame)
        {
            //this deletes a given game content to the api, it deletes all scores of the game and the game from the table
            //if it worked, the true is returned 
            bool returnvalue = true;
            if (aGame == null)
                return false;
            try
            {
                string jsonPayload = System.Text.Json.JsonSerializer.Serialize(aGame, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });
                // Prepare the content with JSON payload
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var urlpost = Globals.serverURL + "/api/statistics/deletescoresforgame";
                using var httpclient = new HttpClient();
                {
                    var jwtToken = await JwtService.GetTokenAsync();
                    httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                    var responsepost = await httpclient.PostAsync(urlpost, content);
                    if (responsepost.IsSuccessStatusCode)
                    {
                        //no returned value
                        Debug.WriteLine("$\"Game succesfully deleted at API.");
                    }
                    else
                    {
                        Debug.WriteLine($"Failed to execute delete Game request. Status Code: {responsepost.StatusCode}");
                        string responseContent = await responsepost.Content.ReadAsStringAsync();
                        Debug.WriteLine($"Response Content: {responseContent}");
                        returnvalue = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred: {ex.Message}");
                returnvalue = false;
            }
            return returnvalue;
        }
        internal async Task<bool> sendScoreList(List<BBScoreStatistic> scoresList)
        {
            //this sends the list of scores to the api
            //if it worked, OK is returned
            bool returnvalue = false;
            try
            {
                string jsonPayload = System.Text.Json.JsonSerializer.Serialize(scoresList);
                // Prepare the content with JSON payload
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var urlpost = Globals.serverURL + "/api/statistics/bookStatistics";
                using var httpclient = new HttpClient();
                {
                    var jwtToken = await JwtService.GetTokenAsync();
                    httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                    var responsepost = await httpclient.PostAsync(urlpost, content);
                    if (responsepost.IsSuccessStatusCode)
                    {
                        returnvalue = true;
                        string responseJSONContent = await responsepost.Content.ReadAsStringAsync();
                        Debug.WriteLine("$\"Game successfully sent to API.");
                        //DisplayAlert("MLBPlayers", $"{listPlayers.Count} Players of {team.Name} successfully sent to API.", "Ok");
                    }
                    else
                    {
                        Debug.WriteLine($"Failed to send New Game request. Status Code: {responsepost.StatusCode}");
                        string responseContent = await responsepost.Content.ReadAsStringAsync();
                        Debug.WriteLine($"Response Content: {responseContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred: {ex.Message}");
            }
            return returnvalue;
        }
        internal async Task<int> countNumTeamsOnServer()
        {
            //this requests the DB via the api for the amount of teams in the table
            int returnvalue = 0;
            try
            {
                var urlget = Globals.serverURL + "/api/teams/getteamcount";
                using var httpclient = new HttpClient();
                {
                    var jwtToken = await JwtService.GetTokenAsync();
                    httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                    var responseget = await httpclient.GetAsync(urlget);
                    if (responseget.IsSuccessStatusCode)
                    {
                        string responsenum = await responseget.Content.ReadAsStringAsync();
                        returnvalue = Convert.ToInt32(responsenum);
                    }
                    else
                    {
                        returnvalue = 0;
                        Debug.WriteLine($"Failed to send request for numteams. Status Code: {responseget.StatusCode}");
                        string responseContent = await responseget.Content.ReadAsStringAsync();
                        Debug.WriteLine($"Response Content: {responseContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred: {ex.Message}");
            }
            return returnvalue;
        }
        public BBTeam getRandomTeam()
        {
            if (mTeamsList.Count == 0)
                return null;

            //pick a random player playing at this position, a team can have eg multiple 1B players
            var random = new Random();
            return mTeamsList[random.Next(mTeamsList.Count)];
        }
        public async Task verifyInitialisationNeededWithTeamData(ContentPage pagecalling)
        {
            int numteamsinserver = await countNumTeamsOnServer();
            if (numteamsinserver < 5)
            {
                //5 is the treshold created by seeding initially but should be removed and replaced
                bool ret = await pagecalling.DisplayAlert("Attention", "You are about to reinitialise your database (loosing all existing games and statistics).", "Ok", "Cancel");
                if (ret == true)
                {
                    await importTeamsFromCSV();
                    //await importPlayerRosterfromCSV();
                    await importPlayerRosterfromMLBAPI();
                    mTeamsList.Clear();
                }
            }
        }
        public async Task importTeamsFromCSV(ContentPage pageCalling = null) // This method is triggered by the button click
        {
            List<BBTeam> listTeams = new List<BBTeam>();
            var stream = await FileSystem.OpenAppPackageFileAsync("teams.csv");

            var reader = new StreamReader(stream);

            // : Read CSV manually 
            bool skipfirstline = true;
            bool firsttime = true;
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (skipfirstline == true && firsttime == true)
                {
                    firsttime = false;
                    continue;
                }

                var values = line.Split(';');
                if (values.Length >= 9) // Ensure there are enough values
                {
                    BBTeam team = new BBTeam
                    {
                        Id = int.Parse(values[0]), // Assuming Id is the first column
                        Name = values[1], // Name is the second column
                        TotalPlayers = int.Parse(values[2]), // TotalPlayers is the third column
                        MLB_Org_ID = values[3], // MLB_Org_ID is the fourth column
                        VenueName = values[4], // VenueName is the fifth column
                        LeagueName = values[5], // LeagueName is the sixth column
                        NameDisplayBrief = values[6], // NameDisplayBrief is the seventh column
                        City = values[7], // City is the eighth column
                        FranchiseCode = values[8], // FranchiseCode is the ninth column
                        Deleted = values[9] == "1" // Deleted is the tenth column, convert to bool
                    };
                    listTeams.Add(team);
                }
            }
            // Now you can use the games list as needed
            //step 2 call api to add teams in server
            try
            {
                string jsonPayload = System.Text.Json.JsonSerializer.Serialize(listTeams, new JsonSerializerOptions
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
                        Debug.WriteLine("Teams successfully sent to API.");
                        mTeamsList.AddRange(listTeams);
                        if (pageCalling != null)
                            pageCalling.DisplayAlert("MLBTeams", $"{listTeams.Count} Teams successfully sent to API.", "Ok");
                    }
                    else
                    {
                        Debug.WriteLine($"Failed to send teams. Status Code: {responsepost.StatusCode}");
                        string responseContent = await responsepost.Content.ReadAsStringAsync();
                        Debug.WriteLine($"Response Content: {responseContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        public async Task importTeamsfromMLBAPI(ContentPage pageCalling = null)
        {
            // get the teams from an online call to the depricated MLB API
            var url2 = "https://lookup-service-prod.mlb.com/json/named.team_all_season.bam?sport_code='mlb'&all_star_sw='N'&sort_order=name_asc&season='2024'";
            using var httpclient2 = new HttpClient();
            {
                BBDataRepository repo = BBDataRepository.getInstance();
                List<BBTeam> listTeams = new List<BBTeam>();
                var response = await httpclient2.GetAsync(url2);
                if (response.IsSuccessStatusCode)
                {
                    var stringvalue = await response.Content.ReadAsStringAsync();
                    Team_AllSeason teamallseason = System.Text.Json.JsonSerializer.Deserialize<Team_AllSeason>(stringvalue);
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
                    string jsonPayload = System.Text.Json.JsonSerializer.Serialize(listTeams, new JsonSerializerOptions
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
                            Debug.WriteLine("Teams successfully retrieved from API.");
                            repo.mTeamsList.AddRange(listTeams);
                            if(pageCalling != null)
                                pageCalling.DisplayAlert("MLBTeams", $"{listTeams.Count} Teams successfully sent to API.", "Ok");
                        }
                        else
                        {
                            Debug.WriteLine($"Failed to send teams. Status Code: {response.StatusCode}");
                            string responseContent = await response.Content.ReadAsStringAsync();
                            Debug.WriteLine($"Response Content: {responseContent}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }
        public async Task importPlayerRosterfromCSV(ContentPage pageCalling = null)
        {
            int countadded = 0;
            List<Player> listPlayers = new List<Player>();
            var stream = await FileSystem.OpenAppPackageFileAsync("players.csv");

            var reader = new StreamReader(stream, Encoding.UTF8);

            // : Read CSV manually 
            bool skipfirstline = true;
            bool firsttime = true;
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (skipfirstline == true && firsttime == true)
                {
                    firsttime = false;
                    continue;
                }
                var values = line.Split(';');
                if (values.Length >= 8) // Ensure there are enough values
                {
                    Player player = new Player
                    {
                        Id = int.Parse(values[0]), // Assuming Id is the first column
                        Name = values[1], // Name is the second column
                        Rugnummer = int.TryParse(values[2], out int rugnummer) ? rugnummer : (int?)null, // Rugnummer is the third column
                        Position = values[3], // Position is the fourth column
                        DOB = DateTime.TryParse(values[4], out DateTime dob) ? dob : DateTime.MinValue, // DOB is the fifth column
                        APILink = values[5], // APILink is the sixth column
                        MLBPersonId = int.TryParse(values[6], out int mlbPersonId) ? mlbPersonId : (int?)null, // MLBPersonId is the seventh column
                        TeamId = int.TryParse(values[7], out int teamId) ? teamId : (int?)null, // TeamId is the eighth column
                        Deleted = values.Length > 8 && values[8] == "1" // Deleted is the ninth column, convert to bool
                    };
                    listPlayers.Add(player);
                }

            }
            //step 2 call api to add teams in server
            try
            {
                string jsonPayload = System.Text.Json.JsonSerializer.Serialize(listPlayers, new JsonSerializerOptions
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
                        Debug.WriteLine($"{listPlayers.Count} Players successfully sent to API.");
                        //DisplayAlert("MLBPlayers", $"{listPlayers.Count} Players of {team.Name} successfully sent to API.", "Ok");
                    }
                    else
                    {
                        Debug.WriteLine($"Failed to send players. Status Code: {responsepost.StatusCode}");
                        string responseContent = await responsepost.Content.ReadAsStringAsync();
                        Debug.WriteLine($"Response Content: {responseContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred: {ex.Message}");
            }
            if (pageCalling != null)
                pageCalling.DisplayAlert("MLBPlayers", $"Adding player roster completed, tot added {countadded}.", "Ok");
        }
        public async Task importPlayerRosterfromMLBAPI(ContentPage pageCalling = null)
        {
            int countadded = 0;
            foreach (BBTeam team in mTeamsList)
            {
             	try
                {
	                //for each team , fetch roster
	                string url2 = $"https://statsapi.mlb.com/api/v1/teams/{team.MLB_Org_ID}/roster";
	                using var httpclient2 = new HttpClient();
	                {
	                    List<BBPlayer> listPlayers = new List<BBPlayer>();
	                    var response = await httpclient2.GetAsync(url2);
	                    if (response.IsSuccessStatusCode)
	                    {
	                        var stringvalue = await response.Content.ReadAsStringAsync();
	                        RosterResponse rosterresponse = System.Text.Json.JsonSerializer.Deserialize<RosterResponse>(stringvalue);
	                        if (rosterresponse != null)
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
	                                nt.Rugnummer = Convert.ToInt16(r.jerseyNumber);
	                                nt.APILink = r.person.link;
	                                nt.MLBPersonId = r.person.id;
	                                nt.TeamId = r.parentTeamId;
	                                nt.Team = getTeamByID(r.parentTeamId);
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
	                        string jsonPayload = System.Text.Json.JsonSerializer.Serialize(listPlayers, new JsonSerializerOptions
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
	                                Debug.WriteLine($"{listPlayers.Count} Players successfully sent to API.");
	                                //DisplayAlert("MLBPlayers", $"{listPlayers.Count} Players of {team.Name} successfully sent to API.", "Ok");
	                            }
	                            else
	                            {
	                                Debug.WriteLine($"Failed to send players. Status Code: {response.StatusCode}");
	                                string responseContent = await response.Content.ReadAsStringAsync();
	                                Debug.WriteLine($"Response Content: {responseContent}");
	                            }
	                        }
	                    }
	                    catch (Exception ex)
	                    {
	                        Debug.WriteLine($"An error occurred: {ex.Message}");
	                    }
	                }
	            }
                catch (Exception ex)
                {
                    Debug.WriteLine($"An error occurred: {ex.Message}");
                }
            }
            if (pageCalling != null)
                pageCalling.DisplayAlert("MLBPlayers", $"Adding player roster completed, tot added {countadded}.", "Ok");
        }
    }
}