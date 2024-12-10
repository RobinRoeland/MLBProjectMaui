using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using BaseballModelsLib.Models;
using BaseballScoringApp.Models;

namespace BaseballScoringApp.ViewModels
{

    public partial class GameStartContentPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private BBDataRepository mRepo;

        private List<BBTeam> _teams;
        private BBTeam _selectedHomeTeam;
        private BBTeam _selectedAwayTeam;

        private List<BBPlayer> _PitchersForSelectedHomeTeam;
        private List<BBPlayer> _PitchersForSelectedAwayTeam;
        private Player _SelectedPitcherHomeTeam;
        private Player _SelectedPitcherAwayTeam;

        private int _numInningsInGame;
        private DateTime _gameTime;
        private DateTime _gameDate;

        public List<BBTeam> TeamList
        {
            get => _teams;
            set
            {
                _teams = value;
                OnPropertyChanged();
            }
        }
        public int NumInningsInGame
        {
            get => _numInningsInGame;
            set
            {
                if (_numInningsInGame != value)
                {
                    _numInningsInGame = value;
                    OnPropertyChanged();

                }
            }
        }
        public DateTime GameDate
        {
            get => _gameDate;
            set
            {
                if (_gameDate != value)
                {
                    _gameDate = value;
                    OnPropertyChanged();
                }
            }
        }
        public DateTime GameTime
        {
            get => _gameTime;
            set
            {
                if (_gameTime != value)
                {
                    _gameTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public BBTeam SelectedHomeTeam
        {
            get => _selectedHomeTeam;
            set
            {
                _selectedHomeTeam = value;
                OnPropertyChanged();
            }
        }
        public BBTeam SelectedAwayTeam
        {
            get => _selectedAwayTeam;
            set
            {
                _selectedAwayTeam = value;
                OnPropertyChanged();
            }
        }
        public Player SelectedPitcherHomeTeam
        {
            get => _SelectedPitcherHomeTeam;
            set
            {
                _SelectedPitcherHomeTeam = value;
                OnPropertyChanged();
            }
        }
        public Player SelectedPitcherAwayTeam
        {
            get => _SelectedPitcherAwayTeam;
            set
            {
                _SelectedPitcherAwayTeam = value;
                OnPropertyChanged();
            }
        }
        public List<BBPlayer> PitchersForSelectedHomeTeam
        {
            get => _PitchersForSelectedHomeTeam;
            set
            {
                _PitchersForSelectedHomeTeam = value;
                OnPropertyChanged();
            }
        }
        public List<BBPlayer> PitchersForSelectedAwayTeam
        {
            get => _PitchersForSelectedAwayTeam;
            set
            {
                _PitchersForSelectedAwayTeam= value;
                OnPropertyChanged();
            }
        }
        public Command UpdateTextCommand { get; }

        public GameStartContentPageViewModel()
        {
            mRepo = BBDataRepository.getInstance();

            TeamList = mRepo.mTeamsList;
            NumInningsInGame = 9;
            GameDate = DateTime.Today;
            GameTime = DateTime.Now;
        }
        
        public void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
