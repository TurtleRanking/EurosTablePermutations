using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TablePermutationsProj.Models;

namespace TablePermutationsProj.Comparators
{
    class TableRowComparator : IComparer<TableRow>
    {
        public int Compare(TableRow first, TableRow second)
        {
            if (first != null && second != null)
            {
                if (TryNumericComparison(first.Points, second.Points, out int pointsResult))
                {
                    return pointsResult;
                }

                if (TryHeadToHeadPointsComparison(first, second, out int h2hPointsResult))
                {
                    return h2hPointsResult;
                }

                /* Only one match can be head to head. If we got here then it's going to be a draw so goal difference, and goals scored will be the same
                if (TryHeadToHeadGoalDifferenceComparison(first, second, out int h2hGoalDifferenceResult))
                {
                    return h2hGoalDifferenceResult;
                }
                */

                /*
                 Head to head goals scored would be here if not for the logic mentioned above
                 */

                if (TryNumericComparison(first.GoalDifference, second.GoalDifference, out int goalDifferenceResult))
                {
                    return goalDifferenceResult;
                }

                if (TryNumericComparison(first.GoalsFor, second.GoalsFor, out int goalsScoredResult))
                {
                    return goalsScoredResult;
                }

                if (TryNumericComparison(first.Wins, second.Wins, out int winsResult))
                {
                    return winsResult;
                }

                //disciplinary tie breaking not supported. set flag for manually checking
                first.ManualSortCheckRequired = true;
                second.ManualSortCheckRequired = true;
                return 0;
            }

            if (first == null && second == null)
            {
                return 0;
            }

            if (first != null)
            {
                return -1;
            }

            return 1;
        }

        private bool TryNumericComparison(int first, int second, out int result)
        {
            result = second - first;
            if (result != 0)
            {
                return true;
            }
            return false;
        }

        private bool TryHeadToHeadPointsComparison(TableRow first, TableRow second, out int result)
        {
            var parentTable = first.ParentTable;
            var completedFixtureResults = parentTable.CompleteFixtures.Values.SelectMany(fr => fr)
                .Where(fr =>
                    (fr.Fixture.HomeTeam == first.Team) && (fr.Fixture.AwayTeam == second.Team)
                    || (fr.Fixture.HomeTeam == second.Team) && (fr.Fixture.AwayTeam == first.Team)
                );

            if (!completedFixtureResults.Any())
            {
                result = 0;
                return false;
            }

            var firstIsHomeFixtureResults = completedFixtureResults.Where(fr => fr.Fixture.HomeTeam == first.Team);
            var secondIsHomeFixtureResults = completedFixtureResults.Where(fr => fr.Fixture.HomeTeam == second.Team);

            var firstPoints = firstIsHomeFixtureResults
                .Select(fr => (fr.Result.HomeScore > fr.Result.AwayScore) ? 3 : ((fr.Result.HomeScore == fr.Result.AwayScore) ? 1 : 0)).Sum()
                + secondIsHomeFixtureResults
                .Select(fr => (fr.Result.AwayScore > fr.Result.HomeScore) ? 3 : ((fr.Result.HomeScore == fr.Result.AwayScore) ? 1 : 0)).Sum();

            var secondPoints = firstIsHomeFixtureResults
                .Select(fr => (fr.Result.AwayScore > fr.Result.HomeScore) ? 3 : ((fr.Result.HomeScore == fr.Result.AwayScore) ? 1 : 0)).Sum()
                + secondIsHomeFixtureResults
                .Select(fr => (fr.Result.HomeScore > fr.Result.AwayScore) ? 3 : ((fr.Result.HomeScore == fr.Result.AwayScore) ? 1 : 0)).Sum();

            return TryNumericComparison(firstPoints, secondPoints, out result);
        }

        /*private bool TryHeadToHeadGoalDifferenceComparison(TableRow first, TableRow second, out int result)
        {
            var parentTable = first.ParentTable;
            var completedFixtureResults = parentTable.CompleteFixtures.Values.SelectMany(fr => fr)
                .Where(fr =>
                    (fr.Fixture.HomeTeam == first.Team) && (fr.Fixture.AwayTeam == second.Team)
                    || (fr.Fixture.HomeTeam == second.Team) && (fr.Fixture.AwayTeam == first.Team)
                );

            if (!completedFixtureResults.Any())
            {
                result = 0;
                return false;
            }

            var firstIsHomeFixtureResults = completedFixtureResults.Where(fr => fr.Fixture.HomeTeam == first.Team);
            var secondIsHomeFixtureResults = completedFixtureResults.Where(fr => fr.Fixture.HomeTeam == second.Team);

            var firstGoalDifference = (firstIsHomeFixtureResults.Sum(fr => fr.Result.HomeScore) + secondIsHomeFixtureResults.Sum(fr => fr.Result.AwayScore))
                - (firstIsHomeFixtureResults.Sum(fr => fr.Result.AwayScore) + secondIsHomeFixtureResults.Sum(fr => fr.Result.HomeScore));

            var secondGoalDifference = (firstIsHomeFixtureResults.Sum(fr => fr.Result.AwayScore) + secondIsHomeFixtureResults.Sum(fr => fr.Result.HomeScore))
                - (firstIsHomeFixtureResults.Sum(fr => fr.Result.HomeScore) + secondIsHomeFixtureResults.Sum(fr => fr.Result.AwayScore));

            return TryNumericComparison(firstGoalDifference, secondGoalDifference, out result);
        }*/
    }
}
