using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Tour
{
    public static int tour(string[] arrFriends, string[][] ftwns, Hashtable h)
    {
        double real;
        int retour; 

        //extract data
        IEnumerable<string> twns = towns(arrFriends, ftwns);
        IEnumerable<DictionaryEntry> entries = h.Cast<DictionaryEntry>();
        Dictionary<string, double> map =
            entries.ToDictionary(e => e.Key?.ToString()?? "", e => (double)(e.Value?? (double)0));
        
        //extract town
        IEnumerable<double> distances = twns.Select(t => map[t]);
        
        //dictionnary index distance
        Dictionary<int, double> indexedDistances =
            distances.Select((distance, index) =>
            new {index, distance}).ToDictionary(atuple => atuple.index, atuple => atuple.distance);

        //calculate;
        real =
            indexedDistances.Skip(1)
            .Aggregate(indexedDistances.Values.First() + indexedDistances.Values.Last(),
            (acc, next) => acc + calculateOpposed(next.Value, indexedDistances[next.Key - 1]));
        retour = (int)real;

        return retour;
    }

    private static double calculateOpposed(double hypotenuse, double adjacent) =>
        //hypotenuse >= adjacent ?
        Math.Sqrt(Math.Pow(hypotenuse, 2) - Math.Pow(adjacent, 2))
        //:calculateOpposed(adjacent, hypotenuse)
        ;//programmaticaly, it would be better to check max. But in our exercice it s impossible and would have no sense because of geometric rules.

    private static IEnumerable<string> towns(string[] arrFriends, string[][] ftwns)
    {
        Dictionary<string, string> dictionary = ftwns.ToDictionary(ft => ft[0] ?? "", ft => ft[1] ?? "");
        IEnumerable<string> enumerable = arrFriends
            .Where(friend => dictionary.ContainsKey(friend))
            .Select(friend => dictionary[friend]);
        
        return enumerable;
    }
}
