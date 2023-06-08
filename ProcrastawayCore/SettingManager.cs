using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Procrastaway
{
    public class SettingManager
    {
        /// <summary>
        /// Amount of game time allowed each week, in minutes
        /// </summary>
        public int weekly_game_time_min = 5 * 60;

        /// <summary>
        /// Games to track and limit
        /// </summary>
        public string[] game_process_list =
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
        public bool playtime_report = true;

        /// <summary>
        /// Try to parse user config file for all settings
        /// </summary>
        /// <param name="configPath">Path to the config file</param>
        /// <returns>True is successful parse, false otherwise</returns>
        public bool ParseUserConfig(string configPath)
        {
            string[] contents;
            bool goodFile = false;
            List<string> games = new List<string>();

            try
            {
                contents = File.ReadAllLines(configPath);
                /* Parse allowed time */
                weekly_game_time_min = Convert.ToInt32(contents[1]);
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
        public void CreateDefaultConfigFile(string path)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine("Enter weekly gaming minutes on the line below:");
                writer.WriteLine(weekly_game_time_min.ToString());
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


    }
}
