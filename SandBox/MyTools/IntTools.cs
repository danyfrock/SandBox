
using System.Globalization;
using System.Security;
using System.Text.RegularExpressions;

namespace SandBox.MyTools
{
    internal class IntTools
    {
        private static Lazy<int> lazyComplete = new(() => chimesFrom00h00(12));
        private static int complete = lazyComplete.Value;

        //y=(x*x + x)/2 + 3x = x^2/2 + 7x/2; 2y = x^2 + 7x; x^2 + 7x - 2y = 0 
        private static int chimesFrom00h00(int x) => ((x * (x + 1)) / 2) + (x * 3);
        private static int minutesToChimes(int x) => x / 15;
        private static int hourFromChimes(int y)
        {
            /*
                Équation de départ (quadratique en x) :
                x^2 + 7x - 2y = 0
                Qu'on identifie sous la forme ax^2 + bx + c = 0 avec :
                a = 1, b = 7, c = -2y
             
                Discriminant (défini par la = 49 + 8y     * Discriminant (défini par la formule générale) :
             
                Comme y >= 0, Δ >= 49 > 0, donc il y a toujours deux solutions réelles.
                La solution utile (positive) pour x est :
                x = (-b + sqrt(Δ)) / (2a) = (-7 + sqrt(49 + 8y)) / 2
                On prend ensuite Floor(x) car on veut le nombre d’heures *complètes* écoulées.
                */

                if (y < 0) throw new ArgumentOutOfRangeException(nameof(y), "y doit être >= 0.");

                // Δ = 49 + 8y (toujours >= 49 si y >= 0)
                double delta = 49.0 + 8.0 * y;

                // Racine utile (la négative est hors domaine)
                double xPos = (-7.0 + Math.Sqrt(delta)) / 2.0;

                // On veut le nombre d’heures complètes écoulées
                int hours = (int)Math.Floor(xPos);

                // Logs concis (optionnel)
                Console.WriteLine($"y={y}, x+={xPos}, floor={hours}");

                return hours;
            }

        public static int CloseCompare(double a, double b, double margin = 0)
        {
            double absoluteDiff = Math.Abs(a - b);
            if (absoluteDiff <= margin) return 0;

            // diff > margin
            if(a > b) return 1;
            return -1;
        }

        public static string CuckooClock(string inputTime, int chimes)
        {
            //check
            if (chimes == 0) return inputTime;

            //prepare
            string hour = "hour";
            string min = "min";

            //extract
            string pattern = $@"(?<{hour}>\d\d):(?<{min}>\d\d)";
            GroupCollection captures = Regex.Match(inputTime, pattern).Groups;
            string inputHourText = captures[hour].Value;
            string inputMinutesText = captures[min].Value;
            int hours = int.Parse(inputHourText);
            int minutes = int.Parse(inputMinutesText);

            //calculate
            DateTime time = new DateTime(1,1,1,hours,(minutes/15)*15,0, new CultureInfo("en-US", false).Calendar);
            int n = chimes;

            //first chime
            if (minutes == 0) n = Math.Max(0, n - hours);
            if (minutes != 0 && minutes % 15 == 0) n = Math.Max(0, n - 1);
            n %= complete;

            //simulate
            while (n > 0)
            {
                time = time.AddMinutes(15);
                if (time.Minute == 0)
                {
                    int h = time.Hour;
                    if (time.Hour == 0) h = 12;
                    n -= h > 12 ? h - 12 : h;
                }
                else
                {
                    n--;
                }
            }

            string ret = time.ToString("hh:mm tt").Split(" ")[0];
            ////Console.WriteLine($"inputTime={inputTime}, chimes={chimes}, ret={ret}");
            return ret;
        }

        public static string QuadratikCuckooClock(string inputTime, int inputchimes)
        {
            //check
            if (inputchimes == 0) return inputTime;

            //prepare
            string hour = "hour";
            string min = "min";
            int chime = inputchimes % complete;

            //extract
            string pattern = $@"(?<{hour}>\d\d):(?<{min}>\d\d)";
            GroupCollection captures = Regex.Match(inputTime, pattern).Groups;
            string inputHourText = captures[hour].Value;
            string inputMinutesText = captures[min].Value;
            int hours = int.Parse(inputHourText);
            hours = hours > 12 ? hours - 12 : hours;
            int minutes = int.Parse(inputMinutesText);

            // calculate
            // intermediaire
            int nh = chimesFrom00h00(hours);
            int nm = minutesToChimes(minutes);
            int intermediaire = nh + nm + chime;

            // start by chime
            if (minutes == 0) intermediaire = Math.Max(0, intermediaire - hours);
            if (minutes != 0 && minutes % 15 == 0) intermediaire = Math.Max(0, intermediaire - 1);

            // complete round of clock
            int newChime = intermediaire % complete;

            // get hour and minute for chimes
            int finalH = hourFromChimes(newChime);
            int totalChimes = chimesFrom00h00(finalH);

            //build response
            int rest = newChime - totalChimes;
            int finalM = 0;
            if (rest > 3) finalH++;
            if (rest <= 3) finalM = rest * 15;

            ////Console.WriteLine($"rest={rest}, finalH={finalH}, finalM={finalM}");

            DateTime time = new DateTime(1, 1, 1, finalH, finalM, 0, new CultureInfo("en-US", false).Calendar);
            string r = time.ToString("hh:mm tt").Split(" ")[0];

            return r;
        }

        // Kata code war : Progressive Spiral Number Position.
        public static long Layers(long n)
        {
            // x = max number of a layer
            // k = layer
            // x = ((2k-1)^2)
            // x = 4k^2-4k+1
            // Delta = b^2 - 4ac
            // delta = 4^2 - 4(4 * (1-x))
            // delta = 4^2 - 4(4 - 4x)
            // delta = 16 - 16 + 16x
            // delta = 16x

            // k = -b ± √delta / 2a
            // k = (4 ± 4√x) / 8
            // k = 4 ± 4√x / 8
            // k = 4(1 ± √x) / 8
            // k = (1 ± √x) / 2

            // Nous prenons la solution positive car k doit être un nombre positif
            // k = (1 + √x) / 2

            return (int)Math.Ceiling((1 + Math.Sqrt(n)) / 2);
        }
    }//classe
}//naespace
