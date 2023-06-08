using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Procrastaway;
using Procrastaway.core;

namespace ProcrastawayConsole
{
    using procCore = Procrastaway.core.ProcrastawayCore;

    public class ProcrastawayConsole
    {
        private SettingManager settings;
        private string exePath;

        /// <summary>
        /// Private constructor to set current dir
        /// </summary>
        public ProcrastawayConsole()
        {
            exePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location + @"\");
            settings = new SettingManager();

            /* Read in user time or game settings */
            if (!settings.ParseUserConfig(exePath + "user_config.txt"))
            {
                settings.CreateDefaultConfigFile(exePath + "user_config.txt");
                FirstTimeSetupMessage();
            }
        }

        /// <summary>
        /// Start operation of the console interface
        /// </summary>
        public void Start()
        {
            /* Print out executing path */
            Console.WriteLine("Starting operation from " + Assembly.GetEntryAssembly().Location);

            /* Play starting sound */
            SystemSounds.PlaySound(SystemSounds.Sounds.Started);

            /* Configure the Procrastaway monitor */
            procCore.Instance.SetGameList(settings.game_process_list);
            procCore.Instance.SetWeeklyGameTime(1);
            //procCore.Instance.setWeeklyGameTime(week_game_time_hrs * 60);

            /* Register event handlers for sounds */
            procCore.Instance.MaxGameTimeReached += MaxGameTimeReachedHandler;
            procCore.Instance.GameStopped += GameStoppedHandler;

            /* Start the  monitor */
            procCore.Instance.Start(exePath + "time_log.txt");

            /* Configure console options */
            int cursorStart = 0;
            if (settings.playtime_report)
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
                if (settings.playtime_report)
                {
                    Console.SetCursorPosition(cursorStart, Console.CursorTop);
                    Console.Write(procCore.Instance.GetCurrentWeeklyGameTimeSec());
                }
                System.Threading.Thread.Sleep(100);
            }
        }

        private void GameStoppedHandler(object sender, EventArgs e)
        {
            SystemSounds.PlaySound(SystemSounds.Sounds.GameStopped);
        }

        private void MaxGameTimeReachedHandler(object sender, EventArgs e)
        {
            SystemSounds.PlaySound(SystemSounds.Sounds.PlaytimeReached);
        }

        /// <summary>
        /// Its the details like this that make things worth it
        /// </summary>
        private void FirstTimeSetupMessage()
        {
            AnnoyingSlowPrintLn("Welcome to Procrastiway!");
            AnnoyingSlowPrintLn("...");
            AnnoyingSlowPrintLn("Thank you for using this app. I made it while I was procrastinating on a school assignment.");
            AnnoyingSlowPrintLn("but I figure thats a better use of time than just playing games");
            AnnoyingSlowPrintLn("...");
            AnnoyingSlowPrintLn("Usage should be very simple. There is a single user_config.txt file in the directory:");
            AnnoyingSlowPrintLn(exePath);
            AnnoyingSlowPrintLn("Edit the config file to change the allowable weekly gaming time. The default is " + settings.week_game_time_hrs + " hours.");
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
