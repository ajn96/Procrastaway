using System.Media;

namespace Procrastaway.core
{
    public class SystemSounds
    {
        public enum Sounds
        {
            Started,
            PlaytimeReached,
            GameStopped
        }

        private string soundDir;

        public SystemSounds(string SoundDirectory)
        {
            soundDir = SoundDirectory;
        }

        public void PlaySound(Sounds sound)
        {
            SoundPlayer player = new SoundPlayer(soundDir + sound.ToString() + ".mp3");
            player.Load();
            player.PlaySync();
        }
    }
}
