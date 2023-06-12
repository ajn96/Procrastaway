using Procrastaway;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ProcrastawayService
{
    using procCore = Procrastaway.core.ProcrastawayCore;

    public partial class Service1 : ServiceBase
    {
        private SettingManager settings;

        public Service1()
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

        protected override void OnStart(string[] args)
        {
            ApplySettings();
            /* Start the  monitor */
            procCore.Instance.Start();
        }

        protected override void OnStop()
        {
            /* Stop the monitor */
            procCore.Instance.Stop();
        }
    }
}
