using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Journey_Of_The_Ship.UI
{
    public class SettingsScreen : UIObject
    {
        public static Texture2D settingsPanel;
        public Vector2 position = new Vector2(Main.desiredResolutionWidth, 0f);     //Off-screen

        private Slider musicVolumeSlider = new Slider(new Vector2(32f, 82f), 45, 4, Color.White, "Music Volume", 2);
        private Slider soundEffectVolumeSlider = new Slider(new Vector2(32f, 92f), 45, 4, Color.Red, "SFX Volume", 2);

        private Color textColor = new Color(88, 240, 99);

        public void InitializeSettingsData()
        {
            musicVolumeSlider.SetValue(Main.musicVolume);
            soundEffectVolumeSlider.SetValue(Main.soundEffectVolume);
        }

        public override void Update()
        {
            musicVolumeSlider.screenPosition = position + new Vector2(musicVolumeSlider.relativePosition.X, musicVolumeSlider.relativePosition.Y);
            soundEffectVolumeSlider.screenPosition = position + new Vector2(soundEffectVolumeSlider.relativePosition.Y, soundEffectVolumeSlider.relativePosition.Y);
            musicVolumeSlider.Update();
            soundEffectVolumeSlider.Update();

            Main.musicVolume = musicVolumeSlider.GetValue();
            Main.soundEffectVolume = soundEffectVolumeSlider.GetValue();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(settingsPanel, position, Color.White);

            musicVolumeSlider.Draw(spriteBatch);
            soundEffectVolumeSlider.Draw(spriteBatch);
        }
    }
}
