using BaseballModelsLib.Models;
using Microsoft.Extensions.Logging;
using System.Text;
using Newtonsoft.Json;
using System.Text.Json;
using System.Net.Http;
using System.Net.Sockets;
using BaseballScoringApp.Services;
using System.Net.Http.Headers;

namespace BaseballScoringApp.Models
{
    public class BBDataRepository
    {
        public static BBDataRepository dataLayer = null;

        public List<BBTeam> mTeamsList; 
        public Boolean mLoggedIn;


        public BBGame? mCurrentGame;

        public BBDataRepository()
        {
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
        public async Task<BBGame> GetLastRunningGame()
        {
            List<BBGame> mGameList = null;
            BBGame lastgame = null;
            //initialises mTeamList from API call
            var url = Globals.serverURL + "/api/games/listgames";
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
                        mGameList = JsonConvert.DeserializeObject<List<BBGame>>(stringvalue);
                        if(mGameList.Count > 0)
                            lastgame = mGameList[0];
                    }
                }
                catch (HttpRequestException e)
                {
                    var str = e.Message;
                    Console.WriteLine($"HttpRequestException: {e.Message}");
                    if (e.InnerException != null)
                        Console.WriteLine($"Inner Exception: {e.InnerException.Message}");
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
                catch(HttpRequestException e)
                {
                    var str = e.Message;
                    Console.WriteLine($"HttpRequestException: {e.Message}");
                    if (e.InnerException != null)
                        Console.WriteLine($"Inner Exception: {e.InnerException.Message}");
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
                                }
                                else
                                {
                                    Console.WriteLine("Player team not found : " + pl.Id.ToString());
                                }
                            }                                
                        }

                    }
                }
                catch (HttpRequestException e)
                {
                    var str = e.Message;
                }
            }
        }
        public BBTeam getTeamByID(int id)
        {
            for(int i = 0; i < mTeamsList.Count; i++) { 
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
                        Console.WriteLine("$\"Game successfully sent to API.");
                        //DisplayAlert("MLBPlayers", $"{listPlayers.Count} Players of {team.Name} successfully sent to API.", "Ok");

                        //this initialises the new game and sets object instances
                        mCurrentGame.StartGame(mCurrentGame.TotalInnings);
                    }
                    else
                    {
                        Console.WriteLine($"Failed to send New Game request. Status Code: {responsepost.StatusCode}");
                        string responseContent = await responsepost.Content.ReadAsStringAsync();
                        Console.WriteLine($"Response Content: {responseContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        internal async Task<bool> sendScoreList(List<BBScoreStatistic> scoresList)
        {
            //this sends the list of scores to the api
            //if it worked, OK is returned
            bool returnvalue=false;
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
                        Console.WriteLine("$\"Game successfully sent to API.");
                        //DisplayAlert("MLBPlayers", $"{listPlayers.Count} Players of {team.Name} successfully sent to API.", "Ok");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to send New Game request. Status Code: {responsepost.StatusCode}");
                        string responseContent = await responsepost.Content.ReadAsStringAsync();
                        Console.WriteLine($"Response Content: {responseContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            return returnvalue;
        }
    }
}
