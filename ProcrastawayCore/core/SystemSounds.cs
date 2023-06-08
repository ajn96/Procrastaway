using System.Media;
using System.Reflection;

namespace Procrastaway.core
{
    public static class SystemSounds
    {
        public enum Sounds
        {
            Started,
            PlaytimeReached,
            GameStopped
        }

        public static void PlaySound(Sounds sound)
        {
            string filename = "";
            switch(sound)
            {
                case Sounds.Started:
                    filename = "started.wav";
                    break;
                case Sounds.PlaytimeReached:
                    filename = "excess_time.wav";
                    break;
                case Sounds.GameStopped:
                    filename = "game_killed.wav";
                    break;
                default:
                    return;
            }
            Assembly a = Assembly.GetExecutingAssembly();
            string[] resourceNames = a.GetManifestResourceNames();

            SoundPlayer player = new SoundPlayer(a.GetManifestResourceStream("Procrastaway.sounds." + filename));
            player.Load();
            player.Play();
        }
    }
}
