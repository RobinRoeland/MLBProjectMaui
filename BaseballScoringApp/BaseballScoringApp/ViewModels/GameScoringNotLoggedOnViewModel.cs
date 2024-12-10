using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseballScoringApp.ViewModels
{
    class GameScoringNotLoggedOnViewModel : INotifyPropertyChanged
    {
        public GameScoringNotLoggedOnViewModel()
        {
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
