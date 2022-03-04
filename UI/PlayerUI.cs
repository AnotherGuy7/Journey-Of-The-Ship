using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Journey_Of_The_Ship.UI
{
    public class PlayerUI : UIObject
    {
        public static Texture2D playerHealthOutlines;
        public static Texture2D playerHealthMark;
        public static Texture2D clearEnvironmentIcon;
        public static Texture2D asteroidEnvironmentIcon;
        public static Texture2D[] abilityIcons = new Texture2D[4];
        public static Texture2D abilityBorder;
        public static float uiAlpha = 0f;

        private Vector2 scorePosition = new Vector2(1f, 1f);
        private Vector2 healthPosition = new Vector2(113f, 1f);
        private Vector2 healthBarOffset = new Vector2(26f, 5f);
        private Vector2 environmentIconPosition = new Vector2(1f, 174f);

        private Color healthGreen = new Color(142, 246, 103);
        private Color healthRed = new Color(246, 103, 103);

        private Color healthMarkDrawColor = Color.White;
        private Color abilityDrawColor = Color.White;

        private Vector2 abilityRequirementTextSize;
        private int previousAbilityNumber = -1;

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

            if (previousAbilityNumber != Main.player.killsNeededForAbility)
            {
                float fadeColor = MathHelper.Lerp(0f, 1f, 1f - (Main.player.killsNeededForAbility / (float)Main.player.killsNeededRequirement));
                abilityDrawColor = new Color(fadeColor, fadeColor, fadeColor);

                abilityRequirementTextSize = Main.mainFont.MeasureString(Main.player.killsNeededForAbility.ToString()) * 0.3f;
                previousAbilityNumber = Main.player.killsNeededForAbility;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(playerHealthOutlines, healthPosition, Color.White * uiAlpha);

            for (int h = 0; h < Main.playerHealth; h++)
            {
                Vector2 pos = healthPosition + healthBarOffset + new Vector2(-5f * h, 0f);
                spriteBatch.Draw(playerHealthMark, pos, healthMarkDrawColor * uiAlpha);
            }

            spriteBatch.DrawString(Main.mainFont, "Score: " + Main.gameScore, scorePosition, Color.White * uiAlpha, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(Main.mainFont, "HS: " + Main.highScore, scorePosition + new Vector2(0f, 8f), Color.White * uiAlpha, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);

            if (Player.abilityType != Player.AbilityType.None)
            {
                Vector2 abilityIconPosition = new Vector2(Main.desiredResolutionWidth - 22f, Main.desiredResolutionHeight - 22f);
                Vector2 abilityIconCenter = abilityIconPosition + new Vector2(abilityBorder.Width / 2f, abilityBorder.Height / 2f);
                Vector2 abilityIconRequirementNumberPosition = abilityIconPosition + new Vector2(abilityBorder.Width, abilityBorder.Height) - abilityRequirementTextSize + new Vector2(2f, 2f);
                spriteBatch.Draw(abilityBorder, abilityIconPosition, Color.White * uiAlpha);
                spriteBatch.Draw(abilityIcons[(int)Player.abilityType - 1], abilityIconCenter, null, abilityDrawColor * uiAlpha, 0f, new Vector2(10f, 10f), 1f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(Main.mainFont, Main.player.killsNeededForAbility.ToString(), abilityIconRequirementNumberPosition, Color.White * uiAlpha, 0f, new Vector2(10f, 10f), 0.3f, SpriteEffects.None, 0f);
            }

            DrawEnvironmentalIcon(spriteBatch);
        }

        private void DrawEnvironmentalIcon(SpriteBatch spriteBatch)
        {
            switch (Main.activeEvent)
            {
                case Main.Events.None:
                    spriteBatch.Draw(clearEnvironmentIcon, environmentIconPosition, Color.White * uiAlpha);
                    break;
                case Main.Events.AsteroidField:
                    spriteBatch.Draw(asteroidEnvironmentIcon, environmentIconPosition, Color.White * uiAlpha);
                    break;
            }
        }
    }
}
