using System;
using System.Collections.Generic;
using System.Text;

namespace TablePermutationsProj.Models
{
    class Result
    {
        public Result(int homeScore, int awayScore)
        {
            HomeScore = homeScore;
            AwayScore = awayScore;
        }

        public Result CloneResult()
        {
            return new Result(HomeScore, AwayScore);
        }

        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
    }
}
