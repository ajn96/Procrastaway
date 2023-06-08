using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Procrastaway
{
    using procCore = core.procrastawayCore;

    public class procrastawayConsole
    {
        /// <summary>
        /// Amount of game time allowed each week, in hours
        /// </summary>
        private int week_game_time_hrs = 5;

        /// <summary>
        /// Games to track and limit
        /// </summary>
        private string[] game_process_list =
        {
            "minecraft.exe",
            "counterstrike.exe",
            "dota2.exe",
            "leagueoflegends.exe",
            "worldofwarcraft.exe",
            "overwatch.exe",
            "hl2.exe",
            "rainbow6.exe",
            "pubg.exe",
            "fortntie.exe",
            "apexlegends.exe",
            "doometernal.exe",
            "residentevilvillage.exe",
            "valorant.exe",
            "rocketleague.exe",
            "skyrimse.exe",
            "fallout76.exe",
            "fifa22.exe",
            "nba2k22.exe",
            "gta5.exe",
            "reddeadredemption2.exe",
            "battlefield2042.exe",
            "far-cry-6.exe",
            "forza_horizon_5.exe",
            "halo_infinite.exe",
            "deathloop.exe"
        };

        /// <summary>
        /// Track if playtime is reported
        /// </summary>
        private int playtime_report = 1;

        /// <summary>
        /// Track path for current executable
        /// </summary>
        private string exePath;

        /// <summary>
        /// Private constructor to set current dir
        /// </summary>
        public procrastawayConsole()
        {
            exePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location + @"\");
        }

        /// <summary>
        /// Start operation of the console interface
        /// </summary>
        public void Start()
        {
            /* Print out executing path */
            Console.WriteLine("Starting operation from " + exePath);

            /* Read in user time or game settings */
            if (!parseUserConfig(exePath + "user_config.txt"))
            {
                createDefaultConfigFile(exePath + "user_config.txt");
                firstTimeSetupMessage();
            }

            /* Configure the Procrastaway monitor */
            procCore.Instance.setGameList(game_process_list);
            procCore.Instance.setWeeklyGameTime(1);
            //procCore.Instance.setWeeklyGameTime(week_game_time_hrs * 60);

            /* Start the  monitor */
            procCore.Instance.start(exePath + "time_log.txt");

            /* Configure console options */
            int cursorStart = 0;
            if(playtime_report == 0)
            {
                /* Wait until we are killed, so hide console */
                Console.WindowWidth = 0;
                Console.WindowHeight = 0;
            }
            else
            {
                Console.Write("Weekly playtime (sec): ");
                /* Record cursor starting position so we can reset later to count timer up */
                cursorStart = Console.CursorLeft;
                Console.CursorVisible = false;
            }

            /* Cyclic executive */
            while(true)
            {
                if (playtime_report != 0)
                {
                    Console.SetCursorPosition(cursorStart, Console.CursorTop);
                    Console.Write(procCore.Instance.getCurrentWeeklyGameTimeSec());
                }
                System.Threading.Thread.Sleep(100);
            }
        }

        public void stop()
        {
            procCore.Instance.stop();
        }

        private bool parseUserConfig(string configPath)
        {
            string[] contents;
            bool goodFile = false;
            
            try
            {
                contents = File.ReadAllLines(configPath);
                goodFile = true;
            }
            catch
            {
                goodFile = false;
            }
            return goodFile;
        }

        /// <summary>
        /// Write default times and game list to a file
        /// </summary>
        /// <param name="path">File to create and write to</param>
        private void createDefaultConfigFile(string path)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine("Enter weekly gaming hours on the line below:");
                writer.WriteLine(week_game_time_hrs.ToString());
                writer.WriteLine("Set following line to 1 for playtime report, 0 for no report:");
                writer.WriteLine(playtime_report.ToString());
                writer.WriteLine("Enter game executable names (such as \"hl2.exe\" on the lines below:");
                foreach (string game in game_process_list)
                {
                    writer.WriteLine(game);
                }
            }
            Console.WriteLine("Created default user config file at " + path);
        }

        private void firstTimeSetupMessage()
        {
            annoyingSlowPrintLn("Welcome to Procrastiway!");
            annoyingSlowPrintLn("...");
            annoyingSlowPrintLn("Thank you for using this app. I made it while I was procrastinating on a school assignment.");
            annoyingSlowPrintLn("but I figure thats a better use of time than just playing games");
            annoyingSlowPrintLn("...");
            annoyingSlowPrintLn("Usage should be very simple. There is a single user_config.txt file in the directory:");
            annoyingSlowPrintLn(exePath);
            annoyingSlowPrintLn("Edit the config file to change the allowable weekly gaming time. The default is " + week_game_time_hrs + " hours.");
            annoyingSlowPrintLn("Game playtime is tracked on a rolling period lasting one week.");
            annoyingSlowPrintLn("To hide this window, edit the config file and disable game playtime display.");
            annoyingSlowPrintLn("...");
            annoyingSlowPrintLn("Now lets go be productive! Press enter to continue");
            Console.Read();
            Console.Clear();
        }

        private void annoyingSlowPrintLn(string line)
        {
            int sleepTimeMs = 3;
            foreach(char c in line)
            {
                Console.Write(c);
                System.Threading.Thread.Sleep(sleepTimeMs);
                if(Console.KeyAvailable)
                {
                    /* Okkkk fine */
                    Console.ReadKey(true);
                    sleepTimeMs = 0;
                }
            }
            Console.Write(Environment.NewLine);
        }
    }


}
