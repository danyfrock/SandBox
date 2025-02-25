using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandBox.MyTools
{
    public static class CheckCodes
    {
        public static string SearchForKey(string[] messages, string[] secrects)
        {
            string codes = string.Empty;
            codes = CheckTheCodes(messages, secrects, oftenUsedCodes.Select(c => c.ToLower()).ToList());

            if(string.IsNullOrEmpty(codes))
            {
                List<string> allCodes = BuildKeyCodes(messages, secrects);
                codes = CheckTheCodes(messages, secrects, allCodes);
            }

            return codes;
        }

        internal static List<string> BuildKeyCodes(string[] messages, string[] secrets)
        {
            List<List<string>> permuts = GetPermutations(messages).Select(p => p.ToList()).ToList();

            List<string> codes =
                permuts
                .Select(permut => BuildKeyCodes(permut, secrets))
                .ToList();

            return codes;
        }

        internal static string BuildKeyCodes(List<string> permut ,string[] secrets)
        {
            string retour = permut.Zip(secrets).Aggregate(string.Empty, (acc, next) => CompleteCode(next.First, next.Second, acc));
            return retour;
        }

        private static List<IEnumerable<T>> GetPermutations2<T>(IEnumerable<T> list)
        {
            if (list.Count() == 1)
                return new List<IEnumerable<T>> { list };

            List<IEnumerable<T>> permuts = list.SelectMany((element, index) =>
                            GetPermutations2(list.Take(index).Concat(list.Skip(index + 1)))
                            .Select(permutation => new[] { element }.Concat(permutation))).ToList();
            
            return permuts;
        }

        private static List<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list)
        {
            return list.Aggregate(
                new List<IEnumerable<T>> { Enumerable.Empty<T>() },
                (acc, element) => acc.SelectMany(
                    permut => Enumerable.Range(0, permut.Count() + 1), // all possibility to insert element
                    (permut, index) => permut.Take(index).Concat(new[] { element }).Concat(permut.Skip(index)) // all string with inserted element
                ).ToList()
            );
        }

        private static string CheckTheCodes(string[] messages, string[] secrects, List<string> oftenUsedCOdes)
        {
            List<string> validCodes = oftenUsedCOdes.Where(o => IsCodeFor(messages, secrects, o)).ToList();

            string retour = validCodes.Any() ? validCodes.First() : string.Empty;
            return retour;
        }

        private static bool IsCodeFor(string[] messages, string[] secrets, string keyCode)
        {
            bool retour;

            bool doublons = keyCode.Distinct().Count() < keyCode.Count();
            bool goodLength = keyCode.Count() <= 12;

            retour = messages.Any(m => secrets.Any(s => IsCodeFor(m, s, keyCode)));
            return retour && !doublons && goodLength;
        }

        private static bool IsCodeFor(string message, string secret, string keyCode)
        {
            bool retour;
            IEnumerable<char[]> kc = keyCode.Chunk(2);
            retour = message.Zip(secret).All(s => kc.Any(k => k.Contains(s.First) && k.Contains(s.Second)) || s.First == s.Second);
            return retour;
        }

        private static string CompleteCode(string message, string secret, string keyCode)
        {
            string retour = string.Empty;
            IEnumerable<string> kc = keyCode.Chunk(2).Select(c => string.Join(string.Empty, c));
            IEnumerable<string> newCode = message.Zip(secret)
                            .Where(nc => nc.First != nc.Second)
                            .Select(s => string.Join(string.Empty, new List<char>() { s.First, s.Second }.OrderBy(c => c)))
                            ;
            newCode = kc.Concat(newCode).OrderBy(s => s).Distinct();

            retour = string.Join(string.Empty, newCode);

            return retour;
        }

        private static List<string> oftenUsedCodes => new List<string>()
{
    //"GADERYPOLUKI",
    //"POLITYKARENU",
    "KACEMINUTOWY",
    "KONIECMATURY",
    "ZAREWYBUHOKI",
    "BAWOLETYKIJU",
    "REGULAMINOWY",
}.Select(s => //s is one often used code
string.Concat(
    s.Chunk(2) //one chunk is a value pair
    .Select(chunk => string.Concat(chunk.OrderBy(c => c))) //each value pair is ordered and concat in string
    .OrderBy(chunk => chunk))) // all value pair of a often used code are ordered
            .ToList();

    }//class
}//namespace
