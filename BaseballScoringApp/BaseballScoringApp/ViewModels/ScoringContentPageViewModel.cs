using BaseballScoringApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BaseballScoringApp.ViewModels
{
    class ScoringContentPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public BBDataRepository mRepo;

        private BBGame? _game;

        private string _imageBatterUrl;
        private string _imagePitcherUrl;


        public ScoringContentPageViewModel()
        {
            mRepo = BBDataRepository.getInstance();
        }

        public void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public BBGame? Game
        {
            get => mRepo.mCurrentGame;
        }
        

    }
}
