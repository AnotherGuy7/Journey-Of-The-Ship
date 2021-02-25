using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Journey_Of_The_Ship.UI
{
    public class WarningOverlay : UIObject
    {
        public static Texture2D warningOverlayTexture;
        public static Texture2D warningOverlayMark;

        private Color drawColor = Color.White;

        private Vector2 upperMarkPositions = new Vector2(-12f, 83f);
        private Vector2 lowerMarkPositions = new Vector2(0f, 113f);
        private Vector2 textPosition = new Vector2(Main.desiredResolutionWidth / 2f - 40f, 86f);

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
            if (upperMarkPositions.X >= 0f)
            {
                upperMarkPositions.X = -12f;
            }

            lowerMarkPositions.X -= 1f;
            if (lowerMarkPositions.X <= -12f)
            {
                lowerMarkPositions.X = 0f;
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

            drawColor.R = (byte)MathHelper.Lerp(Color.White.R, Color.Red.R, 1f - ((float)colorChangeTimer / 30f));
            drawColor.G = (byte)MathHelper.Lerp(Color.White.G, Color.Red.G, 1f - ((float)colorChangeTimer / 30f));
            drawColor.B = (byte)MathHelper.Lerp(Color.White.B, Color.Red.B, 1f - ((float)colorChangeTimer / 30f));

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
