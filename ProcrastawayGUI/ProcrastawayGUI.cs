using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Procrastaway;

namespace ProcrastawayGUI
{
    using procCore = Procrastaway.core.ProcrastawayCore;

    public partial class ProcrastawayGUI : Form
    {
        private SettingManager settings;

        private System.Timers.Timer updateTimesTimer;

        public ProcrastawayGUI()
        {
            InitializeComponent();

            string exePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            settings = new SettingManager();

            /* Read in user time or game settings */
            if (!settings.ParseUserConfig(exePath + @"\user_config.txt"))
            {
                settings.CreateDefaultConfigFile(exePath + @"\user_config.txt");
            }
        }

        private void ApplySettings()
        {
            /* Configure the Procrastaway monitor */
            procCore.Instance.SetGameList(settings.game_process_list);
            procCore.Instance.SetWeeklyGameTime(settings.weekly_game_time_min);
        }

        private void ProcrastawayGUI_Load(object sender, EventArgs e)
        {
            ApplySettings();
            /* Start the  monitor */
            procCore.Instance.Start();

            updateTimesTimer = new System.Timers.Timer(500);
            updateTimesTimer.Elapsed += UpdateTimeDisplay;
            updateTimesTimer.Start();
        }

        private void UpdateTimeDisplay(Object source, ElapsedEventArgs e)
        {
            int totalSecs = settings.weekly_game_time_min * 60;
            int remainingSecs = totalSecs - procCore.Instance.GetCurrentWeeklyGameTimeSec();
            int h = totalSecs / (60 * 60);
            totalSecs -= (h * (60 * 60));
            int m = totalSecs / (60);
            totalSecs -= (m * 60);
            timeReport.Invoke((MethodInvoker)delegate
            {
                timeReport.Text = h.ToString() + "h " + m.ToString() + "m " + totalSecs.ToString() + "s";
            });
        }

        private void ProcrastawayGUI_Close(object sender, EventArgs e)
        {
            /* Stop the monitor */
            procCore.Instance.Stop();
        }
    }
}
