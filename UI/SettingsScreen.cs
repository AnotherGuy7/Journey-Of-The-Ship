using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Journey_Of_The_Ship.UI
{
    public class SettingsScreen : UIObject
    {
        public static Texture2D settingsPanel;
        public Vector2 position = new Vector2(Main.desiredResolutionWidth, 0f);     //Off-screen

        private Color textColor = new Color(88, 240, 99);
        private Slider musicVolumeSlider = new Slider(new Vector2(64f, 82f), 45, 4, Color.White, "Music Volume", Main.musicVolume, 2);
        private Slider soundEffectVolumeSlider = new Slider(new Vector2(56f, 94f), 45, 4, Color.White, "SFX Volume", Main.soundEffectVolume, 2);

        private Vector2 settingsTextSize = Main.mainFont.MeasureString("SETTINGS");

        public override void Update()
        {
            musicVolumeSlider.screenPosition = position + musicVolumeSlider.relativePosition;
            soundEffectVolumeSlider.screenPosition = position + soundEffectVolumeSlider.relativePosition;
            musicVolumeSlider.Update();
            soundEffectVolumeSlider.Update();

            Main.musicVolume = musicVolumeSlider.GetValue();
            Main.soundEffectVolume = soundEffectVolumeSlider.GetValue();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(settingsPanel, position, Color.White);

            spriteBatch.DrawString(Main.mainFont, "SETTINGS", position + new Vector2(settingsPanel.Width / 2f, settingsTextSize.Y - 2f), textColor, 0f, settingsTextSize / 2f, 1f, SpriteEffects.None, 0f);
            musicVolumeSlider.Draw(spriteBatch);
            soundEffectVolumeSlider.Draw(spriteBatch);
        }
    }
}
