﻿using SandBox.MyTools;
using SandBox.TU;
using System.Linq.Expressions;

namespace SandBox
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //tests.TestSolveDBCA();
            //tests.testConvertbool();
            //if (MyListsTools.CanSkipNegatif()) return;
            tests.RunSomeTests(CodeWarTests());
            ////RunAllTests();
        }

        internal static List<Expression<Func<bool>>> ListForChavyTest()
            => new List<Expression<Func<bool>>>()
            {
                () => ExcelTU.CompleteReccordTest(),
                () => ExcelTU.BuildValueOfTableWithTest(),
                () => ExcelTU.BuildValuesOfTableWithTest(),
                () => ExcelTU.DoSomething(),
                () => ExcelTU.GetCommonPrefixTest(),
                () => tests.testRemoveDuplicatesAndRename(),
            };

        internal static void RunAllTests()
        {
            tests.RunSomeTests(Alltests());
        }

        internal static List<Expression<Func<bool>>> ImageTests()
        {
            return
            new List<Expression<Func<bool>>>()
            {
                () => tests.testExtractImage(),
                () => tests.testExtractThePass(),
            };
        }

        internal static List<Expression<Func<bool>>> CodeWarTests()
        {
            return
            new List<Expression<Func<bool>>>()
            {
                () => tests.TestBuildKeyCodes(),
                () => tests.TestCheckCodes(),
            };
        }

        internal static List<Expression<Func<bool>>> LoopOverTest()
        {
            return
            new List<Expression<Func<bool>>>()
            {

            };
        }

        internal static List<Expression<Func<bool>>> Alltests()
        {
            return new List<Expression<Func<bool>>>()
            {
                //CartesianProduct
                ()=>tests.TestCartesianProduct(),
                ()=>tests.TestGrannyHelpCodeWar()
            }
                //listes
                .Concat(listtests())

                //CuckoClock
                .Concat(AllCuckooClockTests())

                //ImageTests
                .Concat(ImageTests())

                //MummyTest
                .Concat (CodeWarTests())

                //liste done
                .ToList();
        }

        internal static List<Expression<Func<bool>>> listtests()
        {
            return
            new List<Expression<Func<bool>>>()
            {
                ()=>tests.TestLinqZipExcept(),
                ()=>tests.TestLinqExcept(),
                ()=>tests.TestClonageUtile(),

            };
        }

        internal static List<Expression<Func<bool>>> AllCuckooClockTests()
        {
            return new List<Expression<Func<bool>>>()
            {
                ()=>tests.TestCuckoClock(),
                ()=>tests.TestQuadratikCuckooClock(),
                ()=>tests.TestSpecialCompareCuckoClockVsQuadratikCuckooClock(),
                ()=>tests.TestCompareCuckoClockVsQuadratikCuckooClock(10000, 10000000),
                ()=>tests.TestRaceCompareCuckoClockVsQuadratikCuckooClock(),
                ()=>tests.TestMaxChimeQuadratikCuckooClock(),
                ()=>tests.TestMaxChimeCuckooClock(),

            };
        }
    }
}