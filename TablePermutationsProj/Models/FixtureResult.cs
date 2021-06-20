using System;
using System.Collections.Generic;
using System.Text;

namespace TablePermutationsProj.Models
{
    class FixtureResult
    {
        public FixtureResult(Fixture fixture, Result result)
        {
            Fixture = fixture;
            Result = result;
        }

        public FixtureResult CloneFixtureResult()
        {
            return new FixtureResult(Fixture.CloneFixture(), Result.CloneResult());
        }

        public Fixture Fixture { get; set; }
        public Result Result { get; set; }
    }
}
