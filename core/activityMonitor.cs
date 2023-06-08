using Procrastaway.core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Procrastaway
{
    class activityMonitor
    {
        private string logPath;
        private string[] procList;
        private int samplePeriod;
        private List<long> timeFIFO;
        private Mutex fifoMutex;
        private Timer sampleTimer;

        /// <summary>
        /// Create a new activity monitor from the specified log file
        /// </summary>
        /// <param name="logFile">File containing the playtime FIFO</param>
        public activityMonitor(string logFilePath, string [] procs, int update_period_sec)
        {
            logPath = logFilePath;
            procList = procs;
            if (update_period_sec < 1)
                throw new ArgumentException("Invalid update rate for the process monitor");
            samplePeriod = update_period_sec;
            fifoMutex = new Mutex();
        }

        ~activityMonitor()
        {
            stop();
        }

        /// <summary>
        /// Report if monitor is running or not
        /// </summary>
        public bool IsMonitorRunning { get; private set; } = false;

        /// <summary>
        /// Report if a tracked process is running or not
        /// </summary>
        public bool IsProcessRunning { get; private set; } = false;

        public void start()
        {
            if(IsMonitorRunning)
            {
                /* I could probably not do this and just return? 
                 But I think its better to tell people something bad happened.
                Or I could be lazy and make it just "work" by re-applying settings,
                but lets save that for a PR */
                throw new Exception("Must stop monitor before starting again");
            }
            timeFIFO = new List<long>();
            /* Read the current time log */
            try
            {
                string[] logTxt = File.ReadAllLines(logPath);
                foreach (string line in logTxt)
                {
                    timeFIFO.Add(Convert.ToInt64(line));
                }
            }
            catch
            {
                /* Invalid time FIFO */
                timeFIFO.Clear();
            }

            sampleTimer = new System.Timers.Timer(1000 * samplePeriod);
            sampleTimer.Elapsed += checkForRunningProcesses;
            sampleTimer.Start();
            IsMonitorRunning = true;
        }

        public void stop()
        {
            fifoMutex.WaitOne();
            sampleTimer.Stop();
            sampleTimer.Dispose();

            WriteLog();
            fifoMutex.Dispose();
            IsMonitorRunning = false;
        }

        private void checkForRunningProcesses(Object source, ElapsedEventArgs e)
        {
            bool found = false;
            foreach(string proc in procList)
            {
                if(ProcessManager.IsProcessRunning(proc))
                {
                    found = true;
                    timeFIFO.Add(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                    IsProcessRunning = true;
                    WriteLog();
                    break;
                }
            }
            if(!found)
            {
                IsProcessRunning = false;
            }
        }

        /// <summary>
        /// Write time FIFO contents to log file. ASCII text, one timestamp per line.
        /// </summary>
        private void WriteLog()
        {
            /* Write the log FIFO to the file */
            using (StreamWriter writer = new StreamWriter(logPath))
            {
                foreach (long time in timeFIFO)
                {
                    writer.WriteLine(time);
                }
            }
        }

        /// <summary>
        /// Calculate how much time has been reported into the time FIFO.
        /// </summary>
        /// <returns>The total process time in the time FIFO, in seconds</returns>
        public int getCurrentLoggedTimeSec()
        {
            /* Lock the time fifo */
            fifoMutex.WaitOne();
            /* Get the current UTC time and find a week ago */
            long curTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long startTime = curTime - (7 * 24 * 60 * 60);
            /* Remove all items from time FIFO which are older */
            List<long> newFIFO = new List<long>();
            foreach(long time in timeFIFO)
            {
                if(time > startTime)
                {
                    newFIFO.Add(time);
                }
            }
            timeFIFO.Clear();
            timeFIFO.AddRange(newFIFO);
            fifoMutex.ReleaseMutex();
            return newFIFO.Count() * samplePeriod;
        }
    }
}
