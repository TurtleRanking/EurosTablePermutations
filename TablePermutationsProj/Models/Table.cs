using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TablePermutationsProj.Comparators;

namespace TablePermutationsProj.Models
{
    class Table
    {
        public Table(int currentRound, IDictionary<int, List<FixtureResult>> completeFixtures, IDictionary<int, List<Fixture>> upcomingFixtures)
        {
            CurrentRound = currentRound;
            CompleteFixtures = completeFixtures;
            UpcomingFixtures = upcomingFixtures;
            TableRows = InitialiseTableRowsFromCompleteResults(completeFixtures);
            TableRowsKeyed = TableRows.ToDictionary(tr => tr.Team, tr => tr); //convenience variable
            StoredPermutations = new List<Table>();

            ApplyTableRowsFromCompleteResults(completeFixtures);
            SortTable();
        }
        public TableRow[] TableRows { get; set; }
        public IDictionary<Team, TableRow> TableRowsKeyed { get; set; }
        public int CurrentRound { get; set; }
        public IDictionary<int, List<FixtureResult>> CompleteFixtures { get; set; }
        public IDictionary<int, List<Fixture>> UpcomingFixtures { get; set; }
        public IList<Table> StoredPermutations { get; set; }

        //when a new permutation table is created, semantically data should be kept the same but later this as data may be changed
        //clone the object as to not modify old referenced data
        public Table CloneTable()
        {
            var clonedCompleteFixtures = CompleteFixtures.ToDictionary(fkp => fkp.Key, fkp => fkp.Value.Select(f => f.CloneFixtureResult()).ToList());
            var clonedUpcomingFixtures = UpcomingFixtures.ToDictionary(fkp => fkp.Key, fkp => fkp.Value.Select(f => f.CloneFixture()).ToList());
            return new Table(CurrentRound, clonedCompleteFixtures, clonedUpcomingFixtures);
        }

        private TableRow[] InitialiseTableRowsFromCompleteResults(IDictionary<int, List<FixtureResult>> completeFixtures)
        {
            var tableRowsList = new List<TableRow>();
            var orderedKeys = completeFixtures.Keys.OrderBy(k => k);
            var firstFixtureResults = completeFixtures[orderedKeys.First()];
            foreach (var currentFixtureResult in firstFixtureResults)
            {
                if (!tableRowsList.Any(tr => tr.Team == currentFixtureResult.Fixture.HomeTeam))
                {
                    tableRowsList.Add(new TableRow(this, currentFixtureResult.Fixture.HomeTeam, 0, 0, 0, 0, 0));
                }
                if (!tableRowsList.Any(tr => tr.Team == currentFixtureResult.Fixture.AwayTeam))
                {
                    tableRowsList.Add(new TableRow(this, currentFixtureResult.Fixture.AwayTeam, 0, 0, 0, 0, 0));
                }
            }
            return tableRowsList.ToArray();
        }

        private void ApplyTableRowsFromCompleteResults(IDictionary<int, List<FixtureResult>> completeFixtures)
        {
            var orderedKeys = completeFixtures.Keys.OrderBy(k => k);

            foreach (var key in orderedKeys)
            {
                foreach (var currentFixtureResult in completeFixtures[key])
                {
                    ApplyFixtureResult(this, currentFixtureResult, false);
                }
            }
        }

        public void CalculatePermutations(int maxGoals)
        {
            if (!UpcomingFixtures.ContainsKey(CurrentRound + 1))
            {
                return;
            }
            var nextFixtures = UpcomingFixtures[CurrentRound + 1];
            var allFixturesPossibleResults = new Dictionary<Fixture, List<Result>>();
            foreach (var currentFixture in nextFixtures)
            {
                var currentFixturePossibleResults = new List<Result>();
                foreach (var homeScore in Enumerable.Range(0, maxGoals + 1))
                {
                    foreach (var awayScore in Enumerable.Range(0, maxGoals + 1))
                    {
                        currentFixturePossibleResults.Add(new Result(homeScore, awayScore));
                    }
                }
                allFixturesPossibleResults[currentFixture] = currentFixturePossibleResults;
            }
            IterateResults(allFixturesPossibleResults);
        }
        
        private void IterateResults(Dictionary<Fixture, List<Result>> results, List<FixtureResult> currentFixtureResults = null)
        {
            if (results.Count == 0)
            {
                var permutationTable = CloneTable();

                permutationTable.CurrentRound++;
                permutationTable.RemoveCurrentFixtures();
                permutationTable.AddExistingRound();

                foreach (var currentFixtureResult in currentFixtureResults)
                {
                    ApplyFixtureResult(permutationTable, currentFixtureResult, true);
                }

                permutationTable.SortTable();

                permutationTable.PrintTable();

                StoredPermutations.Add(permutationTable);

                return;
            }

            if (currentFixtureResults == null)
            {
                currentFixtureResults = new List<FixtureResult>();
            }

            var keys = results.Keys.ToArray();
            var firstKey = keys[0];
            var restOfKeys = keys.Skip(1);

            foreach (var currentResult in results[firstKey])
            {
                currentFixtureResults.Add(new FixtureResult(firstKey, currentResult));
                var remainingDictionary = results.Where(kvp => restOfKeys.Contains(kvp.Key)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                IterateResults(remainingDictionary, currentFixtureResults);
                currentFixtureResults.RemoveAt(currentFixtureResults.Count - 1);
            }
        }

        private static void ApplyFixtureResult(Table applyTable, FixtureResult fixtureResult, bool alterRounds)
        {
            var homeRow = applyTable.TableRowsKeyed[fixtureResult.Fixture.HomeTeam];
            var awayRow = applyTable.TableRowsKeyed[fixtureResult.Fixture.AwayTeam];
            if (fixtureResult.Result.HomeScore > fixtureResult.Result.AwayScore)
            {
                homeRow.Wins++;
                awayRow.Losses++;
            }
            else if (fixtureResult.Result.HomeScore < fixtureResult.Result.AwayScore)
            {
                awayRow.Wins++;
                homeRow.Losses++;
            }
            else
            {
                homeRow.Draws++;
                awayRow.Draws++;
            }
            homeRow.GoalsFor += fixtureResult.Result.HomeScore;
            homeRow.GoalsAgainst += fixtureResult.Result.AwayScore;
            awayRow.GoalsFor += fixtureResult.Result.AwayScore;
            awayRow.GoalsAgainst += fixtureResult.Result.HomeScore;

            if (alterRounds)
            {
                applyTable.AddToExistingRound(fixtureResult.CloneFixtureResult());
            }
        }

        private void AddExistingRound()
        {
            CompleteFixtures.Add(CurrentRound, new List<FixtureResult>());
        }

        private void AddToExistingRound(FixtureResult fixtureResult)
        {
            CompleteFixtures[CurrentRound].Add(fixtureResult);
        }

        private void RemoveCurrentFixtures()
        {
            if (UpcomingFixtures.ContainsKey(CurrentRound))
            {
                UpcomingFixtures.Remove(CurrentRound);
            }
        }

        private void SortTable()
        {
            var sortedList = TableRows.ToList();
            sortedList.Sort(new TableRowComparator());
            TableRows = sortedList.ToArray();
        }

        public void PrintTable()
        {
            Console.WriteLine("");
            Console.WriteLine("---");
            Console.WriteLine($"{"Name",-20}\tW\tD\tL\tGF\tGA\tPTS");
            foreach (var tableRow in TableRows)
            {
                Console.WriteLine($"{tableRow.Team.DisplayName}\t{tableRow.Wins}\t{tableRow.Draws}\t{tableRow.Losses}\t{tableRow.GoalsFor}\t{tableRow.GoalsAgainst}\t{tableRow.Points}");
            }
            Console.WriteLine("---");
        }

        //output as CSV for Excel to read. Once loaded in Excel, simple and quick colouring could be applied
        public void PermutationsToCsv()
        {
            //todo
        }
    }
}
