using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Procrastaway.core
{
    /// <summary>
    /// Top level interface for procrastiway
    /// </summary>
    public class procrastawayCore
    {
        public event EventHandler MaxGameTimeReached;

        public event EventHandler GamePlayable;

        public event EventHandler GameTimeExceeded;

        /// <summary>
        /// Tick rate for the monitor. Results in worst case log of
        /// 4.614 MB. Default 10s tick rate results in 0.46MB worst
        /// case log size.
        /// </summary>
        private readonly int TICK_PERIOD_SEC = 1;

        /* Singleton instance */
        private static procrastawayCore instance;

        /* Processes to track */
        private string[] track_procs;

        /* Allowed process time (seconds) */
        private int process_time_sec;

        /* Instance of activity monitor */
        private activityMonitor monitor;

        /* Thread which supervises gameplay and raises events */
        private Thread gameSupervisor;
        private bool supervisorRunning;

        /* Track if we have reached the gaming time limit in current instance */
        private bool threshReachedInCurrentSession = false;

        /// <summary>
        /// Private constructor for singleton
        /// </summary>
        private procrastawayCore()
        {
        }

        /// <summary>
        /// Get the Procrastaway instance
        /// </summary>
        public static procrastawayCore Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new procrastawayCore();
                }
                return instance;
            }
        }

        public int getCurrentWeeklyGameTimeSec()
        {
            return monitor.getCurrentLoggedTimeSec();
        }

        /// <summary>
        /// Set list of games to track and block
        /// </summary>
        /// <param name="gameList">Array of game process names</param>
        public void setGameList(string[] gameList)
        {
            track_procs = gameList;
        }

        /// <summary>
        /// Set the weekly game time limit, in hours. Games launched after the total screen
        /// time has elapsed will be blocked.
        /// </summary>
        /// <param name="timeHrs"></param>
        public void setWeeklyGameTime(int timeMins)
        {
            if (timeMins < 1)
                throw new ArgumentException("Minimum game time of 1 min...");
            /* Work internally in seconds */
            process_time_sec = timeMins * 60;
        }

        /// <summary>
        /// Start monitoring for games. Runs async
        /// </summary>
        public void start(string logDir)
        {
            /* Monitor tracks game time */
            monitor = new activityMonitor(logDir, track_procs, TICK_PERIOD_SEC);
            monitor.start();

            /* Supervisor kills game processes if the total playtime has been exceeded */
            supervisorRunning = true;
            gameSupervisor = new System.Threading.Thread(new ThreadStart(gameSupervisorWorker));
            gameSupervisor.Start();
        }

        /// <summary>
        /// Stop the game monitoring system.
        /// </summary>
        public void stop()
        {
            supervisorRunning = false;
            monitor.stop();
        }

        /// <summary>
        /// Worker thread for the game supervisor. This thread checks if the total weekly playtime has
        /// been exceeded, and if so, kills all eligible processes. This prevents the user from starting
        /// a game once the weekly time has been exceeded.
        /// </summary>
        void gameSupervisorWorker()
        {
            int lastTime, curTime;
            lastTime = 0;
            curTime = 0;
            while(supervisorRunning)
            {
                lastTime = curTime;
                curTime = getCurrentWeeklyGameTimeSec();
                /* Are we over time? */
                if (curTime >= process_time_sec)
                {
                    if (lastTime < process_time_sec)
                    {
                        threshReachedInCurrentSession = true;
                        MaxGameTimeReached?.Invoke(this, new EventArgs());
                    }
                    else
                    {
                        GameTimeExceeded?.Invoke(this, new EventArgs());
                    }

                    /* Kill processes only if we started over time, to be generous */
                    if(!threshReachedInCurrentSession)
                    {
                        foreach (string proc in track_procs)
                        {
                            ProcessManager.KillProcess(proc);
                        }
                    }
                }
                /* We have time! */
                else
                {
                    /* Did we just come back under the limit? */
                    if (lastTime >= process_time_sec)
                    {
                        GamePlayable?.Invoke(this, new EventArgs());
                    }
                }

                /* If user exits game, prevent re-opening */
                if(!monitor.IsProcessRunning)
                {
                    threshReachedInCurrentSession = false;
                }

                /* Sleep for a system tick */
                System.Threading.Thread.Sleep(TICK_PERIOD_SEC);
            }
        }
    }
}
