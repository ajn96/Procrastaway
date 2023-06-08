using System;
using System.Diagnostics;

namespace Procrastaway.core
{
    /// <summary>
    /// Wrapper to allow basic process management
    /// </summary>
    static class ProcessManager
    {
        /// <summary>
        /// Check if a process is currently running
        /// </summary>
        /// <param name="processName">The process name (such as "hl2.exe")</param>
        /// <returns>True if running, false if not</returns>
        public static bool IsProcessRunning(string processName)
        {
            Process[] proclist = Process.GetProcesses();
            foreach (Process proc in proclist)
            {
                if (MatchProcessNames(processName, proc))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Kill a running process.
        /// </summary>
        /// <param name="processName">The process name (such as "hl2.exe")</param>
        /// <returns>True if found and killed, false if insufficient permission or invalid process name</returns>
        public static bool KillProcess(string processName)
        {
            bool success = false;
            Process[] procList = Process.GetProcesses();
            /* Wrap in try catch in case we don't have permissions */
            try
            {
                foreach (Process proc in procList)
                {
                    if (MatchProcessNames(processName, proc))
                    {
                        proc.CloseMainWindow();
                        proc.Kill();
                        success = true;
                    }
                }
            }
            catch
            {
                /* We failed to close process */
                success = false;
            }
            return success;
        }

        /// <summary>
        /// Helper functions to check if a game name matches a running process. Ignores case and .exe file extension.
        /// </summary>
        /// <param name="name">Game name, which may or may not have .exe</param>
        /// <param name="proc">Proccess to check against</param>
        /// <returns>True if match, false if no match</returns>
        private static bool MatchProcessNames(string name, Process proc)
        {
            if (proc.ProcessName == name)
            {
                return true;
            }
            /* Without exe and case insensitive */
            name = name.Replace(".exe", "");
            if (String.Equals(name, proc.ProcessName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }


    }
}
