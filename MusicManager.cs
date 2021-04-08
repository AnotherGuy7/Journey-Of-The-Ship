using Microsoft.Xna.Framework.Media;

namespace Journey_Of_The_Ship
{
    public class MusicManager
    {
        public static Song titleMusic;
        public static Song mainMusic;

        private bool fadingOut = false;
        private int fadeOutDuration = 0;
        private int fadeOutTimer = 0;
        private Song nextSong;

        private bool fadingIn = false;
        private int fadeInDuration = 0;
        private int fadeInTimer = 0;

        public void UpdateMusic()
        {
            if (fadingOut)
                FadeOut();

            if (fadingIn)
                FadeIn();

            if (!fadingOut && !fadingIn)
            {
                MediaPlayer.Volume = Main.musicVolume;
            }

            if (MediaPlayer.Volume <= 0.01f)
                return;

            if (MediaPlayer.State == MediaState.Playing)
                return;

            if (Main.gameState == Main.GameStates.GameState_Title)
            {
                MediaPlayer.Play(titleMusic);
            }
            else if (Main.gameState == Main.GameStates.GameState_Playing)
            {
                MediaPlayer.Play(mainMusic);
                if (Main.activeEvent == Main.Events.None)
                {
                }
            }
        }

        public static void FadeOutInto(Song song, int fadeOutTime, int fadeInTime = 180)
        {
            Main.musicManager.nextSong = song;
            Main.musicManager.fadingOut = true;
            Main.musicManager.fadeOutDuration = fadeOutTime;
            Main.musicManager.fadeInDuration = fadeInTime;
        }

        public void FadeOut()
        {
            if (fadeOutTimer < fadeOutDuration)
                fadeOutTimer++;

            MediaPlayer.Volume = (((float)fadeOutDuration - (float)fadeOutTimer) / (float)fadeOutDuration) * Main.musicVolume;

            if (fadeOutTimer >= fadeOutDuration)
            {
                fadingOut = false;
                fadingIn = true;
                fadeOutTimer = 0;
                fadeOutDuration = 0;
                MediaPlayer.Play(nextSong);
                nextSong = null;
            }
        }

        public void FadeIn()
        {
            if (fadeInTimer < fadeInDuration)
                fadeInTimer++;

            MediaPlayer.Volume = ((float)fadeInTimer / (float)fadeInDuration) * Main.musicVolume;

            if (fadeInTimer >= fadeInDuration)
            {
                fadingIn = false;
                fadeInTimer = 0;
                fadeInDuration = 0;
            }
        }
    }
}
