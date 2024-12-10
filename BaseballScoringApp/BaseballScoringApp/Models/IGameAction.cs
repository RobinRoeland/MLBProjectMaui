using System;
using System.Collections.Generic;

namespace BaseballScoringApp.Models
{
    public interface IGameAction
    {
        public string ActionName { get; set; }
        public string ActionDisplayName { get; set; }
        public BBPlayer playerInvolved { get; set; }

        public void DoAction(BBGame forGame);        
    }
}
