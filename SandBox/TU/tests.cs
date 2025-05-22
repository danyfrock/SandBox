using SandBox.MyTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SandBox.TU
{
    /// <summary>
    /// Dummy modification
    /// </summary>
    internal class tests
    {
        private static string ToVeryShortString(DateTime time) => time.ToString("hh:mm tt").Split(" ")[0];
        private static int randomIntFromZeroTo(int value) 
        {
            int rnd = new Random().Next(0, value);
            return rnd;
        }
        private static DateTime randomHour()
        {
            var date = new DateTime(1, 1, 1, randomIntFromZeroTo(12), randomIntFromZeroTo(59), 0, new CultureInfo("en-US", false).Calendar);
            return date;
        }

        internal static bool TestCartesianProduct()
        {
            int[][] sets = new int[][]
                        {
                new int[] { 1, 2, 3 },
                new int[] { 4, 5,},
                new int[] { 6, 7, 8, 9},
                new int[] { 8, 9, 10, 11},
                        };

            var result = MyListsTools.CartesianProduct(sets);
            var result2 = MyListsTools.CartesianProduct2(sets);

            IEnumerable<int> many = result.SelectMany(i => i);
            IEnumerable<int> second = result2.SelectMany(i => i);
            IEnumerable<int> intersec = many.Intersect(second);
            IEnumerable<int> except = many.Except(second);
            bool v = intersec.Count() == many.Distinct().Count();
            bool v1 = !except.Any();
            bool v2 = v || v1;

            if (!v2)
            {
                throw new Exception("TestCartesianProduct fail !!!");
            }

            return v2;
        }

        internal static bool TestCuckoClock()
        {
            bool v = true;
            v &= IntTools.CuckooClock("03:38", 19) == "06:00";
            v &= IntTools.CuckooClock("08:17", 113) == "08:00";
            v &= IntTools.CuckooClock("08:17", 150) == "11:00";
            v &= IntTools.CuckooClock("08:17", 200) == "05:45";
            v &= IntTools.CuckooClock("10:06", 141) == "12:00";

            if(!v)
            {
                throw new Exception("TestCuckoClock fail !!!");
            }

            return v;
        }

        internal static bool TestQuadratikCuckooClock()
        {
            bool v = true;
            v &= IntTools.QuadratikCuckooClock("03:38", 19) == "06:00";
            v &= IntTools.QuadratikCuckooClock("08:17", 113) == "08:00";
            v &= IntTools.QuadratikCuckooClock("08:17", 150) == "11:00";
            v &= IntTools.QuadratikCuckooClock("08:17", 200) == "05:45";
            v &= IntTools.QuadratikCuckooClock("10:06", 141) == "12:00";

            if (!v)
            {
                throw new Exception("QuadratikCuckooClock fail !!!");
            }

            return v;
        }

        internal static bool TestMaxChimeQuadratikCuckooClock()
        {
            bool v = true;
            IntTools.QuadratikCuckooClock("23:59", int.MaxValue);
            return v;
        }

        internal static bool TestMaxChimeCuckooClock()
        {
            bool v = true;
            IntTools.CuckooClock("23:59", int.MaxValue);
            return v;
        }

        internal static bool TestCompareCuckoClockVsQuadratikCuckooClock(int numberOfTests, int maxTestsSize)
        {
            //prepare
            bool v = true;
            string paramDate = string.Empty;
            int chimes =0;
            string calculation = string.Empty;

            //action
            Enumerable.Range(0, numberOfTests).ToList().ForEach(x =>
            {
                DateTime date = randomHour();
                chimes = randomIntFromZeroTo(maxTestsSize);
                paramDate = ToVeryShortString(date);
                string simulation = IntTools.CuckooClock(paramDate, chimes);
                calculation = IntTools.QuadratikCuckooClock(paramDate, chimes);

                //Assert
                if (simulation != calculation) 
                {
                    throw new Exception($@"paramDate={paramDate}, chimes={chimes}
simulation={simulation} but
calculation={calculation}");
                }

                Console.WriteLine($"OK paramDate={paramDate}, chimes={chimes}, sol={calculation}");
            });

            return v;
        }

        internal static bool TestSpecialCompareCuckoClockVsQuadratikCuckooClock()
        {
            //prepare
            const string time = "12:00";
            const int chimes = 157890;

            //action
            string simulation = IntTools.CuckooClock(time, chimes);
            string calculation = IntTools.QuadratikCuckooClock(time, chimes);
            bool ok = simulation == calculation;

            //assert
            if (!ok) { throw new Exception($@"KO paramDate={time}, chimes={chimes}
expected {simulation} but return {calculation}"); }

            Console.WriteLine($@"OK paramDate={time}, chimes={chimes}");
            return ok;
        }

        internal static bool TestRaceCompareCuckoClockVsQuadratikCuckooClock()
        {
            //prepare
            const string time = "23:59";
            const int chimes = int.MaxValue;

            //action
            return TestRaceCompareCuckoClockVsQuadratikCuckooClock(time, chimes);
        }

        internal static bool TestRaceCompareCuckoClockVsQuadratikCuckooClock(string time, int chimes)
        {
            DateTime startSimu = DateTime.Now;
            string simulation = IntTools.CuckooClock(time, chimes);
            DateTime endSimu = DateTime.Now;
            TimeSpan tsimu = endSimu.Subtract(startSimu);

            DateTime startCalc = DateTime.Now;
            string calculation = IntTools.QuadratikCuckooClock(time, chimes);
            DateTime endCalc = DateTime.Now;
            TimeSpan tcalc = endCalc.Subtract(startCalc);

            bool ok = simulation == calculation;
            bool calcFaster = tcalc < tsimu;
            string winner = calcFaster ? "calculation" : "simulation";
            double factor = Math.Max(tcalc / tsimu, tsimu / tcalc);

            //assert
            Console.WriteLine($@"{(ok ? "OK ;)" : "KO :(")} paramDate={time}, chimes={chimes},
expected {simulation} {(ok ? "and" : "but")} return {calculation}");
            Console.WriteLine($"{winner} methode is faster by factor {factor}");
            return ok;
        }

        internal static bool TestLinqExcept()
        {
            var a = new List<string> {  "a", "a","a","b","c","d","d","e" };
            var b = new List<string> { "c" };
            var c = a.Except(b);
            var d = new List<string> { "a","b","d","e" };
            string sep = ",";

            string s1 = string.Join(sep, c.Select((s, i) => $"{s}{i}"));
            string s2 = string.Join(sep, d.Select((s, i) => $"{s}{i}"));
            const string seed = "=>";
            string s3 = c.Aggregate(seed, (acc,next) => $"{acc}{next}");
            string s4 = d.Aggregate(seed, (acc,next) => $"{acc}{next}");

            bool comp = s1 == s2;

            string egale = comp ? "=" : "!=";
            Console.WriteLine($"{s1}{egale}{s2}");
            Console.WriteLine($"{s3}{egale}{s4}");
            return comp;
        }

        internal static bool TestLinqZipExcept()
        {
            var a = new List<string> { "a", "a", "a", "b", "c", "d", "d", "e" };
            var b = new List<string> { "c" };
            var c = a.Except(b);
            var d = new List<string> { "a", "b", "d", "e" };
            string sep = ", ";
            
            IEnumerable<(string First, string Second)> zipped = c.Zip(d);
            bool comp = c.Count() == d.Count() &&  zipped.All(z => z.First.Equals(z.Second));
            string s1 = string.Join(sep, zipped.Select((s, i) => s.First));
            string s2 = string.Join(sep, zipped.Select((s, i) => s.Second));
            Console.WriteLine($"{s1}{(comp ? "=" : "!=")}{s2}");
            return comp;
        }

        internal static bool TestClonageUtile()
        {
            Dummyclasse foo = new Dummyclasse("a", 1);
            Dummyclasse bar = new Dummyclasse("b", 2);
            Dummyclasse truc = new Dummyclasse("c", 3);
            var l = new List<Dummyclasse> { foo, bar, truc };
            var l2 = l.Where(i => i.Name != "c");
            l2.First().Name = "z";
            bool areEquals = l.First().Name == l2.First().Name;
            Console.WriteLine($"{l.First().Name} {(areEquals?"==":"!=")} {l2.First().Name}");
            return areEquals;
        }

        internal static void RunTest(Expression<Func<bool>> test)
        {
            string methodeName = ((MethodCallExpression)test.Body).Method.Name;
            Console.WriteLine($"start test {methodeName}");
            Func<bool> run = test.Compile();
            if (!run()) { throw new Exception($"test failure : {methodeName} fail !!!"); }
            Console.WriteLine($"test {methodeName} success");
        }

        internal static void RunSomeTests(List<Expression<Func<bool>>> tests)
        {
            List<string> failedList = new List<string>();

            tests.ForEach(test =>
            {
                string methodeName = ((MethodCallExpression)test.Body).Method.Name;
                try
                {
                    RunTest(test);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    failedList.Add(methodeName);
                }
            });

            int failures = failedList.Count;
            Console.WriteLine($@"{(failures == 0? "All test passed" :$"{failures} test{(failures==1?"":"s")} failed")}
{failedList.Aggregate(string.Empty, (acc,next) => $"{acc},{next}")}");
        }

        internal static void TestSolve()
        {
            MyListsTools.Solve(
                new char[][] 
                {
                    new char[]{'a', 'b', 'c',},
                    new char[]{'d', 'e', 'f',},
                },
                new char[][]
                {
                    new char[]{'d', 'e', 'f',},
                    new char[]{'a', 'b', 'c',},
                }
                );
        }
        internal static void TestSolveDBCA()
        {
            MyListsTools.Solve(
                new char[][]
                {
                    new char[]{'D', 'B',},
                    new char[]{'C', 'A',},
                },
                new char[][]
                {
                    new char[]{'A', 'B',},
                    new char[]{'C', 'D', 'c',},
                }
                );
        }

        internal static bool testConvertbool() 
        {
            object o = 1 as object;
            object n = 0 as object;
            bool vrai = Convert.ToBoolean( o );
            bool faux = Convert.ToBoolean( n );
            return vrai && !faux;//=vrai
        }

        internal static bool testRemoveDuplicatesAndRename() 
        {
            //prepare
            List<string> list = new List<string>() { "foo","bar", "foo","bar", "foo", "foo", "bar", "foo"};
            List<string> attendu = new List<string>() { "foo", "bar", "foo1", "bar1", "foo2", "foo3", "bar2", "foo4" };

            //action
            List<string> result = Chavy.RemoveDuplicatesAndRename(list).ToList();

            //assert
            bool reponse = result.Zip( attendu ).All(z => z.First.Equals(z.Second));

            return reponse;
        }

        internal static bool testExtractImage()
        {
            string foobar = ImagesTools.ExtractTextFromImage(@"C:\Users\daniel.lorenzi\Documents\missions\Orléans\AXEREAL\image.png");
            string textToFind = @"4fbf74e2-073e-44d5-b2f3-6bc835160f52";
            bool test = foobar.Contains(textToFind); 

            return test;
        }
        internal static bool testExtractThePass()
        {
            string foobar = ImagesTools.ExtractTextFromImage(@"C:\Users\daniel.lorenzi\Documents\missions\Orléans\AXEREAL\pass.PNG");
            bool test = !string.IsNullOrEmpty(foobar);

            return test;
        }

        internal static bool TestGrannyHelpCodeWar()
        {
            string[] friends1 = new string[] { "A1", "A2", "A3", "A4", "A5" };
            string[][] fTowns1 = { new string[] { "A1", "X1" }, new string[] { "A2", "X2" }, new string[] { "A3", "X3" }, new string[] { "A4", "X4" } };
            Hashtable distTable1 = new Hashtable { { "X1", 100.0 }, { "X2", 200.0 }, { "X3", 250.0 }, { "X4", 300.0 } };
            return Tour.tour(friends1, fTowns1, distTable1) == 889;
        }

        internal static bool TestCheckCodes()
        {
            string[] messages = { "dance", "level", "right", "yours" };
            string[] secretes = { "tpnes", "irvri", "dkucr", "elghy" };
            return CheckCodes.SearchForKey(messages, secretes) == "akerilnuopty";
        }

        internal static bool TestBuildKeyCodes()
        {
            string[] messages = { "yours", "level", "dance", "right" };
            string[] secretes = { "tpnes", "irvri", "dkucr", "elghy" };
            return CheckCodes.BuildKeyCodes(messages.ToList(), secretes) == "akerilnuopty";
        }

        internal static bool TestFuryAndFire()
        {
            return RegexCodeWar.FireAndFury("FURYYYFIREYYFIRE") == "I am furious. You and you are fired!";
        }
        internal static bool TestFuryAndFireAlt()
        {
            return RegexCodeWar.FireAndFuryAlt("FURYYYFIREYYFIRE") == "I am furious. You and you are fired!";
        }


        public static bool TestWave()
        {
            var result = MyListsTools.Wave("hello");
            var expected = new List<string> { "Hello", "hEllo", "heLlo", "helLo", "hellO" };

            return result.Count == expected.Count &&
                   result.Zip(expected, (r, e) => r == e).All(match => match);
        }
    }//class
}//namespace
