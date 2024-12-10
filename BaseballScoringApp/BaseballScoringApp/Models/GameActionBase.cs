using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseballScoringApp.Models
{
    public class GameActionBase : IGameAction
    {
        public string ActionName { get; set; }
        public string ActionDisplayName { get; set; }

        public BBPlayer? playerInvolved { get; set; }

        public GameActionBase()
        {
            ActionName = "Undefined";
            ActionDisplayName = "Undefined";
            playerInvolved = null;
        }

        virtual public void DoAction(BBGame forGame)
        {
            throw new NotImplementedException();
        }
    }
}
