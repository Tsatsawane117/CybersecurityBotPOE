namespace CybersecurityBotPOE.Helpers
{
    public class AudioHelper
    {
        private const string VoiceGreetingPath = "greeting.wav";

        public void PlayVoiceGreeting()
        {
            try
            {
                if (!File.Exists(VoiceGreetingPath)) return;
                if (OperatingSystem.IsWindows()) PlayOnWindows(VoiceGreetingPath);
            }
            catch { /* Non-fatal — continue without audio */ }
        }

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        private static void PlayOnWindows(string path)
        {
            using var player = new System.Media.SoundPlayer(path);
            player.PlaySync();
        }
    }
}