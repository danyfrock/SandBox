using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SandBox.MyTools
{
    internal class RegexCodeWar
    {
        public static string FireAndFury(string tweet)
        {
            //declare
            string fury = "FURY";
            string fire = "FIRE";
            string pattern = $@"({fury})|({fire})";
            const string fake = "Fake tweet.";

            //check
            if (!Regex.IsMatch(tweet, @"^[EFIRUY]*$"))
                return fake;

            //search
            MatchCollection matches = Regex.Matches(tweet, pattern);

            //check
            if (!matches.Any())
                return fake;

            //analyse
            const string sep = "|";
            string motsIsoles = matches.Aggregate(string.Empty, (acc, next) =>
                $"{acc}{next.Value}{(next.NextMatch().Success && next.Value != next.NextMatch().Value ? sep : string.Empty)}");
            List<string> phrases = motsIsoles.Split(sep).ToList();

            List<(string cle, int nb)> liste = phrases.Select(p =>
            {
                MatchCollection subMatch = Regex.Matches(p, pattern);
                return (subMatch?.FirstOrDefault()?.Value?? string.Empty, subMatch?.Count()??0);
            }).ToList();

            //build
            string soluce = string.Join(" ", liste.Select(l =>
            {
                string realTweet = string.Empty;
                if (l.cle == fury)
                {
                    realTweet =
                    $"I am{Enumerable.Range(0, l.nb - 1).Aggregate(string.Empty, (acc, next) => $"{acc} really")} furious.";
                }
                else if (l.cle == fire)
                {
                    realTweet =
                    $"You{Enumerable.Range(0, l.nb - 1).Aggregate(string.Empty, (acc, next) => $"{acc} and you")} are fired!";
                }
                else
                {
                    realTweet = fake;
                }

                return realTweet;
            }));

            //soluce
            return soluce;
        }
        public static string FireAndFuryAlt(string tweet)
        {
            var matches = new Regex("(FURY|FIRE)").Matches(tweet);
            if (Regex.Match(tweet, "[^EFIRUY]+").Success || !matches.Any()) return "Fake tweet.";

            return string.Join(" ", Regex.Matches(string.Concat(matches), @"(FURY|FIRE)\1*").Select(m =>
            {
                var n = m.Length / 4 - 1;
                return m.Value[1] == 'I'
                    ? "You " + string.Concat(Enumerable.Repeat("and you ", n)) + "are fired!"
                    : "I am " + string.Concat(Enumerable.Repeat("really ", n)) + "furious.";
            }));
        }
    }//classe
}//namespace
