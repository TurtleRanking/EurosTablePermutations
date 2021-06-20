using System;
using System.Collections.Generic;
using System.Text;

namespace TablePermutationsProj.Models
{
    class Fixture
    {
        public Fixture(Team homeTeam, Team awayTeam)
        {
            HomeTeam = homeTeam;
            AwayTeam = awayTeam;
        }

        public Fixture CloneFixture()
        {
            return new Fixture(HomeTeam, AwayTeam);
        }

        public Team HomeTeam { get; set; }
        public Team AwayTeam { get; set; }
    }
}
