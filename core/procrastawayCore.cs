using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Procrastaway.core
{
    /// <summary>
    /// Top level interface for procrastiway
    /// </summary>
    public class ProcrastawayCore
    {
        /// <summary>
        /// Event which is raised when the maximim allowable game playtime is first reached
        /// </summary>
        public event EventHandler MaxGameTimeReached;

        /// <summary>
        /// Event which is raised when a game is first playable again, after time was previously exceeded
        /// </summary>
        public event EventHandler GamePlayable;

        /// <summary>
        /// Event which is raised at the tick rate of the system when the game playtime is exceeded
        /// </summary>
        public event EventHandler GameTimeExceeded;

        /// <summary>
        /// Event which is raised whenever a game is stopped
        /// </summary>
        public event EventHandler GameStopped;

        /// <summary>
        /// Tick rate for the monitor. Results in worst case log of
        /// 4.614 MB for default time of 1s.
        /// </summary>
        private readonly int TICK_PERIOD_SEC = 1;

        /* Singleton instance */
        private static ProcrastawayCore instance;

        /* Processes to track */
        private string[] track_procs;

        /* Allowed process time (seconds) */
        private int process_time_sec;

        /* Instance of activity monitor */
        private ActivityMonitor monitor;

        /* Thread which supervises gameplay and raises events */
        private Thread gameSupervisor;
        private bool supervisorRunning;

        /* Track if we have reached the gaming time limit in current instance */
        private bool threshReachedInCurrentSession = false;

        /// <summary>
        /// Private constructor for singleton
        /// </summary>
        private ProcrastawayCore()
        {
        }

        /// <summary>
        /// Get the Procrastaway instance
        /// </summary>
        public static ProcrastawayCore Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ProcrastawayCore();
                }
                return instance;
            }
        }

        /// <summary>
        /// Get the amount of time spent on games in the last week
        /// </summary>
        /// <returns>The time, in seconds</returns>
        public int GetCurrentWeeklyGameTimeSec()
        {
            return monitor.GetCurrentLoggedTimeSec();
        }

        /// <summary>
        /// Set list of games to track and block
        /// </summary>
        /// <param name="gameList">Array of game process names</param>
        public void SetGameList(string[] gameList)
        {
            track_procs = gameList;
        }

        /// <summary>
        /// Set the weekly game time limit, in hours. Games launched after the total screen
        /// time has elapsed will be blocked.
        /// </summary>
        /// <param name="timeHrs"></param>
        public void SetWeeklyGameTime(int timeMins)
        {
            if (timeMins < 1)
                throw new ArgumentException("Minimum game time of 1 min...");
            /* Work internally in seconds */
            process_time_sec = timeMins * 60;
        }

        /// <summary>
        /// Start monitoring for games. Runs async
        /// </summary>
        public void Start()
        {
            string logDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            logDir += @"\time_log.txt";
            /* Monitor tracks game time */
            monitor = new ActivityMonitor(logDir, track_procs, TICK_PERIOD_SEC);
            monitor.Start();

            /* Supervisor kills game processes if the total playtime has been exceeded */
            supervisorRunning = true;
            gameSupervisor = new System.Threading.Thread(new ThreadStart(GameSupervisorWorker));
            gameSupervisor.Start();
        }

        /// <summary>
        /// Stop the game monitoring system.
        /// </summary>
        public void Stop()
        {
            supervisorRunning = false;
            monitor.Stop();
        }

        /// <summary>
        /// Worker thread for the game supervisor. This thread checks if the total weekly playtime has
        /// been exceeded, and if so, kills all eligible processes. This prevents the user from starting
        /// a game once the weekly time has been exceeded.
        /// </summary>
        void GameSupervisorWorker()
        {
            int lastTime, curTime;
            curTime = 0;
            while (supervisorRunning)
            {
                lastTime = curTime;
                curTime = GetCurrentWeeklyGameTimeSec();
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
                    if (!threshReachedInCurrentSession)
                    {
                        foreach (string proc in track_procs)
                        {
                            if (ProcessManager.KillProcess(proc))
                            {
                                GameStopped?.Invoke(this, new EventArgs());
                            }
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
                if (!monitor.IsProcessRunning)
                {
                    threshReachedInCurrentSession = false;
                }

                /* Sleep for a system tick */
                System.Threading.Thread.Sleep(TICK_PERIOD_SEC);
            }
        }
    }
}
