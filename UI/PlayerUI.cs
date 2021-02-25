using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Journey_Of_The_Ship.UI
{
    public class PlayerUI : UIObject
    {
        public static Texture2D playerHealthOutlines;
        public static Texture2D playerHealthMark;

        private Vector2 scorePosition = new Vector2(1f, 1f);
        private Vector2 healthPosition = new Vector2(113f, 1f);
        private Vector2 healthBarOffset = new Vector2(26f, 5f);

        private Color healthGreen = new Color(142, 246, 103);
        private Color healthRed = new Color(246, 103, 103);

        private Color healthMarkDrawColor = Color.White;

        public static void InitializePlayerUI()
        {
            PlayerUI newInstance = new PlayerUI();
            Main.activeUI.Add(newInstance);
        }

        public override void Update()
        {
            healthMarkDrawColor.R = (byte)MathHelper.Lerp(healthGreen.R, healthRed.R, 1f - (Main.playerHealth / 6f));
            healthMarkDrawColor.G = (byte)MathHelper.Lerp(healthGreen.G, healthRed.G, 1f - (Main.playerHealth / 6f));
            healthMarkDrawColor.B = (byte)MathHelper.Lerp(healthGreen.B, healthRed.B, 1f - (Main.playerHealth / 6f));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(playerHealthOutlines, healthPosition, Color.White);

            for (int h = 0; h < Main.playerHealth; h++)
            {
                Vector2 pos = healthPosition + healthBarOffset + new Vector2(-5f * h, 0f);
                spriteBatch.Draw(playerHealthMark, pos, healthMarkDrawColor);
            }

            spriteBatch.DrawString(Main.mainFont, "Score: " + Main.gameScore, scorePosition, Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);

            DrawEnvironmentalIcon(spriteBatch);
        }

        private void DrawEnvironmentalIcon(SpriteBatch spriteBatch)
        {
            switch (Main.activeEvent)
            {
                case Main.Events.None:
                    break;
                case Main.Events.AsteroidField:
                    break;
            }
        }
    }
}
