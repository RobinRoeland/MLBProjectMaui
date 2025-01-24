using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseballModelsLib;
using BaseballModelsLib.Models;

namespace BaseballScoringApp.Models
{
    public class BBTeam : Team
    {
        public Color mTeamColor;
        public List<BBPlayer> mRosterList;
        //sublist of players
        public List<BBPlayer> mPitchers;

        //during Game:
        public List<BBPlayer> mLineUpList;

        public Dictionary<string, BBPlayer> mLineUpByPositionDict;

        public BBTeam()
        {
            mRosterList = new List<BBPlayer>();
            mPitchers = new List<BBPlayer>();
            mLineUpList = new List<BBPlayer>();
            mLineUpByPositionDict = new Dictionary<string, BBPlayer>();
            mTeamColor = Colors.AliceBlue;
        }
        public void addPlayer(BBPlayer bballplayer)
        {
            if (!mRosterList.Contains(bballplayer))
            {
                mRosterList.Add(bballplayer);
                if (bballplayer.isPitcher())
                    mPitchers.Add(bballplayer);
            }


        }

        public BBPlayer getPlayer(int MLBPlayerId)
        {
            foreach(BBPlayer bbplayer in mRosterList)
            {
                if (bbplayer.MLBPersonId == MLBPlayerId)
                {
                    return bbplayer;
                }
            }
            return null;
        }
        public BBPlayer getRandomPlayerPlayingPosition(string position)
        {
            List<BBPlayer> subListPlayersAtPosition= mRosterList
                .Where(player => player.Position == position)
                .ToList();

            if (subListPlayersAtPosition.Count == 0)
                return null;

            //pick a random player playing at this position, a team can have eg multiple 1B players
            var random = new Random();
            return subListPlayersAtPosition[random.Next(subListPlayersAtPosition.Count)];
        }
        public BBPlayer getRandomPlayerPlayingGivenPosition(string position)
        {
            // used to return a random player that is not a pitcher and is not  yet in the lineup
            var filteredRemainingPlayers = mRosterList
                .Where(player => player.Position == position)
                .ToList();

            if (filteredRemainingPlayers.Any())
            {
                var random = new Random();
                return filteredRemainingPlayers[random.Next(filteredRemainingPlayers.Count)];
            }
            return null;
        }
        public BBPlayer getRandomPlayerPlayingPosition(List<BBPlayer> exclusionList, string excludingposition)
        {
            // used to return a random player that is not a pitcher and is not  yet in the lineup
            var filteredRemainingPlayers = mRosterList
                .Where(player => !exclusionList.Contains(player) && player.Position!= excludingposition)
                .ToList();

            if (filteredRemainingPlayers.Any())
            {
                var random = new Random();
                return filteredRemainingPlayers[random.Next(filteredRemainingPlayers.Count)];
            }
            return null;
        }
        public void addPlayerToLineupAtPosition(string fieldposition)
        {
            //not to be used for pitchers, only for field players
            BBPlayer pl = getRandomPlayerPlayingPosition(fieldposition);
            if (pl== null)
            {
                //the team has no player with given positon, 
                //get another player playing any other position to play this position which is not yet in lineup and not a pitcher
                pl = getRandomPlayerPlayingPosition(mLineUpList, "P"); //not already in lineup and not a pitcher
            }
            if (pl != null)
            {
                mLineUpList.Add(pl); // defines the batting order
                mLineUpByPositionDict[fieldposition] = pl; //allows accessing a position even if player default pos is another position
            }
                
        }
        public bool GenerateLineUp_ForGame()
        {
            addPlayerToLineupAtPosition("C");
            addPlayerToLineupAtPosition("1B");
            addPlayerToLineupAtPosition("2B");
            addPlayerToLineupAtPosition("3B");
            addPlayerToLineupAtPosition("SS");
            addPlayerToLineupAtPosition("LF");
            addPlayerToLineupAtPosition("CF");
            addPlayerToLineupAtPosition("RF");
            // player 9 is the DH or another not pitcher random player
            addPlayerToLineupAtPosition("DH");

            if (mLineUpList.Count == 9)
                return true;
            return false;
        }
        public BBPlayer getPlayerTypeFromLineUp(String position)
        {
            if (mLineUpByPositionDict.Keys.Contains(position)) { 
                 return mLineUpByPositionDict[position];
            }
            return null;
        }
        public void debugDumpLineUp()
        {
            int i = 0;
            Debug.WriteLine("Line Up : " + Name);

            foreach (BBPlayer pl in mLineUpList)
            {
                i++;
                Debug.WriteLine($"\t{i} : {pl.GetDebugDisplayString()}");
            }


        }
        public BBPlayer getRandomPitcher()
        {
            if (mPitchers.Count == 0)
                return null;

            //pick a random player playing at this position, a team can have eg multiple 1B players
            var random = new Random();
            return mPitchers[random.Next(mPitchers.Count)];
        }
    }
}
