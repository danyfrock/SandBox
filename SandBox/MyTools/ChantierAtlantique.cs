using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandBox.MyTools
{
    internal class ChantierAtlantique
    {
        /*
NICOLAS : x:5,y:3 ; x:4,y:3 ; x:3,y:3 ; x:3,y:4 ; x:2,y:4 ; x:2,y:5 ; x:2,y:6
GAILLET : x:7,y:2 ; x:6,y:3 ; x:7,y:4 ; x:8,y:4 ; x:8,y:5 ; x:7,y:5 ; x:7,y:6
CGI : x:1,y:7 ; x:1,y:8 ; x:2,y:8
LESCHANTIERSDELATLANTIQUE :
        x:1,y:0 ; x:1,y:1 ; x:2,y:1 ; x:2,y:2 ; x:3,y:2 ; x:3,y:1 ; x:3,y:0 ;
        x:4,y:0 ; x:5,y:0 ; x:6,y:0 ; x:7,y:0 ; x:8,y:0 ; x:8,y:1 ; x:7,y:1 ;
        x:6,y:1 ; x:5,y:1 ; x:5,y:2 ; x:6,y:2 ; x:6,y:3 ; x:5,y:3 ; x:5,y:4 ;
        x:6,y:4 ; x:6,y:5 ; x:6,y:6 ; x:7,y:5
         */
        private string[,] hiddenWords = new string[10, 10]
              {
////              0   1   2   3   4   5   6   7   8   9
                {"X","L","P","N","T","I","E","R","S","R"},// 0
                {"E","E","S","A","F","A","L","E","D","P"},// 1
                {"X","W","C","H","R","T","L","G","Y","U"},// 2
                {"T","U","M","C","I","N","A","A","U","I"},// 3
                {"O","C","L","O","W","T","I","I","L","K"},// 4
                {"K","W","A","L","L","A","Q","E","L","X"},// 5
                {"B","R","S","R","N","K","U","T","N","X"},// 6
                {"S","C","R","X","E","S","E","A","C","M"},// 7
                {"E","G","I","X","X","G","L","X","C","M"},// 8
                {"O","J","O","J","W","X","F","A","M","C"} // 9
              };

        public string GetCoordsText(string word)
        {
            IEnumerable<(int x, int y)> coords = FindWords(word);
            IEnumerable<string> coordText = coords.Select(c => $@"x:{c.x},y:{c.y}");
            string text = $@"{word} : {string.Join(" ; ", coordText)}";
            return text;
        }

        public IEnumerable<(int x, int y)> FindWords(string word) => FindWords(hiddenWords, word);

        private IEnumerable<(int x, int y)> FindWords(string[,] letters, string word)
        {
            IEnumerable<IEnumerable<(int x, int y)>> wordCoords = FindWordCoords(letters, word);
            IEnumerable<IEnumerable<(int x, int y)>> all = AllPossibilities(wordCoords);
            IEnumerable<(int x, int y)> response = all.First(poss => AreAllConsecutives(poss));
            return response;
        }

        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(IEnumerable<IEnumerable<T>> sequences, Func<IEnumerable<T>, bool> condition) =>
    sequences.Aggregate(
        (IEnumerable<IEnumerable<T>>)new[] { Enumerable.Empty<T>() },
        (accumulator, sequence) =>
            accumulator.Where(a => condition(a)).SelectMany(
                partial => sequence,
                (partial, item) => partial.Append(item)
            )
    );

        public IEnumerable<IEnumerable<(int x, int y)>> AllPossibilities(IEnumerable<IEnumerable<(int x, int y)>> sequences) =>
            CartesianProduct(sequences, AreAllConsecutives);

        private bool AreAllConsecutives(IEnumerable<(int x, int y)> coords) => 
            coords.Zip(coords.Skip(1)).All(c => AreConsecutives(c.First, c.Second));

        private bool AreConsecutives((int x, int y)? coord1, (int x, int y)? coord2)
        {
            int x1 = coord1?.x?? -1;
            int y1 = coord1?.y?? -1;
            int x2 = coord2?.x?? -1;
            int y2 = coord2?.y?? -1;

            //chek =. = is not consecutive
            if (x1 == x2 && y1 == y2) { return false; }

            //calculate
            bool response = Math.Abs(x1 - x2) <= 1 && Math.Abs(y1 - y2) <= 1;
            return response;
        }

        /// <summary>
        /// Find response wordCoords of letters of a word.
        /// </summary>
        private IEnumerable<IEnumerable<(int x, int y)>> FindWordCoords(string[,] letters, string word)
        {
            IEnumerable<IEnumerable<(int x, int y)>> coords = word.Select(l => FindCoords(letters, l.ToString()));

            return coords;
        }

        /// <summary>
        /// Find wordCoords of a letters
        /// </summary>
        private IEnumerable<(int x, int y)> FindCoords(string[,] letters, string lettre)
        {
            for (int i = 0; i < letters.GetLength(0); i++)
            {
                {
                    for (int j = 0; j < letters.GetLength(1); j++)
                    {
                        if (letters[i, j] == lettre)
                        {
                            yield return (x: j, y: i);
                        }
                    }
                }
            }
        }
    }//class
}//namespace
