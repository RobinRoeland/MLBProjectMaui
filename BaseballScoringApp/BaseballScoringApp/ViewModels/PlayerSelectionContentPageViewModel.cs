using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BaseballModelsLib.Models;
using BaseballScoringApp.Models;
using Microsoft.Extensions.Logging;
using MLBRestAPI;

namespace BaseballScoringApp.ViewModels
{
    public partial class PlayerSelectionContentPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private BBDataRepository mRepo;

        private Dictionary<string, bool> _filters;
        public Dictionary<string, bool> Filters
        {
            get => _filters;
            set
            {
                _filters = value;
                OnPropertyChanged();
                UpdateFilteredPlayers();
            }
        }

        private List<BBTeam> _teams;
        private BBTeam _selectedTeam;

        private List<BBPlayer> _players;

        public List<BBTeam> TeamList
        {
            get => _teams;
            set
            {
                _teams = value;
                OnPropertyChanged();
            }
        }

        public BBTeam SelectedTeam
        {
            get => _selectedTeam;
            set
            {
                if (_selectedTeam != value)
                {
                    _selectedTeam = value;
                    OnPropertyChanged();
                    UpdateFilters();
                    UpdateFilteredPlayers(); // Update players when the team changes
                }
            }
        }

        public List<BBPlayer> PlayerList
        {
            get => _players;
            set
            {
                _players = value;
                OnPropertyChanged();
            }
        }

        public PlayerSelectionContentPageViewModel()
        {
            mRepo = BBDataRepository.getInstance();
            TeamList = mRepo.mTeamsList;
            SelectedTeam = mRepo.mTeamsList.First();
            PlayerList = new List<BBPlayer>(); // Initialize the filtered players list
            UpdateFilters();
        }

        private void UpdateFilters()
        {
            if (SelectedTeam != null && SelectedTeam.mRosterList != null)
            {
                var PositionList = SelectedTeam.mRosterList
                    .Select(p => p.Position!)
                    .Where(p => !string.IsNullOrEmpty(p))
                    .Distinct()
                    .ToList();

                Filters = PositionList.ToDictionary(
                    position => position,
                    value => false
                );
            }
        }

        private void UpdateFilteredPlayers()
        {
            if (SelectedTeam != null)
            {
                PlayerList = mRepo.mTeamsList.Where(team => team.Id == SelectedTeam.Id).First().mRosterList;
                
                // Apply position filters
                PlayerList = PlayerList.Where(player =>
                    !Filters.Any(filter => filter.Value) || // If no filters are active, show all players
                    (Filters.ContainsKey(player.Position!) && Filters[player.Position!])
                ).ToList();

                // Apply search filter
                PlayerList = PlayerList.Where(player =>
                    string.IsNullOrEmpty(searchText) ||
                    player.Name.StartsWith(searchText)
                ).ToList();
            }
            else
            {
                PlayerList = new List<BBPlayer>();
            }
        }

        private string searchText = string.Empty;
        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                OnPropertyChanged();
                UpdateFilteredPlayers();
            }
        }

        private bool _isFilterPopupVisible;
        public bool IsFilterPopupVisible 
        {
            get => _isFilterPopupVisible;

            set
            {
                _isFilterPopupVisible = !_isFilterPopupVisible;
                OnPropertyChanged();
            }
        }

        public Command ToggleFilterPopupCommand => new Command(() =>
        {
            IsFilterPopupVisible = !IsFilterPopupVisible;
        });

        public void OnFilterChanged(string filterName, bool isChecked)
        {
            Filters[filterName] = isChecked;
            Filters = Filters;
        }

        public void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
