using System;
using System.Collections.Generic;
using System.Text;

namespace TablePermutationsProj.Models
{
    class TableRow
    {
        public TableRow(Table parentTable, Team team, int wins, int draws, int losses, int goalsFor, int goalsAgainst)
        {
            ParentTable = parentTable;
            Team = team;
            Wins = wins;
            Draws = draws;
            Losses = losses;
            GoalsFor = goalsFor;
            GoalsAgainst = goalsAgainst;
        }

        public Table ParentTable { get; set; }
        public Team Team { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; } //losses not really needed, more just for display niceness
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int GoalDifference => GoalsFor - GoalsAgainst;
        public int Points => (Wins * 3) + Draws;
        public bool ManualSortCheckRequired { get; set; }
    }
}
