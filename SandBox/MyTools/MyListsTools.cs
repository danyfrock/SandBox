using System.Linq;
using System.Text.RegularExpressions;

namespace SandBox.MyTools
{
    internal class MyListsTools
    {
        private const string left = "L";
        private const string right = "R";
        private const string up = "U";
        private const string down = "D";

        public static ISet<int[]> CartesianProduct(int[][] sets) =>
            sets.Aggregate(
                (IEnumerable<int[]>)new[] { new int[0] },
                (accumulator, next) =>
                     accumulator
                     .SelectMany(
                       cumul => next,
                       (cumul, item) => cumul.Concat(new[] { item }).ToArray()
                       )
            ).ToHashSet();

        public static ISet<int[]> CartesianProduct2(int[][] sets) =>
            sets.Aggregate(
                (IEnumerable<int[]>)new[] { new int[0] },
                (accumulator, next) =>
                     accumulator
                     .SelectMany(
                       cumul => next.Select(item => cumul.Concat(new int[] { item }).ToArray()
                       ))
            ).ToHashSet();

        public static List<string> Solve(char[][] mixedUpBoard, char[][] solvedBoard)
        {
            //check
            if (mixedUpBoard is null) return null;
            if (mixedUpBoard.Length == 0) return null;
            if (mixedUpBoard[0].Length == 0) return null;

            //are equals
            if (AreEquals(mixedUpBoard, solvedBoard))
            {
                return new List<string>();
            }


            //calculation
            int linesize = mixedUpBoard[0].Length;
            int colsize = mixedUpBoard.Length;

            // add index
            IEnumerable<(char c, int numligne, int numcol)> mixedUp = IndexedArray(mixedUpBoard);
            IEnumerable<(char c, int numligne, int numcol)> solved = IndexedArray(solvedBoard);

            //compare both
            IEnumerable<(char el, int li, int col, IEnumerable<string> movs)> calc =
            mixedUp.OrderBy(m => m.c)
            .Zip(solved.OrderBy(m => m.c),
            (z1, z2) => (z1.c, z1.numligne, z1.numcol, moves(z1.numligne, z1.numcol, z2.numligne, z2.numcol)));

            //simplification
            IEnumerable<(char el, int li, int col, IEnumerable<string> movs)> simplified =
                calc.Select(c => (c.el, c.li, c.col, realySimplifyMove(c.movs, linesize, colsize)));

            //move
            IEnumerable<string> result =
                simplified.Aggregate(new List<string>(),
                (acc, next) =>
                    acc.Concat(skipAlreadyDone(acc, next.movs))
                    .ToList());

            List<string> list = result.ToList();
            return list;
        }

        public static bool CanSkipNegatif() => new List<int>() { 0, 1, 2, }.Skip(-1).Any();

        private static bool AreEquals(char[][] mixedUpBoard, char[][] solvedBoard)
        {
            return mixedUpBoard.SelectMany(m => m.Select(i => i))
                            .Zip(solvedBoard.SelectMany(m => m.Select(i => i)), (m1, m2) => m1 == m2).All(z => z);
        }

        private static IEnumerable<string> skipAlreadyDone(IEnumerable<string> dones, IEnumerable<string> todos)
        {
            //regroup
            IEnumerable<IGrouping<string, string>> regroupedDones = dones.GroupBy(move => move).OrderBy(g => g.Key);
            IEnumerable<IGrouping<string, string>> regroupedTodos = todos.GroupBy(move => move).OrderBy(g => g.Key);


            //search already applied
            IEnumerable<IEnumerable<string>> skipped =
            regroupedTodos.Select(r => r.Skip(regroupedDones.Where(d => d.Key == r.Key).Count()));

            //return selection
            IEnumerable<string> enumerable = skipped.SelectMany(gr => gr.Select(el => el));
            return enumerable;
        }

        private static IEnumerable<string> realySimplifyMove(IEnumerable<string> moves, int linesize, int colsize)
        {
            return simplifyMove(
                moves.SelectMany(move => toUniqueMove(move,linesize,colsize)),
                linesize,
                colsize);
        }

        private static IEnumerable<string> simplifyMove(IEnumerable<string> moves, int linesize, int colsize)
        {
            //regroup
            IEnumerable<IGrouping<string, string>> regrouped = moves.GroupBy(move => move);

            //simplify
            IEnumerable<string> simplifiedGroups =
                regrouped.SelectMany(group =>
                group.Take(group.Count() % (group.Key == up || group.Key == down ? colsize : linesize)));

            return simplifiedGroups;
        }

        private static IEnumerable<string> moves (int ls, int cs, int le, int ce)
        {
            //         col 0, col 1
            // ligne 1   A  ,  *     A move right  *,A 
            string sensHorizontal = cs >= ce ? left : right;
            string sensVertical = ls >= le ? down : up;

            IEnumerable<string> horizontal =
                Enumerable.Range(0, Math.Abs(cs - ce)).Select(i => $"{sensHorizontal}{ls}");

            // vertical moves when horizontal moves are done, so colums have changed
            IEnumerable<string> vertical =
                Enumerable.Range(0, Math.Abs(ls - le)).Select(i => $"{sensVertical}{ce}");

            return horizontal.Concat(vertical);
        }

        private static IEnumerable<string> toUniqueMove(string move, int linesize, int colsize)
        {
            //prepare
            IEnumerable<string> uniquemove = new List<string>() { move };
            string motsens = "sens";
            string nb = "nb";
            Match match = Regex.Match(move, $@"(?<{motsens}>{left}|{up})(?<{nb}>\d)");
            bool success = match.Success;
            
            if (success)
            {

                GroupCollection captures = match.Groups;
                string sens = captures[motsens].Value;
                int n = int.Parse(captures[nb].Value);

                //determine unique sens
                string uniqSens = sens == left ? right : sens;
                uniqSens = uniqSens == up ? down : uniqSens;

                //determine number of moves
                int size = uniqSens == down ? colsize : linesize;
                int uniqNb = size - 1;

                //build moves
                uniquemove = Enumerable.Range(0, uniqNb).Select(i => $"{uniqSens}{n}");

            }

            return uniquemove;
        }

        private static IEnumerable<(char c, int numligne, int numcol)> IndexedArray(char[][] matrix)
        {
            var enumerable =
                        matrix
                        .Select((ligne, numligne) => (ligne, numligne))
                        .SelectMany(l => l.ligne.Select((c, numcol) => (c, l.numligne, numcol)));

            return enumerable;
        }

        public static string[] Solution(string str) =>
            str.Chunk(2).Select(l => string.Join(string.Empty,$"{string.Join(string.Empty, l)}_".Take(2))).ToArray();
        public static string[] SolutionAlt(string str) =>
            Regex.Matches($"{str}_",@"\w{2}").Select(s => s.Value).ToArray();

    }//class
}//namespace
