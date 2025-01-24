using BaseballModelsLib.Models;
using BaseballScoringApp.Models;
using BaseballScoringApp.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BaseballScoringApp.ViewModels
{
    class PlayerStatisticsContentPageViewModel : INotifyPropertyChanged
    {
        private List<BBStatisticsKPI> _statistics;
        public List<BBStatisticsKPI> Statistics
        {
            get => _statistics;
            set
            {
                _statistics = value;
                OnPropertyChanged();
            }
        }

        private Player _player;
        public Player Player
        {
            get => _player;
            set
            {
                _player = value;
                OnPropertyChanged();
            }
        }

        public PlayerStatisticsContentPageViewModel()
        {
        }

        public async void GetStatistics()
        {
            List<StatisticsKPI> statisticList = null;
         
            //initialises mTeamList from API call
            var url = Globals.serverURL + "/api/statistics/ScoreNames";
            using var httpclient = new HttpClient();
            {
                try
                {
                    //send false as param
                    string jsonPayload = System.Text.Json.JsonSerializer.Serialize(_player, new System.Text.Json.JsonSerializerOptions
                    {
                        WriteIndented = true
                    });
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    var jwtToken = await JwtService.GetTokenAsync();
                    httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                    var response = await httpclient.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        string stringvalue = await response.Content.ReadAsStringAsync();
                        Statistics = JsonConvert.DeserializeObject<List<BBStatisticsKPI>>(stringvalue);

                        foreach (BBStatisticsKPI item in Statistics)
                        {
                            item.detailScores = await getDetailStatisticForKPI(item);
                        }
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

        public async Task<List<BBStatisticsKPI>> getDetailStatisticForKPI(BBStatisticsKPI statistic)
        {
            List<BBStatisticsKPI> statisticList = null;

            //initialises mTeamList from API call
            var url = Globals.serverURL + "/api/statistics/ScoreNamesForKPI";
            using var httpclient = new HttpClient();
            {
                try
                {
                    //send false as param
                    string jsonPayload = System.Text.Json.JsonSerializer.Serialize(statistic, new System.Text.Json.JsonSerializerOptions
                    {
                        WriteIndented = true
                    });
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    var jwtToken = await JwtService.GetTokenAsync();
                    httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                    var response = await httpclient.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        string stringvalue = await response.Content.ReadAsStringAsync();
                        statisticList = JsonConvert.DeserializeObject<List<BBStatisticsKPI>>(stringvalue);
                    }
                }
                catch (HttpRequestException e)
                {
                    var str = e.Message;
                    Debug.WriteLine($"HttpRequestException: {e.Message}");
                    if (e.InnerException != null)
                        Debug.WriteLine($"Inner Exception: {e.InnerException.Message}");
                }

                return statisticList;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
