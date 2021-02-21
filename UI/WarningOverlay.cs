using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Journey_Of_The_Ship.UI
{
    public class WarningOverlay : UIObject
    {
        public static Texture2D warningOverlayTexture;
        public static Texture2D warningOverlayMark;

        private Color drawColor = Color.White;

        private Vector2 upperMarkPositions = new Vector2(0f, 83f);
        private Vector2 lowerMarkPositions = new Vector2(0f, 113f);
        private Vector2 textPosition = new Vector2(4f, 82f);

        private int showTimer = 0;
        private int colorChangeTimer;
        private bool subtractColorTimer = false;

        public static void ShowWarning(int showTime)
        {
            WarningOverlay newInstance = new WarningOverlay();
            newInstance.showTimer = showTime;
            Main.activeUI.Add(newInstance);
        }

        public override void Update()
        {
            upperMarkPositions.X += 1f;
            if (upperMarkPositions.X >= 12f)
            {
                upperMarkPositions.X = 0f;
            }

            upperMarkPositions.X -= 1f;
            if (upperMarkPositions.X <= -12f)
            {
                upperMarkPositions.X = 0f;
            }

            if (colorChangeTimer >= 30)
            {
                subtractColorTimer = true;
            }
            if (colorChangeTimer <= 0)
            {
                subtractColorTimer = false;
            }
            if (!subtractColorTimer)
            {
                colorChangeTimer++;
            }
            else
            {
                colorChangeTimer--;
            }

            float red = MathHelper.Lerp(Color.White.R, Color.Red.R, (float)colorChangeTimer / 30f);
            float green = MathHelper.Lerp(Color.White.R, Color.Red.R, (float)colorChangeTimer / 30f);
            float blue = MathHelper.Lerp(Color.White.R, Color.Red.R, (float)colorChangeTimer / 30f);
            drawColor = new Color(red, green, blue);

            showTimer--;
            if (showTimer <= 0)
            {
                DestroyInstance(this);
            }
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int m = 0; m < 24; m++)
            {
                Vector2 pos = upperMarkPositions + new Vector2(12f * m, 0f);
                spriteBatch.Draw(warningOverlayMark, pos, null, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            for (int m = 0; m < 24; m++)
            {
                Vector2 pos = lowerMarkPositions + new Vector2(12f * m, 0f);
                spriteBatch.Draw(warningOverlayMark, pos, null, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0f);
            }

            spriteBatch.Draw(warningOverlayTexture, Vector2.Zero, drawColor);
            spriteBatch.DrawString(Main.mainFont, "WARNING!", textPosition, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
