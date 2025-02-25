using ExcelFluentTools;
using SandBox.MyTools;

namespace SandBox.TU
{
    internal static class ExcelTU
    {
        public static string GetCommonPrefix(string str1, string str2)
        {
            if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2))
                return string.Empty;

            const char diffChar = '\0';
            return new string(str1
                .Zip(str2, (c1, c2) =>
                {
                    return (c1 == c2) ? c1 : diffChar;
                })
                .TakeWhile(c => c != diffChar)
                .ToArray());
        }

        public static bool DoSomething()
        {
            var excelTools = new ExcelTools()
                .DefinirWorkBook(@"C:\Users\daniel.lorenzi\Documents\Chavigny\tests\Chavigny GCE 2024 pour test.xlsx")
                .DefinirWorkSheet("Chavigny 2024")
                .DefinirColumns(new List<string> { "REFERENCE", "LIBELLE", "PRIX CAT", "PRIX NET" });

            var data = excelTools.GetTextForColumns();

            string valueTable = Chavy.BuildValuesTableForCTE(data);
            Console.WriteLine(valueTable);


            return !string.IsNullOrEmpty(valueTable);
        }//DoSomething

        public static bool CompleteReccordTest()
        {
            return Chavy.CompleteReccord(new List<string> { "a", "b", "c" }, 4).ToList()
                .Zip(new List<string> { "a", "b", "c", string.Empty }, (obtenu, attendu) => new { obtenu, attendu })
                .All(pair => pair.obtenu == pair.attendu); // Comparer les valeurs dans les tuples
        }

        public static bool BuildValueOfTableWithTest()
        {
            string obtenu = Chavy.BuildValueOfTableWith(new List<string> { "a", "b", });
            const string attendu = "('a', 'b')";
            bool result = obtenu == attendu;

            if (!result)
            {
                Console.WriteLine($@"attendu = {attendu} et obtenu = {obtenu}"); 
            }
            
            return result;
        }

        public static bool BuildValuesOfTableWithTest()
        {
            string obtenu = Chavy.BuildValuesOfTableWith(new List<string> {
                "(liste de val 1)",
                "(...2)",
                "(...)"
            }, 3);

            string attendu = $@"(VALUES
(liste de val 1),
(...2),
(...)) as t (col1, col2, col3)";

            bool result = obtenu == attendu;

            if (!result)
            {
                Console.WriteLine($@"attendu =
{attendu}
et obtenu =
{obtenu}
common =
{ GetCommonPrefix(attendu, obtenu)}");
            }

            return result;
        }

        public static bool GetCommonPrefixTest()
        {
            string a = "123abc";
            string b = "456def";
            string c = "_";
            string d = $@"{a}{b}";
            string e = $@"{a}{c}";

            string comp = GetCommonPrefix(d, e);

            bool result = comp == a;

            Console.WriteLine($@"{result} : {d} have {a} in common with {e} and we found  {comp}");

            return result;
        }
    }//classe
}//namespace
