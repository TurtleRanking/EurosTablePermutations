using System;
using System.Collections.Generic;
using System.Linq;
using TablePermutationsProj.Models;

namespace TablePermutationsProj
{
    class Program
    {
        static void Main()
        {
            //quick and unpolished code to see Euros possibilities

            //setup data
            var teamA = new Team("Czech Republic");
            var teamB = new Team("England");
            var teamC = new Team("Croatia");
            var teamD = new Team("Scotland");

            //round-fixtures mapping
            var completeFixtures = new Dictionary<int, List<FixtureResult>> {
                { 1, new List<FixtureResult>{
                    new FixtureResult(new Fixture(teamB, teamC), new Result(1, 0)),
                    new FixtureResult(new Fixture(teamD, teamA), new Result(0, 2)),
                }},
                { 2, new List<FixtureResult>{
                    new FixtureResult(new Fixture(teamC, teamA), new Result(1, 1)),
                    new FixtureResult(new Fixture(teamB, teamD), new Result(0, 0)),
                }},
            };

            var upcomingFixtures = new Dictionary<int, List<Fixture>> {
                { 3, new List<Fixture>{new Fixture(teamC, teamD), new Fixture(teamA, teamB) } },
            };

            var table = new Table(
                2,
                completeFixtures,
                upcomingFixtures
            );

            //see if there any ambiguous sorts prior to making permutations
            var existingManualChecks = table.StoredPermutations.Where(t => t.TableRows.Any(tr => tr.ManualSortCheckRequired)).ToList();

            //use a table as a seed to calculate further permutations (up to 4 goals currently)
            table.CalculatePermutations(4);

            //debugging code to check count Scotland placements
            //var blah1 = table.StoredPermutations.Where(t => t.TableRows[0].Team == teamD).ToList();
            //var blah2 = table.StoredPermutations.Where(t => t.TableRows[1].Team == teamD).ToList();
            //var blah3 = table.StoredPermutations.Where(t => t.TableRows[2].Team == teamD).ToList();
            //var blah4 = table.StoredPermutations.Where(t => t.TableRows[3].Team == teamD).ToList();

            //check if there are any ambiguous sorts after permutations (likely at disciplinary stage)
            existingManualChecks = table.StoredPermutations.Where(t => t.TableRows.Any(tr => tr.ManualSortCheckRequired)).ToList();

            //todo
            //table.PermutationsToCsv();

            Console.WriteLine("Hit any button to end");
            Console.ReadLine();
        }
    }
}
