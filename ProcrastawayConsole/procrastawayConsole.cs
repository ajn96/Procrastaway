using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Procrastaway;

namespace ProcrastawayConsole
{
    using procCore = Procrastaway.core.ProcrastawayCore;

    public class ProcrastawayConsole
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
            "deathloop.exe",
            "smite.exe"
        };

        /// <summary>
        /// Track if playtime is reported
        /// </summary>
        private bool playtime_report = true;

        /// <summary>
        /// Track path for current executable
        /// </summary>
        private string exePath;

        /// <summary>
        /// Private constructor to set current dir
        /// </summary>
        public ProcrastawayConsole()
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
            if (!ParseUserConfig(exePath + "user_config.txt"))
            {
                CreateDefaultConfigFile(exePath + "user_config.txt");
                FirstTimeSetupMessage();
            }

            /* Configure the Procrastaway monitor */
            procCore.Instance.SetGameList(game_process_list);
            procCore.Instance.SetWeeklyGameTime(1);
            //procCore.Instance.setWeeklyGameTime(week_game_time_hrs * 60);

            /* Start the  monitor */
            procCore.Instance.Start(exePath + "time_log.txt");

            /* Configure console options */
            int cursorStart = 0;
            if (playtime_report)
            {
                Console.Write("Weekly playtime (sec): ");
                /* Record cursor starting position so we can reset later to count timer up */
                cursorStart = Console.CursorLeft;
                Console.CursorVisible = false;
            }
            else
            {
                /* Wait until we are killed, so hide console */
                Console.WindowWidth = 1;
                Console.WindowHeight = 1;
            }

            /* Cyclic executive */
            while (true)
            {
                if (playtime_report)
                {
                    Console.SetCursorPosition(cursorStart, Console.CursorTop);
                    Console.Write(procCore.Instance.GetCurrentWeeklyGameTimeSec());
                }
                System.Threading.Thread.Sleep(100);
            }
        }

        public void Stop()
        {
            procCore.Instance.Stop();
        }

        /// <summary>
        /// Try to parse user config file for all settings
        /// </summary>
        /// <param name="configPath">Path to the config file</param>
        /// <returns>True is successful parse, false otherwise</returns>
        private bool ParseUserConfig(string configPath)
        {
            string[] contents;
            bool goodFile = false;
            List<string> games = new List<string>();

            try
            {
                contents = File.ReadAllLines(configPath);
                /* Parse allowed time */
                week_game_time_hrs = Convert.ToInt32(contents[1]);
                /* Parse playtime report setting */
                if (!Boolean.TryParse(contents[3], out playtime_report))
                {
                    /* Yes I am lazy */
                    throw new Exception();
                }
                /* Parse game list */
                if (contents.Length > 5)
                {
                    for (int i = 5; i < contents.Length; i++)
                    {
                        games.Add(contents[i]);
                    }
                    game_process_list = games.ToArray();
                }
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
        private void CreateDefaultConfigFile(string path)
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

        private void FirstTimeSetupMessage()
        {
            AnnoyingSlowPrintLn("Welcome to Procrastiway!");
            AnnoyingSlowPrintLn("...");
            AnnoyingSlowPrintLn("Thank you for using this app. I made it while I was procrastinating on a school assignment.");
            AnnoyingSlowPrintLn("but I figure thats a better use of time than just playing games");
            AnnoyingSlowPrintLn("...");
            AnnoyingSlowPrintLn("Usage should be very simple. There is a single user_config.txt file in the directory:");
            AnnoyingSlowPrintLn(exePath);
            AnnoyingSlowPrintLn("Edit the config file to change the allowable weekly gaming time. The default is " + week_game_time_hrs + " hours.");
            AnnoyingSlowPrintLn("Game playtime is tracked on a rolling period lasting one week.");
            AnnoyingSlowPrintLn("To hide this window, edit the config file and disable game playtime display.");
            AnnoyingSlowPrintLn("...");
            AnnoyingSlowPrintLn("Now lets go be productive! Press enter to continue");
            Console.Read();
            Console.Clear();
        }

        private void AnnoyingSlowPrintLn(string line)
        {
            int sleepTimeMs = 3;
            foreach (char c in line)
            {
                Console.Write(c);
                System.Threading.Thread.Sleep(sleepTimeMs);
                if (Console.KeyAvailable)
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
